using static Microsoft.Maui.ApplicationModel.Permissions;
using System.Text;
using System.Text.Json;
using AttendanceRegisterMAUIapp.Pages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using AttendanceRegisterMAUIapp.Models;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing.Imaging;
using AttendanceRegisterMAUIapp.Models.Request;
using System.Diagnostics;

namespace AttendanceRegisterMAUIapp
{
    public static class AppSession
    {
        public static string UserId { get; set; }
        public static string Image { get; set; }
    }

    public partial class MainPage : ContentPage
    {
        private readonly IConfiguration _configurationService;
        private const string ApiUrl = "https://eu.opencv.fr/search";
        private const string ApiKey = "";

        public MainPage(IConfiguration configurationService)
        {
            InitializeComponent();
            _configurationService = configurationService;
        }

        private void cameraView_CamerasLoaded(object sender, EventArgs e)
        {
            cameraView.Camera = cameraView.Cameras.AsEnumerable().FirstOrDefault(x => x.Position == Camera.MAUI.CameraPosition.Front);
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            });
        }

        private async void BtnSubmitBrowse_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Please select an image file",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    // Open a read stream from the picked file
                    var stream = await result.OpenReadAsync();

                    // Set the image source to the Image control
                    myImage.Source = ImageSource.FromStream(() => stream);

                    // Call the AzureApi method with the image stream
                     FaceApi(stream);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
        private async void BtnSubmit_Clicked(object sender, EventArgs e)

        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Stream imageStream = await ((StreamImageSource)cameraView.GetSnapShot(Camera.MAUI.ImageFormat.PNG)).Stream(CancellationToken.None);
                FaceApi(imageStream);
                // OpenCVRestAPI(imageStream);
                stopwatch.Stop();
                var totalExecutionTime = stopwatch.Elapsed.TotalSeconds;
                string log = "..........Verification successfully completed............" + Environment.NewLine +
                    "..........Attendance marked successfully ............";
                    log +=Environment.NewLine+ string.Format("Completed in  {0} seconds", totalExecutionTime);
                log += Environment.NewLine + "......................................................";
                System.Diagnostics.Debug.WriteLine(log);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }


        private async void FaceApi(Stream imageStream)
        {
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Add("accept", "application/json");
                byte[] imageBytes;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await imageStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    imageBytes = memoryStream.ToArray();
                }

                string base64Image = Convert.ToBase64String(imageBytes);
                myImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                var api = _configurationService.GetRequiredSection("ApiBaseUrl").Value;
                System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();
                _httpClient.BaseAddress = new Uri(api);
                string jsonRequest = JsonConvert.SerializeObject(base64Image);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("Face/Recognize", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var verificationResponse = JsonConvert.DeserializeObject<VerificationResponseAPI>(responseContent);
                    if (verificationResponse.Id <= 0)
                    {
                        await DisplayAlert("Error", verificationResponse.Message, "OK");

                    }
                    else
                    {
                        AppSession.UserId = verificationResponse.Id.ToString();
                        AppSession.Image = verificationResponse.Image;
                        await DisplayAlert("Success", verificationResponse.Message, "OK");
                        await Navigation.PushAsync(new AttendanceConfirm(AppSession.UserId, _configurationService));
                    }
                }
            }
        }
        private async void OpenCVRestAPI(Stream imageStream)
        {
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Add("accept", "application/json");
                client.DefaultRequestHeaders.Add("x-api-key", ApiKey);
                client.BaseAddress = new Uri(ApiUrl);

                byte[] imageBytes;
                Bitmap bitmap;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await imageStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    bitmap = new Bitmap(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }

                //Step 2: Convert the Bitmap to Emgu.CV.Image<Bgr, Byte>
                Image<Bgr, Byte> emguImage = BitmapToImage(bitmap);

                //Convert the image data to a base64 - encoded string
                string base64Image = Convert.ToBase64String(imageBytes);
                // Prepare the request payload
                string jsonPayload = @"{
                ""images"": [""" + base64Image + @"""],
                ""max_results"": 10,
                ""min_score"": 0.7,
                ""search_mode"": ""ACCURATE""
            }";

                HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                System.Net.Http.HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(responseContent);
                    List<VerificationRepsonse> responseItems = JsonConvert.DeserializeObject<List<VerificationRepsonse>>(responseContent);
                    string id = responseItems[0].Id;
                    string image = responseItems[0].Thumbnails[0].Thumbnail;
                    AppSession.UserId = id;
                    AppSession.Image = image;
                }
            };
        }
        private Image<Bgr, byte> BitmapToImage(Bitmap bitmap)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int numberOfChannels = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            Image<Bgr, byte> img = new Image<Bgr, byte>(bitmap.Width, bitmap.Height, bmpData.Stride, bmpData.Scan0);
            bitmap.UnlockBits(bmpData);
            return img;
        }

        private async void BtnProfile_Clicked(object sender, EventArgs e)
        {

        }

        private async void BtnAttendanceHistory_Clicked(object sender, EventArgs e)
        {

        }
    }
}
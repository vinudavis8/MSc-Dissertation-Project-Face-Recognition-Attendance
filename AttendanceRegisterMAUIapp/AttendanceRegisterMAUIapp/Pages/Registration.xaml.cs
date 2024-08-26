using AttendanceRegisterMAUIapp.Models;
using AttendanceRegisterMAUIapp.Models.Request;
using Camera.MAUI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace AttendanceRegisterMAUIapp.Pages;

public partial class Registration : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configurationService;
    private string apiUrl;
    private const string ApiUrl = "https://eu.opencv.fr/person";
    private const string ApiKey = "";
    Stream stream;
    string base64Image1, base64Image2, base64Image3;

    public Registration(IConfiguration configurationService)
	{
		InitializeComponent();
        _configurationService = configurationService;

         apiUrl = configurationService.GetRequiredSection("ApiBaseUrl").Value;
        // Initialize HttpClient
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(apiUrl);
         LoadDDL();
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
    public async void LoadDDL()
    {
        var response = await _httpClient.GetAsync("Department");
        if (response.IsSuccessStatusCode)
        {
            var departmentsJson = await response.Content.ReadAsStringAsync();
            var departments = JsonConvert.DeserializeObject<List<Department>>(departmentsJson);
            DepartmentPicker.ItemsSource = departments;
        }

    }
    public async void LoadCourseDDL(int departmentId)
    {
        var response = await _httpClient.GetAsync("Course/GetCoursedByDepId/" + departmentId);
        if (response.IsSuccessStatusCode)
        {
            var courseJson = await response.Content.ReadAsStringAsync();
            var courses = JsonConvert.DeserializeObject<List<Course>>(courseJson);
            CoursePicker.ItemsSource = courses;
        }
    }
    private void DepartmentPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Handle department selection change
        var selectedDepartment = (Department)DepartmentPicker.SelectedItem;
        if (selectedDepartment != null)
        {
            LoadCourseDDL(selectedDepartment.Id);
        }
    }
    private async void OnSubmitButtonClicked(object sender, EventArgs e)
    {
        var stopwatch = Stopwatch.StartNew();
        try {
            // Retrieve values from the input fields
            string firstName = FirstNameEntry.Text;
            string lastName = LastNameEntry.Text;
            //DateTime dateOfBirth = DateOfBirthPicker.Date;
            string email = EmailEntry.Text;
            string phoneNumber = PhoneNumberEntry.Text;
            string address = AddressEntry.Text;
            int gradeLevel = 7;
            var selectedDepartment = (Department)DepartmentPicker.SelectedItem;
            int departmentId = selectedDepartment.Id;
            var selectedCourse = (Course)CoursePicker.SelectedItem;
            int courseId = selectedCourse.Id;



            // Construct the request DTO
            var addStudentRequest = new AddStudentRequestDTO
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = DateOfBirthPicker.Date,
                Email = email,
                PhoneNumber = phoneNumber,
                Address = address,
                GradeLevel = gradeLevel,
                CourseId = courseId
            };

            string jsonRequest = JsonConvert.SerializeObject(addStudentRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Student", content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                //bool result=await RegisterInOpenCVAsync(Convert.ToInt32( responseContent));
                bool result = await RegisterInAzureAsync(Convert.ToInt32(responseContent));

                if (result)
                await DisplayAlert("Success", "Student registration successful", "OK");
                else
                    await DisplayAlert("Failed", "Student registration Failed", "OK");

                await Navigation.PushAsync(new Login(_configurationService));

            }
            else
            {
                // Handle failure
                await DisplayAlert("Error", "Failed to register student", "OK");
            }
        }
        catch (Exception ex)
        {

        }
        stopwatch.Stop();
        var totalExecutionTime = stopwatch.Elapsed.TotalSeconds;
        string log = string.Format("Completed in  {0} seconds", totalExecutionTime);
        System.Diagnostics.Debug.WriteLine("..........enrolment successfully completed............");
        System.Diagnostics.Debug.WriteLine(log);
        System.Diagnostics.Debug.WriteLine("......................................................");
        Console.WriteLine(log);
    }

    public async Task<bool> RegisterInOpenCVAsync(int studentId)
    {
        bool status = false;
        try
        {
            var photo = cameraView.GetSnapShot(Camera.MAUI.ImageFormat.PNG);
            base64Image1 = await ImageToBase64(photo);
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("accept", "application/json");
                client.DefaultRequestHeaders.Add("x-api-key", ApiKey);
                string firstName = FirstNameEntry.Text;
                string lastName = LastNameEntry.Text;


//                string json = @"
//{
//""collections"": [ """" ],
//""date_of_birth"": ""2024-04-11"",
//""gender"": ""M"",
//""id"": """ + studentId + @""",
//""images"": [ """ + base64Image + @""" ],
//""is_bulk_insert"": false,
// ""name"":  """ + firstName+ " "+ lastName + @"""
//""nationality"": ""Indian"",
// ""notes"": ""string""
//}";
                string json = @"
{
""collections"": [""""],
""date_of_birth"": ""2024-04-11"",
""gender"": ""M"",
""id"": """ + studentId + @""",
""images"": [""" + base64Image1 + @"""],
""is_bulk_insert"": false,
""name"":  """ + firstName+ " "+ lastName + @""",
""nationality"": ""Indian"",
""notes"": ""string""
}";


                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                     responseContent = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(responseContent);
                    JsonElement root = jsonDocument.RootElement;
                    if (root.TryGetProperty("id", out JsonElement idElement))
                    {
                        string idValue = idElement.GetString();
                        if (!string.IsNullOrEmpty(idValue))
                            status= true;
                        else
                            status= false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return false;
        }
        return status;
    }
    public async Task<bool> RegisterInAzureAsync(int studentId)
    {
        string firstName = FirstNameEntry.Text;
        string lastName = LastNameEntry.Text;
        //var photo = cameraView.GetSnapShot(Camera.MAUI.ImageFormat.PNG);
        //stream = await ((StreamImageSource)photo).Stream(CancellationToken.None);

        using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
        {
            client.DefaultRequestHeaders.Add("accept", "application/json");
            //byte[] imageBytes;
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    await stream.CopyToAsync(memoryStream);
            //    memoryStream.Position = 0;
            //    imageBytes = memoryStream.ToArray();
            //}

            //string base64Image = Convert.ToBase64String(imageBytes);
            List<string> list = new List<string>();
            list.Add(base64Image1);
            list.Add(base64Image2);
            list.Add(base64Image3);
            var api = _configurationService.GetRequiredSection("ApiBaseUrl").Value;
            System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();
            _httpClient.BaseAddress = new Uri(api);
            var request = new TrainModelRequest
            {
                Base64Image = list,
                StudentId = studentId.ToString()
            };
            string jsonRequest = JsonConvert.SerializeObject(request);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Face/TrainModel", content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                if(responseContent.Contains("Already Registered"))
                {
                    await DisplayAlert("Failed", responseContent, "OK");
                    return false;
                }
                return true;
            }
            string responseContent1 = await response.Content.ReadAsStringAsync();

            return false;
        }
    }

    private async Task<string> ImageToBase64(ImageSource source)
    {
        try
        {
            
            // Convert the stream to a byte array
            using (MemoryStream ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                byte[] imageBytes = ms.ToArray();

                // Convert the byte array to a base64 string
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting image to base64: {ex.Message}");
            return null;
        }
    }
    private async void ButtonImage1_Clicked(object sender, EventArgs e)
    {

        var imageBytes = await GetImage();
        myImage1.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        base64Image1 = Convert.ToBase64String(imageBytes);
    }
    private async void ButtonImage2_Clicked(object sender, EventArgs e)
    {
        var imageBytes = await GetImage();
        myImage2.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        base64Image2 = Convert.ToBase64String(imageBytes);
    }
    private async void ButtonImage3_Clicked(object sender, EventArgs e)
    {
        var imageBytes = await GetImage();
        myImage3.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        base64Image3 = Convert.ToBase64String(imageBytes);
    }
    private async Task<byte[]> GetImage()
    {
        byte[] imageBytes = null;
        Stream imageStream = await ((StreamImageSource)cameraView.GetSnapShot(Camera.MAUI.ImageFormat.PNG)).Stream(CancellationToken.None);
        using (MemoryStream memoryStream = new MemoryStream())
        {
            await imageStream.CopyToAsync(memoryStream);
            imageBytes = memoryStream.ToArray();
            return imageBytes;
        }
    }

}
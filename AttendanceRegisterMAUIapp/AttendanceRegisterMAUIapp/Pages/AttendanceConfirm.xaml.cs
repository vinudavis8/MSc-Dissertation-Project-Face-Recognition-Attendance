using AttendanceRegisterMAUIapp.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using Plugin.Maui.Calendar.Models;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Calendar.Models;

namespace AttendanceRegisterMAUIapp.Pages;

public partial class AttendanceConfirm : ContentPage
{
    private readonly IConfiguration _configurationService;
    private readonly HttpClient _httpClient;
    public EventCollection Events { get; set; }
    public AttendanceConfirm(string studentId, IConfiguration configurationService)
    {
        InitializeComponent();
        var apiUrl = configurationService.GetRequiredSection("ApiBaseUrl").Value;
        _configurationService = configurationService;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(apiUrl);
        LoadStudentDetails(studentId);
        LoadAttendanceHistory();
    }



    public async void LoadStudentDetails(string studentId)
    {
        var response = await _httpClient.GetAsync("Student/" + studentId);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            Student student = JsonConvert.DeserializeObject<Student>(json);
            Name.Text = student.FirstName + " " + student.LastName;
            Department.Text = student.Course.Department.Name;
            Course.Text = student.Course.Name;
            var imageBytes = Convert.FromBase64String(AppSession.Image);
            MemoryStream imageDecodeStream = new(imageBytes);
            img.Source = ImageSource.FromStream(() => imageDecodeStream);

            var schedule = await _httpClient.GetAsync("CourseModule/GetSchedule?courseId=" + student.CourseId);
            if (response.IsSuccessStatusCode)
            {

                var jsonSchedule = await schedule.Content.ReadAsStringAsync();
                List<Schedule> sch = JsonConvert.DeserializeObject<List<Schedule>>(jsonSchedule);
                List<ComboBoxItem> moduleItems = new List<ComboBoxItem>();
                foreach (var item in sch)
                {
                    moduleItems.Add(new ComboBoxItem
                    {
                        Id = item.CourseModuleId,
                        DisplayText = item.CourseModule.Name
                    });
                }
            }
        }
    }
    public async void LoadAttendanceHistory()
    {
        var schedule = await _httpClient.GetAsync("Attendance?studentId=" + AppSession.UserId);
        var jsonSchedule = await schedule.Content.ReadAsStringAsync();
        List<Attendance> sch = JsonConvert.DeserializeObject<List<Attendance>>(jsonSchedule);
        AttendanceListView.ItemsSource = sch;
    }
    private async void ModulePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        //var selectedModule = (ComboBoxItem)modules.SelectedItem;


        //// Construct the request DTO
        //var request = new Attendance
        //{
        //    StudentId= Convert.ToInt32( AppSession.UserId),
        //    CourseModuleId= selectedModule.Id,
        //    IsPresent=true
        //};
        //// Serialize the DTO to JSON
        //string jsonRequest = JsonConvert.SerializeObject(request);

        //// Post the JSON data to the API endpoint
        //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        //var response = await _httpClient.PostAsync("Attendance", content);

        //// Check if the request was successful
        //if (response.IsSuccessStatusCode)
        //{
        //    string responseContent = await response.Content.ReadAsStringAsync();
        //    await DisplayAlert("Success", "Attendance Registered", "OK");
        //    await Navigation.PushAsync(new AttendanceHistory(_configurationService));

        //}
        //else
        //{
        //    // Handle failure
        //    await DisplayAlert("Error", "Failed to register student", "OK");
        //}
    }

}

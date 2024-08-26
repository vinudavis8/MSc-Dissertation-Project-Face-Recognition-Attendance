using AttendanceRegisterMAUIapp.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AttendanceRegisterMAUIapp.Pages;

public partial class AttendanceHistory : ContentPage
{
    private readonly IConfiguration _configurationService;
    private readonly HttpClient _httpClient;
    public AttendanceHistory(IConfiguration configurationService)
	{
		InitializeComponent();
        var apiUrl = configurationService.GetRequiredSection("ApiBaseUrl").Value;
        _configurationService = configurationService;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(apiUrl);
        LoadAttendanceHistory();
    }

    public async void LoadAttendanceHistory()
    {
            var schedule = await _httpClient.GetAsync("Attendance?studentId=" + AppSession.UserId);
                var jsonSchedule = await schedule.Content.ReadAsStringAsync();
                List<Attendance> sch = JsonConvert.DeserializeObject<List<Attendance>>(jsonSchedule);
        AttendanceListView.ItemsSource = sch;
    }
}
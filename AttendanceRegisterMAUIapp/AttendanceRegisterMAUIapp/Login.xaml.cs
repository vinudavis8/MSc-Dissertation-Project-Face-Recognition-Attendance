using AttendanceRegisterMAUIapp.Pages;
using Microsoft.Extensions.Configuration;

namespace AttendanceRegisterMAUIapp;

public partial class Login : ContentPage
{
    private const string CorrectPin = "1234";
    private readonly IConfiguration _configuration;

    public Login(IConfiguration configuration)
    {
        _configuration = configuration;
        InitializeComponent();
	}
    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        string enteredPin = Pin.PINValue;

        if (string.IsNullOrEmpty(enteredPin))
        {
            await DisplayAlert("Error", "Please enter a PIN", "OK");
            return;
        }

        if (enteredPin == CorrectPin)
        {
            await DisplayAlert("Success", "Login Successful", "OK");
            // Navigate to the main page or dashboard
            await Navigation.PushAsync(new MainPage(_configuration));
        }
        else
        {
            await DisplayAlert("Error", "Incorrect PIN", "OK");
        }
    }
    private async void OnRegisterLabelTapped(object sender, EventArgs e)
    {
        // Navigate to the registration page
        await Navigation.PushAsync(new Registration(_configuration));
    }
}
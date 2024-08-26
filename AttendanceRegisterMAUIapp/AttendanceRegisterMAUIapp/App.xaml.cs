using AttendanceRegisterMAUIapp.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace AttendanceRegisterMAUIapp
{
    public partial class App : Application
    {
        private readonly IConfiguration _configuration;

        public App(IConfiguration configuration)
        {
            _configuration = configuration;

            //MainPage = new AppShell();
            //MainPage = new MainPage(configuration);
            MainPage = new NavigationPage(new Login(_configuration));

           // MainPage = new NavigationPage(new MainPage(configuration));
        }

    }
}
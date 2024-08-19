using AppLanches.Pages;
using AppLanches.Services;

namespace AppLanches
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;

        public App(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            SetMainPage();
        }

        private void SetMainPage()
        {
            var accessToken = Preferences.Get("accesstokne", string.Empty);

            if (string.IsNullOrEmpty(accessToken))
            {
                MainPage = new NavigationPage(new LoginPage(_apiService));
                return;
            }

            MainPage = new AppShell(_apiService);
        }
    }
}

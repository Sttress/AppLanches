using AppLanches.Services;

namespace AppLanches.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;

    public LoginPage(ApiService apiService)
	{
		InitializeComponent();
		_apiService = apiService;
	}

    private async void BtnSingin_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            await DisplayAlert("Erro", "Informe o email", "Cancelar");
        }
        if (string.IsNullOrEmpty(EntPassword.Text))
        {
            await DisplayAlert("Erro", "Informe a senha", "Cancelar");
        }
        var response = await _apiService.Login(EntEmail.Text, EntPassword.Text);
        if (!response.HasError)
        {
            Application.Current!.MainPage = new AppShell(_apiService);
        }
        else
        {
            await DisplayAlert("Erro", "Algo deu errado", "Cancelar");
        }
    }

    private async void TapRegister_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new InscricaoPage(_apiService));
    }
}
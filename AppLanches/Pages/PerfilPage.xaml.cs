using AppLanches.Services;

namespace AppLanches.Pages;

public partial class PerfilPage : ContentPage
{
    private readonly ApiService _apiService;

    public PerfilPage(ApiService apiService)
	{
		InitializeComponent();
		_apiService = apiService;
	}
}
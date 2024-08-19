using AppLanches.Services;
namespace AppLanches.Pages;

public partial class FavoritosPage : ContentPage
{
    private readonly ApiService _apiService;

    public FavoritosPage(ApiService apiService)
	{
		InitializeComponent();
		_apiService = apiService;
	}
}
using AppLanches.Services;

namespace AppLanches.Pages;

public partial class CarrinhoPage : ContentPage
{
    private readonly ApiService _apiService;
    
    public CarrinhoPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }
}
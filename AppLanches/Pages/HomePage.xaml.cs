using AppLanches.Models;
using AppLanches.Services;

namespace AppLanches.Pages;

public partial class HomePage : ContentPage
{
	private readonly ApiService _apiService;
	private bool _loginPageDisplayed = false;

	public HomePage(ApiService apiService)
	{
		InitializeComponent();
		_apiService = apiService;
		LblNomeUsuario.Text = "Ol�, " + Preferences.Get("usuarionome", string.Empty);
		_apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        Title = AppConfig.tituloHomePage;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await GetListCategorias();
		await GetMaisVendidos();
		await GetPopulares();
	}

	private async Task<IEnumerable<Categoria>> GetListCategorias()
	{
		try
		{
			var (categorias, errorMessage) = await _apiService.GetCategorias();

			if(errorMessage == "Unathorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Categoria>();
            }

			if(categorias == null)
			{
                await DisplayAlert("Erro", errorMessage ?? "N�o foi poss�vel obter as categorias", "OK");
                return Enumerable.Empty<Categoria>();
            }

			CvCategorias.ItemsSource = categorias;
			return categorias;

		}catch(Exception ex)
		{
			await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
			return Enumerable.Empty<Categoria>();
		}

	}

    private async Task<IEnumerable<Produto>> GetMaisVendidos()
    {
        try
        {
            var (produtos, errorMessage) = await _apiService.GetProdutos("maisvendido",string.Empty);

            if (errorMessage == "Unathorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Produto>();
            }

            if (produtos == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "N�o foi poss�vel obter os produtos", "OK");
                return Enumerable.Empty<Produto>();
            }

            CvMaisVendidos.ItemsSource = produtos;
            return produtos;

        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Produto>();
        }

    }

    private async Task<IEnumerable<Produto>> GetPopulares()
    {
        try
        {
            var (produtos, errorMessage) = await _apiService.GetProdutos("popular",string.Empty);

            if (errorMessage == "Unathorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Produto>();
            }

            if (produtos == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "N�o foi poss�vel obter os produtos", "OK");
                return Enumerable.Empty<Produto>();
            }

            CvPopulares.ItemsSource = produtos;
            return produtos;

        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Produto>();
        }

    }
    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService));
    }

    private void CvCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Categoria;

        if (currentSelection == null) return;

        Navigation.PushAsync(new ListaProdutosPage(currentSelection.Id,
                                                   currentSelection.Nome!,
                                                   _apiService));
        ((CollectionView)sender).SelectedItem = null;

    }

    private void CvMaisVendidos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Produto;

        if(currentSelection == null)return;

        Navigation.PushAsync(new ProdutoPage(_apiService, currentSelection));
        ((CollectionView)sender).SelectedItem = null;
    }

    private void CvPopulares_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Produto;

        if (currentSelection == null) return;

        Navigation.PushAsync(new ProdutoPage(_apiService, currentSelection));
        ((CollectionView)sender).SelectedItem = null;
    }
}
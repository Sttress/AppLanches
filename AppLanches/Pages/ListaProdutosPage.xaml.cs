using AppLanches.Models;
using AppLanches.Services;

namespace AppLanches.Pages;

public partial class ListaProdutosPage : ContentPage
{
	private readonly ApiService _apiService;
	private readonly int _categoriaId;
    private bool _loginPageDisplayed = false;

    public ListaProdutosPage(int categoriaId, string categoriaNome, ApiService apiService)
	{
		InitializeComponent();
		_apiService = apiService;
		_categoriaId = categoriaId;
		Title = categoriaNome ?? "Produtos";
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await GetListaProdutos(_categoriaId);
	}

	private async Task<IEnumerable<Produto>> GetListaProdutos(int categoriaId)
	{
		try
		{
			var (produtos, erroMessage) = await _apiService.GetProdutos("categoria", categoriaId.ToString());

			if(erroMessage == "Unauthorized" && !_loginPageDisplayed)
			{
				await DisplayLoginPage();
				return Enumerable.Empty<Produto>();
			}

			if(produtos is null)
			{
				await DisplayAlert("Erro", erroMessage ?? "Não foi possivel obter os produtos.", "OK");
                return Enumerable.Empty<Produto>();
            }

			CvProdutos.ItemsSource = produtos;
			return produtos;
        }
        catch(Exception ex)
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

    private async void CvProdutos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Produto;

        if (currentSelection == null) return;
		await Navigation.PushAsync(new ProdutoPage(_apiService, currentSelection));

		((CollectionView)sender).SelectedItem = null;

    }
}
using AppLanches.Models;
using AppLanches.Services;

namespace AppLanches.Pages;

public partial class ProdutoPage : ContentPage
{
	private readonly ApiService _apiService;
    private bool _loginPageDisplayed = false;
	private Produto _produto;

    public ProdutoPage
		(
			ApiService apiService,
			Produto produto
		)
	{
		InitializeComponent();
		_apiService = apiService;
		_produto = produto;
        Title = produto.Nome ?? "Produto";

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        IniciaPage(_produto);
    }

    private void IniciaPage(Produto produto)
    {
        LblProdutoPreco.Text = produto.Preco.ToString();
        LblProdutoDescricao.Text = produto.Detalhe ?? produto.Nome;
        ImagemProduto.Source = produto.CaminhoImagem;
        LblProdutoNome.Text = produto.Nome;
        AtualizaPrecoTotal();

    }

    private void ImagemBtnFavorito_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnRemove_Clicked(object sender, EventArgs e)
    {
        var resultadoQuantidade = int.Parse(LblQuantidade.Text.ToString()) - 1;
        if (resultadoQuantidade > 0)
        {
            LblQuantidade.Text = resultadoQuantidade.ToString();
            AtualizaPrecoTotal();
        }
    }

    private void AtualizaPrecoTotal()
    {
        LblPrecoTotal.Text = (decimal.Parse(LblProdutoPreco.Text.ToString()) * decimal.Parse(LblQuantidade.Text.ToString())).ToString();
    }

    private async void BtnIncluirNoCarrinho_Clicked(object sender, EventArgs e)
    {
        try
        {
            var carrinhoCompra = new CarrinhoCompra()
            {
                Quantidade = Convert.ToInt32(LblQuantidade.Text),
                ClienteId = Preferences.Get("usuarioid", 0),
                PrecoUnitario = Convert.ToDecimal(LblProdutoPreco.Text),
                ProdutoId = _produto.Id,
                ValorTotal = Convert.ToDecimal(LblPrecoTotal.Text)
            };

            var response = await _apiService.AdicionaItemNoCarrinho(carrinhoCompra);
            if (response.Data)
            {
                await DisplayAlert("Sucesso", "Item adicionado ao carrinho !", "OK");
            }
        }catch(Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }

    private void BtnAdiciona_Clicked(object sender, EventArgs e)
    {
        LblQuantidade.Text = (int.Parse(LblQuantidade.Text.ToString()) + 1).ToString();
        AtualizaPrecoTotal();
       
    }
}
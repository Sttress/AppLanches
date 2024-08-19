using AppLanches.Pages;
using AppLanches.Services;

namespace AppLanches
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;

        public AppShell(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            ConfigureShell();
            
        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService);
            var carrinhoPage = new CarrinhoPage(_apiService);
            var favoritosPage = new FavoritosPage(_apiService);
            var perfilPage = new PerfilPage(_apiService);

            Items.Add(new TabBar
            {
                Items =
                {
                    new ShellContent{ Title = "Home",Content = homePage },
                    new ShellContent{ Title = "Carrinho",Content = carrinhoPage },
                    new ShellContent{ Title = "Favoritos",Content = favoritosPage },
                    new ShellContent{ Title = "Perfil",Content = perfilPage },

                }
            });
        }
    }
}

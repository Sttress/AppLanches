﻿using AppLanches.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AppLanches.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;

        JsonSerializerOptions _serializerOptions;

        public ApiService
            (HttpClient httpClient,
            ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        #region USUARIO

        public async Task<ApiResponse<bool>> Login(string email, string password)
        {
            try
            {
                var login = new Login()
                {
                    Email = email,
                    Senha = password
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var contet = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Usuarios/Login", contet);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP:{response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP:{response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                Preferences.Set("acesstoken", result!.AccessToken);
                Preferences.Set("usuarioid", (int)result.UsuarioId);
                Preferences.Set("usuarionome", result!.UsuarioNome);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro no login:{ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> RegistrarUsuario(string nome, string email, string telefone, string password)
        {
            try
            {
                var register = new Register()
                {
                    Nome = nome,
                    Email = email,
                    Telefone = telefone,
                    Senha = password
                };

                var json = JsonSerializer.Serialize(register, _serializerOptions);
                var contet = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Usuarios/Register", contet);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP:{response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP:{response.StatusCode}"
                    };
                }


                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao registrar o usuário: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        #endregion USUARIO

        public async Task<(List<Categoria>? Categorias, string? ErrorMessage)> GetCategorias()
        {
            return await GetAsync<List<Categoria>>("api/categorias");
        }

        public async Task<(List<Produto>? Produtos, string? ErrorMessage)> GetProdutos(string tipoProduto, string categoriaId)
        {
            string endpoint = $"api/Produtos?tipoProduto={tipoProduto}&categoriaId={categoriaId}";
            return await GetAsync<List<Produto>>(endpoint);
        }

        public async Task<ApiResponse<bool>> AdicionaItemNoCarrinho(CarrinhoCompra carrinhoCompra)
        {
            try
            {
                var json = JsonSerializer.Serialize(carrinhoCompra, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/ItensCarrinhoCompra", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP:{response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP:{response.StatusCode}"
                    };
                }
                return new ApiResponse<bool> { Data = true };

            }catch(Exception ex)
            {
                _logger.LogError($"Erro ao adicionar item no carrinho:{ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }

        }

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accesstoken", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            var enderecoUrl = AppConfig.BaseUrl + uri;
            try
            {
                var result = await _httpClient.PostAsync(enderecoUrl, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao enviar requisição POST para {uri}:{ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generalErrorMessage = $"Erro na requisição:{response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (default, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Erro da requisição HTTP:{ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);

            }
            catch (JsonException ex)
            {
                string errorMessage = $"Erro da desserialização HTTP:{ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro inesperado HTTP:{ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
        }
    }
        
}

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using frontendapi_bikeshop.Models.account;
using Microsoft.AspNetCore.Components.Authorization;

namespace frontendapi_bikeshop.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _loclStorage;
        public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, ILocalStorageService loclStorage)
        {
            _loclStorage = loclStorage;
            _authStateProvider = authStateProvider;
            _httpClient = httpClient;
        }

        public async Task<LoginResult> Login(LoginModel loginModel)
        {
            var loginasJson = JsonSerializer.Serialize(loginModel);
            var response = await _httpClient.PostAsync("api/auth/login", new StringContent(loginasJson, Encoding.UTF8, "application/json"));
            var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

            if (!response.IsSuccessStatusCode)
            {
                return loginResult;
            }

            await _loclStorage.SetItemAsync("authToken", loginResult.Token);
            ((ApiAuthenticatedStateProvider)_authStateProvider).MarkUserAsAuthenticated(loginModel.Username);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

            return loginResult;
        }

        public Task Logout()
        {
            throw new System.NotImplementedException();
        }

        public Task<RegisterResult> Register(RegisterModel registerModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
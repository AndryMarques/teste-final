#nullable enable
using Newtonsoft.Json;

namespace GamePadAPI.Services
{
    /// <summary>
    /// Obtém e cacheia o App Access Token do Twitch usado pela API do IGDB.
    /// Renova automaticamente quando o token expira.
    /// </summary>
    public class IgdbTokenService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _clientId;
        private readonly string _clientSecret;

        private readonly SemaphoreSlim _lock = new(1, 1);
        private string? _cachedToken;
        private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

        public IgdbTokenService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _clientId = configuration["Igdb:ClientId"]
                ?? throw new InvalidOperationException("Configuração 'Igdb:ClientId' não encontrada.");
            _clientSecret = configuration["Igdb:ClientSecret"]
                ?? throw new InvalidOperationException("Configuração 'Igdb:ClientSecret' não encontrada.");
        }

        public string ClientId => _clientId;

        public async Task<string> GetAccessTokenAsync()
        {
            // Margem de 60s para evitar usar um token prestes a expirar
            if (_cachedToken != null && DateTimeOffset.UtcNow < _expiresAt.AddSeconds(-60))
            {
                return _cachedToken;
            }

            await _lock.WaitAsync();
            try
            {
                // Re-checa após adquirir o lock (outra thread pode ter renovado)
                if (_cachedToken != null && DateTimeOffset.UtcNow < _expiresAt.AddSeconds(-60))
                {
                    return _cachedToken;
                }

                var client = _httpClientFactory.CreateClient();
                var url = $"https://id.twitch.tv/oauth2/token?client_id={_clientId}"
                    + $"&client_secret={_clientSecret}&grant_type=client_credentials";

                var response = await client.PostAsync(url, null);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        $"Falha ao obter token do Twitch ({(int)response.StatusCode}): {body}");
                }

                var token = JsonConvert.DeserializeObject<TwitchTokenResponse>(body);
                if (token?.AccessToken == null)
                {
                    throw new InvalidOperationException($"Resposta de token inválida do Twitch: {body}");
                }

                _cachedToken = token.AccessToken;
                _expiresAt = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn);
                return _cachedToken;
            }
            finally
            {
                _lock.Release();
            }
        }

        private class TwitchTokenResponse
        {
            [JsonProperty("access_token")]
            public string? AccessToken { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("token_type")]
            public string? TokenType { get; set; }
        }
    }
}

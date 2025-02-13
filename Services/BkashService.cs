using hki_2025_registration.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace hki_2025_registration.Services
{
    public class BkashService
    {
        private readonly BkashSettings _settings;
        private readonly IMemoryCache _cache;

        public BkashService(IOptions<BkashSettings> settings, IMemoryCache cache)
        {
            _settings = settings.Value;
            _cache = cache;
        }

        public async Task<string> GetTokenAsync()
        {
            if (_cache.TryGetValue("BkashToken", out BkashTokenResponse token))
            {
                if (DateTime.UtcNow >= DateTime.Parse(token.expires_at))
                {
                    token = await RefreshTokenAsync(token.refresh_token);
                }

                return token.id_token;
            }

            var client = new RestClient($"{_settings.BaseUrl}/tokenized/checkout/token/grant/");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("username", _settings.Username);
            request.AddHeader("password", _settings.Password);

            var requestBody = new
            {
                app_key = _settings.AppKey,
                app_secret = _settings.AppSecret
            };

            request.AddJsonBody(JsonConvert.SerializeObject(requestBody));

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var tokenResponse = JsonConvert.DeserializeObject<BkashTokenResponse>(response.Content);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(25));

                tokenResponse.expires_at = DateTime.UtcNow.AddSeconds(tokenResponse.expires_in).ToString("o");
                _cache.Set("BkashToken", tokenResponse, cacheEntryOptions);

                return tokenResponse.id_token;
            }
            else
            {
                throw new Exception($"Error: {response.StatusCode} - {response.Content}");
            }
        }

        private async Task<BkashTokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var client = new RestClient($"{_settings.BaseUrl}/tokenized/checkout/token/refresh");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("username", _settings.Username);
            request.AddHeader("password", _settings.Password);

            var requestBody = new
            {
                app_key = _settings.AppKey,
                app_secret = _settings.AppSecret,
                refresh_token = refreshToken
            };

            request.AddJsonBody(JsonConvert.SerializeObject(requestBody));

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var tokenResponse = JsonConvert.DeserializeObject<BkashTokenResponse>(response.Content);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(tokenResponse.expires_in - 5));

                tokenResponse.expires_at = DateTime.UtcNow.AddSeconds(tokenResponse.expires_in).ToString("o");
                _cache.Set("BkashToken", tokenResponse, cacheEntryOptions);

                return tokenResponse;
            }
            else
            {
                throw new Exception($"Error: {response.StatusCode} - {response.Content}");
            }
        }

        public async Task<BkashCreatePaymentResponse> CreatePaymentAsync(string invoiceNumber, int amount, string payerReference)
        {
            var token = await GetTokenAsync();

            var client = new RestClient($"{_settings.BaseUrl}/tokenized/checkout/create");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("authorization", token);
            request.AddHeader("x-app-key", _settings.AppKey);

            var requestBody = new
            {
                mode = "0011",
                payerReference = payerReference,
                callbackURL = _settings.CallbackUrl,
                amount = amount,
                currency = "BDT",
                intent = "sale",
                merchantInvoiceNumber = invoiceNumber
            };

            request.AddJsonBody(JsonConvert.SerializeObject(requestBody));

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<BkashCreatePaymentResponse>(response.Content);
                if (result != null)
                {
                    return result;
                }
            }

            throw new Exception($"Error: {response.StatusCode} - {response.Content}");
        }

        public async Task<BkashExecutePaymentResponse> ExecutePaymentAsync(string paymentID)
        {
            var token = await GetTokenAsync();

            var client = new RestClient($"{_settings.BaseUrl}/tokenized/checkout/execute");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("authorization", token);
            request.AddHeader("x-app-key", _settings.AppKey);

            var requestBody = new
            {
                paymentID = paymentID
            };

            request.AddJsonBody(JsonConvert.SerializeObject(requestBody));

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<BkashExecutePaymentResponse>(response.Content);
                if (result != null)
                {
                    return result;
                }
            }

            throw new Exception($"Error: {response.StatusCode} - {response.Content}");
        }
    }
}

public class BkashExecutePaymentResponse
{
    public string statusCode { get; set; }
    public string statusMessage { get; set; }
    public string paymentID { get; set; }
    public string payerReference { get; set; }
    public string customerMsisdn { get; set; }
    public string trxID { get; set; }
    public string amount { get; set; }
    public string transactionStatus { get; set; }
    public string paymentExecuteTime { get; set; }
    public string currency { get; set; }
    public string intent { get; set; }
    public string merchantInvoiceNumber { get; set; }
}

public class BkashTokenResponse
{
    public string token_type { get; set; }
    public string id_token { get; set; }
    public int expires_in { get; set; }
    public string refresh_token { get; set; }
    public string expires_at { get; set; }
}

public class BkashCreatePaymentResponse
{
    public string paymentID { get; set; }
    public string bkashURL { get; set; }
    public string callbackURL { get; set; }
    public string successCallbackURL { get; set; }
    public string failureCallbackURL { get; set; }
    public string cancelledCallbackURL { get; set; }
    public string amount { get; set; }
    public string intent { get; set; }
    public string currency { get; set; }
    public string paymentCreateTime { get; set; }
    public string transactionStatus { get; set; }
    public string merchantInvoiceNumber { get; set; }
    public string statusCode { get; set; }
    public string statusMessage { get; set; }
}

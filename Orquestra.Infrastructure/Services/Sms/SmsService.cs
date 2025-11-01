using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Services.Email.Models;
using System.Text;
using System.Text.Json;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure.Services.Sms;

public class SmsService(IOptions<EmailSettings> options, IWebHostEnvironment env, HttpClient httpClient) : ISmsService
{
    private readonly string _apiKey = options.Value.ApiKey ?? throw new ArgumentException("Brevo API key não está configurada.");
    private readonly bool _isDevelopment = env.IsDevelopment();
    private readonly HttpClient _httpClient = httpClient;

    public async Task<string> SendSms(string to, string from, string text, bool mustThrow = false, string tag = "", string type = "transactional", string? callbackUrl = null)
    {
        if (_isDevelopment)
        {
            text = $"[DEBUG] {text}";
        }

        string? phoneNormalized = NormalizeBrazilianPhone(to);

        if (string.IsNullOrEmpty(phoneNormalized))
        {
            if (mustThrow)
            {
                throw new ArgumentException("Número de telefone inválido");
            }

            return string.Empty;
        }

        var payload = new
        {
            sender = from,
            recipient = phoneNormalized,
            content = text,
            tag,
            type, // marketing || transactional;
            callback = callbackUrl
        };

        if (_isDevelopment && SystemConsts.Brevo.DoNotSendEmailIfDev)
        {
            return string.Empty;
        }

        string jsonPayload = JsonSerializer.Serialize(payload);

        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://api.brevo.com/v3/transactionalSMS/send"),
            Headers =
            {
                { "accept", "application/json" },
                { "api-key", _apiKey }
            },
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };

        try
        {
            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (mustThrow)
                {
                    string msg = $"Erro ao enviar SMS ({(int)response.StatusCode} {response.StatusCode}): {responseBody}";
                    throw new HttpRequestException(msg);
                }

                return string.Empty;
            }

            return responseBody;
        }
        catch
        {
            if (mustThrow)
            {
                throw;
            }

            return string.Empty;
        }
    }
}
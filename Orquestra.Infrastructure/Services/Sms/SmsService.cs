using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orquestra.Domain.Consts;
using System.Net.Http.Json;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure.Services.Sms;

public class SmsService(IWebHostEnvironment env, HttpClient httpClient) : ISmsService
{
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
            recipient = to,        // addTo;
            content = text,        // setText;
            tag,                   // setTag;
            type,                  // marketing | transactional;
            callback = callbackUrl // setCallback;
        };

        if (_isDevelopment && SystemConsts.Brevo.DoNotSendEmailIfDev)
        {
            return string.Empty;
        }

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("send", payload);
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            if (mustThrow)
            {
                throw new HttpRequestException($"Erro ao enviar SMS ({response.StatusCode}): {responseBody}");
            }

            return string.Empty;
        }

        return responseBody;
    }
}
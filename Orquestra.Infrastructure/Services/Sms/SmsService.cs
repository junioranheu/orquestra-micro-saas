using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orquestra.Domain.Consts;
using System.Net.Http.Json;

namespace Orquestra.Infrastructure.Services.Sms;

public class SmsService(IWebHostEnvironment env, HttpClient httpClient) : ISmsService
{
    private readonly bool _isDevelopment = env.IsDevelopment();
    private readonly HttpClient _httpClient = httpClient;

    public async Task<string> SendSms(string to, string from, string text, string tag = "", string type = "transactional", string? callbackUrl = null)
    {
        if (_isDevelopment)
        {
            text = $"[DEBUG] {text}";
        }

        var payload = new
        {
            sender = from,
            recipient = to,        // addTo
            content = text,        // setText
            tag,                   // setTag
            type,                  // marketing | transactional
            callback = callbackUrl // setCallback
        };

        if (_isDevelopment && SystemConsts.Brevo.DoNotSendEmailIfDev)
        {
            return string.Empty;
        }

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("send", payload);
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Erro ao enviar SMS ({response.StatusCode}): {responseBody}");
        }

        return responseBody;
    }
}
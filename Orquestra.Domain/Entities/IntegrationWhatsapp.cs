using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Orquestra.Domain.Entities;

public sealed class IntegrationWhatsApp : Audit
{
    [Key]
    public Guid IntegrationWhatsAppId { get; set; }

    [MaxLength(512)]
    [Display(Description = "Mensagem enviada um dia antes")]
    public string MessageReminderBeforeSchedule { get; set; } = "Olá, {cliente}. Você tem um agendamento amanhã às {hora}. Nós, {empresa}, estamos te esperando!";

    [MaxLength(512)]
    [Display(Description = "Mensagem enviada pouco antes do horário do agendamento")]
    public string MessageBeforeScheduleAlert { get; set; } = "Olá, {cliente}. Seu agendamento em {data} às {hora} está chegando! Preparado? Nós, {empresa}, estamos te esperando!";

    [MaxLength(512)]
    [Display(Description = "Mensagem enviada quando o agendamento é confirmado")]
    public string MessageOnScheduleConfirmed { get; set; } = "Olá, {cliente}. Seu agendamento em {data} às {hora} foi confirmado! Nós, {empresa}, estamos te esperando!";

    [MaxLength(512)]
    [Display(Description = "Mensagem enviada quando o agendamento é cancelado")]
    public string MessageOnScheduleCanceled { get; set; } = "Olá, {cliente}. Seu agendamento em {data} foi cancelado. Entre em contato para reagendar. Nós, {empresa}, estamos te esperando!";

    public Guid CompanyId { get; set; }
    [JsonIgnore]
    public Company? Company { get; set; }

    public static string FormatMessage(string template, string cliente, DateTime data)
    {
        if (string.IsNullOrEmpty(template))
        {
            return string.Empty;
        }

        string normalized = template.ToLowerInvariant();
        string result = template;

        if (normalized.Contains("{cliente}"))
        {
            result = result.Replace("{cliente}", cliente, StringComparison.OrdinalIgnoreCase);
        }

        if (normalized.Contains("{data}"))
        {
            result = result.Replace("{data}", data.ToString("dd/MM/yyyy", new CultureInfo("pt-BR")), StringComparison.OrdinalIgnoreCase);
        }

        if (normalized.Contains("{hora}"))
        {
            result = result.Replace("{hora}", data.ToString("HH:mm", new CultureInfo("pt-BR")), StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }
}
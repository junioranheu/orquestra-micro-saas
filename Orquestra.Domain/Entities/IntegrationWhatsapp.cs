using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Orquestra.Domain.Entities;

public sealed class IntegrationWhatsapp : Audit
{
    [Key]
    public Guid IntegrationWhatsappId { get; set; }

    [MaxLength(512)]
    [Display(Description = "Mensagem enviada como lembrete antes do agendamento")]
    public string MessageReminderBeforeSchedule { get; set; } = "Olá, {Cliente}. Você tem um agendamento em {Data} às {Hora}. Estamos te esperando!";

    [MaxLength(512)]
    [Display(Description = "Mensagem enviada quando o agendamento é confirmado")]
    public string MessageOnScheduleConfirmed { get; set; } = "Olá, {Cliente}. Seu agendamento em {Data} às {Hora} foi confirmado!";

    [MaxLength(512)]
    [Display(Description = "Mensagem enviada quando o agendamento é cancelado")]
    public string MessageOnScheduleCanceled { get; set; } = "Olá, {Cliente}. Seu agendamento em {Data} foi cancelado. Entre em contato para reagendar.";

    [MaxLength(512)]
    [Display(Description = "Mensagem enviada pouco antes do horário do agendamento")]
    public string MessageBeforeScheduleAlert { get; set; } = "Olá, {Cliente}. Seu agendamento em {Data} às {Hora} está chegando! Preparado?";

    public Guid CompanyId { get; set; }
    [JsonIgnore]
    public Company? Company { get; set; }

    public static string FormatMessage(string template, string cliente, DateTime data)
    {
        return template.Replace("{Cliente}", cliente).Replace("{Data}", data.ToString("dd/MM/yyyy", new CultureInfo("pt-BR"))).Replace("{Hora}", data.ToString("HH:mm", new CultureInfo("pt-BR")));
    }
}
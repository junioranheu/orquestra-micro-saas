namespace Orquestra.Domain.Entities;

public sealed class EmailSettings
{
    public required string SmtpHost { get; set; } 
    public int SmtpPort { get; set; }
    public required string SenderName { get; set; }
    public required string SenderEmail { get; set; } 
    public required string Username { get; set; }
    public required string Password { get; set; }
    public bool EnableSsl { get; set; }
}
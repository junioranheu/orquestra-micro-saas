using System.Text.RegularExpressions;

namespace Orquestra.Utils.Fixtures;

public static partial class RegexPatterns
{
    [GeneratedRegex(@"^[\p{L}][\p{L}'\-\.]*(?:\s+[\p{L}][\p{L}'\-\.]*)*$", RegexOptions.IgnoreCase)]
    public static partial Regex RegexName();

    /// <summary>
    /// Expressão regular para validar números de telefone brasileiros (fixos e celulares),
    /// aceitando formatos com ou sem DDD, espaços, parênteses e hífen.
    /// Exemplos válidos:
    /// - 12987654321
    /// - 12 987654321
    /// - (12) 987654321
    /// - (12) 98765-4321
    /// - 1234567890
    /// - (12) 3456-7890
    /// </summary>
    [GeneratedRegex(@"^(?:\([1-9]{2}\)|[1-9]{2})\s?(9?[2-9]\d{3})-?\d{4}$")]
    public static partial Regex RegexPhone();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    public static partial Regex RegexEmail();

    [GeneratedRegex(@"^https?:\/\/[^\s]+$")]
    public static partial Regex RegexLogoUrl();

    [GeneratedRegex(@"^\d{8}$")]
    public static partial Regex RegexZipCode();

    [GeneratedRegex(@"^https?:\/\/[^\s]+$")]
    public static partial Regex RegexCustomUrl();

    [GeneratedRegex(@"^(?=.*[\d@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
    public static partial Regex RegexPassword();

    [GeneratedRegex("<.*?>")]
    public static partial Regex RegexRemoveHtml();

    [GeneratedRegex(@"\D")]
    public static partial Regex RegexRemoveAllButDigits();

    // Se descomentar a linha debaixo, desbuga qualquer problema nos Regex acima (wtf);
    // public static bool IsValidPhone(string phone) => RegexPhone().IsMatch(phone);
}
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Orquestra.Utils.Fixtures;

public static class Get
{
    /// <summary>
    /// Obtém o horário atual, forçando ao horário de Brasilia;
    /// </summary>
    public static DateTime GetDate()
    {
        return DateTime.UtcNow;
    }

    /// <summary>
    /// Converte um DateTime qualquer para o formato UTC, tratando automaticamente o DateTimeKind.
    /// Se o DateTime estiver em Local, será convertido usando o fuso horário local.
    /// Se estiver em Unspecified, assumirá como horário local antes de converter.
    /// </summary>
    /// <param name="date">A data a ser convertida.</param>
    /// <returns>Um DateTime representando a mesma data/hora em UTC.</returns>
    public static DateTime ConvertToUtc(DateTime date)
    {
        if (date.Kind == DateTimeKind.Utc)
        {
            return date;
        }

        if (date.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Local);
        }

        return date.ToUniversalTime();
    }

    /// <summary>
    /// Converte um DateTime qualquer para o horário de Brasília (UTC-3 ou horário de verão, quando aplicável),
    /// tratando automaticamente o DateTimeKind.
    /// Se o DateTime estiver em Local, será convertido usando o fuso horário local.
    /// Se estiver em Unspecified, assumirá como horário local antes de converter.
    /// </summary>
    /// <param name="date">A data a ser convertida.</param>
    /// <returns>Um DateTime representando a mesma data/hora no horário de Brasília.</returns>
    public static DateTime ConvertToBrasiliaTime(DateTime date)
    {
        if (date.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Local);
        }

        // Converte para UTC primeiro, caso necessário
        DateTime utcDate = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();

        TimeZoneInfo brasiliaTimeZone;

        try
        {
            // Windows
            brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"); 
        }
        catch (TimeZoneNotFoundException)
        {
            // Linux/Mac;
            brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        }

        return TimeZoneInfo.ConvertTimeFromUtc(utcDate, brasiliaTimeZone);
    }

    /// <summary>
    /// Obtém a descrição de um enum;
    /// </summary>
    public static string GetEnumDesc(Enum enumVal)
    {
        MemberInfo[] memInfo = enumVal.GetType().GetMember(enumVal.ToString());
        DescriptionAttribute? attribute = CustomAttributeExtensions.GetCustomAttribute<DescriptionAttribute>(memInfo[0]);

        return attribute!.Description;
    }

    /// <summary>
    /// Detalha em texto a data e hora atual;
    /// </summary>
    public static string GetDateDetails(DateTime? date = null, bool withHour = true)
    {
        if (date is null || date == DateTime.MinValue)
        {
            date = GetDate();
        }

        string dateDetails = withHour ? $"{date:dd/MM/yyyy} às {date:HH:mm:ss}" : $"{date:dd/MM/yyyy}";

        return dateDetails;
    }

    /// <summary>
    /// Gera uma string aleatória com base na quantidade de caracteres desejados;
    /// </summary>
    public static string GetRandomString(int charLength, bool onlyUpper)
    {
        Random random = new();
        string chars = (onlyUpper ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
        string? randomStr = new([.. Enumerable.Repeat(chars, charLength).Select(s => s[random.Next(s.Length)])]);

        return randomStr;
    }

    /// <summary>
    /// Gera um número aleatório dentro de um intervalo definido.
    /// </summary>
    /// <param name="min">Valor mínimo.</param>
    /// <param name="max">Valor máximo.</param>
    /// <returns>Um número aleatório entre o intervalo especificado.</returns>
    public static int GetRandomNumber(int min, int max)
    {
        Random random = new();
        return random.Next(min, max + 1);
    }

    /// <summary>
    /// Normaliza um nome. "JuNioR ROBerto dE soUZA" = "Junior Roberto de Souza";
    /// </summary>
    public static string NormalizeToProperName(string fullName)
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        StringBuilder result = new();

        // Lista de palavras que devem ficar em minúsculas, exceto se forem a primeira palavra;
        string[] lowercaseExceptions = ["de", "da", "do", "dos", "das", "e"];

        string[] words = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i].ToLowerInvariant();

            // Verifica se a palavra está na lista de exceções e não é a primeira palavra;
            if (i > 0 && Array.Exists(lowercaseExceptions, exception => exception == word))
            {
                result.Append(word);
            }
            else
            {
                result.Append(textInfo.ToTitleCase(word));
            }

            if (i < words.Length - 1)
            {
                result.Append(' ');
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Gera um valor booleano aleatório representando "Sim" ou "Não";
    /// </summary>
    /// <param name="hitChanceForTrue">
    /// A porcentagem de chance de retornar <c>true</c> (Sim). 
    /// O valor padrão é 50, ou seja, 50% de chance.
    /// </param>
    /// <returns><c>true</c> para "Sim" ou <c>false</c> para "Não".</returns>
    public static bool GenerateTrueOrFalse(int hitChanceForTrue = 50)
    {
        Random random = new();
        int value = random.Next(0, 100);
        bool hitTrue = value < hitChanceForTrue;

        return hitTrue;
    }

    /// <summary>
    /// Retorna a primeira parte de uma string, separada por um delimitador específico.
    /// </summary>
    /// <param name="input">A string de entrada que será dividida.</param>
    /// <param name="delimiter">O delimitador usado para separar a string. O padrão é espaço.</param>
    /// <returns>A primeira parte da string antes do delimitador. Se <paramref name="input"/> for <c>null</c> ou vazio, retorna string vazia.</returns>
    public static string GetFirstWord(string? input, char delimiter = ' ')
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        string[] parts = input.Split([delimiter], 2, StringSplitOptions.RemoveEmptyEntries);
        string firstPart = parts.Length > 0 ? parts[0] : string.Empty;

        return firstPart;
    }

    /// <summary>
    /// Gera um token seguro aleatório de 32 bytes codificado em Base64 URL-safe.
    /// Ideal para verificação de conta, redefinição de senha ou tokens temporários.
    /// Pode ser usado diretamente em URLs sem quebrar a query string.
    /// </summary>
    /// <returns>Uma string Base64 URL-safe representando o token gerado.</returns>
    public static string GenerateSafeToken32Bytes(bool urlSafe)
    {
        byte[] random = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(random);

        string token = Convert.ToBase64String(random);

        if (urlSafe)
        {
            token = token.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        return token;
    }

    /// <summary>
    /// Normaliza uma string, removendo espaços extras nas extremidades
    /// e convertendo todo o conteúdo para minúsculas.
    /// </summary>
    /// <param name="str">Texto de entrada que pode ser nulo ou vazio.</param>
    /// <returns>
    /// A string normalizada em minúsculas sem espaços nas extremidades,
    /// ou <see cref="string.Empty"/> caso o valor seja nulo ou vazio.
    /// </returns>
    public static string GetNormalizedLowerStr(string? str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return string.Empty;
        }

        return str.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Valida se o e-mail informado está em um formato válido.
    /// Retorna false se for nulo, vazio ou não corresponder ao padrão de e-mail.
    /// </summary>
    /// <param name="email">E-mail a ser validado.</param>
    /// <returns>true se o e-mail for válido; caso contrário, false.</returns>
    [SuppressMessage("Performance", "SYSLIB1045:Converter em 'GeneratedRegexAttribute'.", Justification = "<Pendente>")]
    [SuppressMessage("CodeQuality", "IDE0079:Remover a supressão desnecessária", Justification = "<Pendente>")]
    public static bool IsEmailValid(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        Regex regex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        return regex.IsMatch(email);
    }

    /// <summary>
    /// Gera um segredo aleatório para JWT no formato hexadecimal de 32 caracteres (16 bytes),
    /// que pode ser usado para configurar a chave secreta do JWT.
    /// </summary>
    /// <returns>Uma string hexadecimal de 32 caracteres representando a chave secreta.</returns>
    public static string GenerateJwtSecret()
    {
        byte[] bytes = new byte[16]; // 16 bytes = 32 chars hex;
        Random.Shared.NextBytes(bytes);
        string hex = string.Concat(bytes.Select(x => x.ToString("x2"))); // Hex string;

        return hex;
    }

    /// <summary>
    /// Converte um <see cref="Guid"/> em um identificador numérico baseado em hash.
    /// Usa SHA1 para gerar 20 bytes e pega os 8 primeiros para caber em um <see cref="long"/> (64 bits).
    /// O resultado não é reversível e pode haver colisões em cenários de altíssimo volume,
    /// mas na prática é suficientemente único para a maioria dos usos.
    /// </summary>
    /// <param name="guid">O GUID de entrada.</param>
    /// <returns>Um número positivo de até 18-19 dígitos.</returns>
    public static long GuidToNumericId(Guid guid)
    {
        byte[] hash = SHA1.HashData(guid.ToByteArray());
        long value = BitConverter.ToInt64(hash, 0);
        long output = Math.Abs(value);

        return output;
    }

    /// <summary>
    /// Calcula a diferença em minutos entre duas datas.
    /// </summary>
    /// <param name="start">Data/hora inicial.</param>
    /// <param name="end">Data/hora final.</param>
    /// <returns>
    /// A quantidade total de minutos entre <paramref name="start"/> e <paramref name="end"/>.
    /// O valor pode ser negativo caso <paramref name="end"/> seja anterior a <paramref name="start"/>.
    /// </returns>
    public static int GetDatesDiffInMinutes(DateTime start, DateTime end)
    {
        int diff = (int)(end - start).TotalMinutes;

        return diff;
    }
}
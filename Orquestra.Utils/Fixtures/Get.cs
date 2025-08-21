using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Orquestra.Utils.Fixtures;

public static class Get
{
    /// <summary>
    /// Obtém o horário atual, forçando ao horário de Brasilia;
    /// </summary>
    public static DateTime GetDate()
    {
        // TimeZoneInfo timeZone = TZConvert.GetTimeZoneInfo("E. South America Standard Time");
        // return TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);
        return DateTime.UtcNow;
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
    public static string GetDateDetails()
    {
        DateTime data = GetDate();
        return $"{data:dd/MM/yyyy} às {data:HH:mm:ss}";
    }

    /// <summary>
    /// Gera uma string aleatória com base na quantidade de caracteres desejados;
    /// </summary>
    public static string GetRandomString(int charLength, bool onlyUpper)
    {
        Random random = new();
        string chars = (onlyUpper ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
        string? randomStr = new(Enumerable.Repeat(chars, charLength).Select(s => s[random.Next(s.Length)]).ToArray());

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
        string[] lowercaseExceptions = { "de", "da", "do", "dos", "das", "e" };

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
    /// Retorna as URLs base da aplicação (API e Front-end) de acordo com o ambiente.
    /// - Em modo DEBUG, retorna os endereços locais.
    /// - Em modo RELEASE, retorna os endereços publicados.
    /// </summary>
    public static (string urlBack, string urlFront) GetUrls()
    {
#if DEBUG
        return ("http://localhost:5035/api", "http://localhost:5173");
#else
    return ("https://orquestra-cbgkgtayftdeaxh2.brazilsouth-01.azurewebsites.net/api", "https://orquestra.vercel.app");
#endif
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
}
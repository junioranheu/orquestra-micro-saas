using System.ComponentModel;
using System.Globalization;
using System.Reflection;
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
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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

        string? description = attribute?.Description;

        if (string.IsNullOrEmpty(description))
        {
            return enumVal.ToString();
        }

        return description;
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
    public static string GetRandomString(int charLength, bool onlyUpper = false, bool onlyLetters = false)
    {
        Random random = new();

        string chars;

        if (onlyLetters)
        {
            chars = onlyUpper ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ" : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        }
        else
        {
            chars = onlyUpper ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        }

        string randomStr = new([.. Enumerable.Repeat(chars, charLength).Select(s => s[random.Next(s.Length)])]);

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

    /// <summary>
    /// Retorna a lista de países em ordem alfabética.
    /// Fonte: ISO 3166 (nomes comuns dos países).
    /// </summary>
    public static List<string> GetCountries()
    {
        #region countries
        return [.. new List<string>
        {
            "Afeganistão",
            "África do Sul",
            "Albânia",
            "Alemanha",
            "Andorra",
            "Angola",
            "Antígua e Barbuda",
            "Arábia Saudita",
            "Argélia",
            "Argentina",
            "Armênia",
            "Austrália",
            "Áustria",
            "Azerbaijão",
            "Bahamas",
            "Bangladesh",
            "Barbados",
            "Bahrein",
            "Bélgica",
            "Belize",
            "Benin",
            "Bielorrússia",
            "Bolívia",
            "Bósnia e Herzegovina",
            "Botsuana",
            "Brasil",
            "Brunei",
            "Bulgária",
            "Burkina Faso",
            "Burundi",
            "Butão",
            "Cabo Verde",
            "Camarões",
            "Camboja",
            "Canadá",
            "Catar",
            "Cazaquistão",
            "Chade",
            "Chile",
            "China",
            "Chipre",
            "Colômbia",
            "Comores",
            "Congo",
            "Coreia do Norte",
            "Coreia do Sul",
            "Costa do Marfim",
            "Costa Rica",
            "Croácia",
            "Cuba",
            "Dinamarca",
            "Djibouti",
            "Dominica",
            "Egito",
            "El Salvador",
            "Emirados Árabes Unidos",
            "Equador",
            "Eritreia",
            "Eslováquia",
            "Eslovênia",
            "Espanha",
            "Estados Unidos",
            "Estônia",
            "Eswatini",
            "Etiópia",
            "Fiji",
            "Filipinas",
            "Finlândia",
            "França",
            "Gabão",
            "Gâmbia",
            "Gana",
            "Geórgia",
            "Granada",
            "Grécia",
            "Guatemala",
            "Guiana",
            "Guiné",
            "Guiné-Bissau",
            "Guiné Equatorial",
            "Haiti",
            "Holanda",
            "Honduras",
            "Hungria",
            "Iêmen",
            "Índia",
            "Indonésia",
            "Irã",
            "Iraque",
            "Irlanda",
            "Islândia",
            "Israel",
            "Itália",
            "Jamaica",
            "Japão",
            "Jordânia",
            "Kiribati",
            "Kosovo",
            "Kuwait",
            "Laos",
            "Lesoto",
            "Letônia",
            "Líbano",
            "Libéria",
            "Líbia",
            "Liechtenstein",
            "Lituânia",
            "Luxemburgo",
            "Macedônia do Norte",
            "Madagascar",
            "Malásia",
            "Malawi",
            "Maldivas",
            "Mali",
            "Malta",
            "Marrocos",
            "Maurício",
            "Mauritânia",
            "México",
            "Micronésia",
            "Moçambique",
            "Moldávia",
            "Mônaco",
            "Mongólia",
            "Montenegro",
            "Namíbia",
            "Nauru",
            "Nepal",
            "Nicarágua",
            "Níger",
            "Nigéria",
            "Noruega",
            "Nova Zelândia",
            "Omã",
            "Palau",
            "Panamá",
            "Papua-Nova Guiné",
            "Paquistão",
            "Paraguai",
            "Peru",
            "Polônia",
            "Portugal",
            "Quênia",
            "Quirguistão",
            "Reino Unido",
            "República Centro-Africana",
            "República Checa",
            "República Democrática do Congo",
            "República Dominicana",
            "Romênia",
            "Ruanda",
            "Rússia",
            "Salomão",
            "Samoa",
            "San Marino",
            "Santa Lúcia",
            "São Cristóvão e Névis",
            "São Tomé e Príncipe",
            "São Vicente e Granadinas",
            "Seicheles",
            "Senegal",
            "Serra Leoa",
            "Sérvia",
            "Singapura",
            "Síria",
            "Somália",
            "Sri Lanka",
            "Sudão",
            "Sudão do Sul",
            "Suécia",
            "Suíça",
            "Suriname",
            "Tailândia",
            "Taiwan",
            "Tajiquistão",
            "Tanzânia",
            "Timor-Leste",
            "Togo",
            "Tonga",
            "Trinidad e Tobago",
            "Tunísia",
            "Turcomenistão",
            "Turquia",
            "Tuvalu",
            "Ucrânia",
            "Uganda",
            "Uruguai",
            "Uzbequistão",
            "Vanuatu",
            "Vaticano",
            "Venezuela",
            "Vietnã",
            "Zâmbia",
            "Zimbábue"
        }.OrderBy(x => x)];
        #endregion
    }

    /// <summary>
    /// Normaliza uma string removendo acentos (diacríticos),
    /// convertendo para minúsculas e removendo espaços extras.
    /// Útil para comparações seguras ignorando capitalização e acentos.
    /// </summary>
    /// <param name="input">Texto que será normalizado.</param>
    /// <returns>Texto normalizado sem acentos, em lowercase e sem espaços extras.</returns>
    public static string NormalizeTextRemoveAccentsAndLower(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        // Normaliza para FormD (separa letras de acentos);
        string formD = input.Normalize(NormalizationForm.FormD);

        // Remove caracteres de acento;
        string withoutAccents = new(formD.Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark).ToArray());

        // Converte para lowercase e remove espaços extras
        string output = withoutAccents.ToLowerInvariant().Trim();

        return output;
    }

    /// <summary>
    /// Verifica se o tamanho de um array de bytes ultrapassa o limite permitido em megabytes.
    /// Lança uma exceção se o tamanho for excedido.
    /// </summary>
    /// <param name="input">Array de bytes a ser validado.</param>
    /// <param name="maxMegabytes">Limite máximo em MB permitido.</param>
    /// <exception cref="InvalidOperationException">Lançada quando o array excede o tamanho permitido.</exception>
    public static void ValidateMaxSizeBytes(byte[]? input, int maxMegabytes)
    {
        ArgumentNullException.ThrowIfNull(input);

        long maxBytes = maxMegabytes * 1024L * 1024L;

        if (input.Length > maxBytes)
        {
            throw new InvalidOperationException($"O arquivo excede o limite de {maxMegabytes}MBs.");
        }
    }

    /// <summary>
    /// Verifica se o tamanho de um arquivo enviado (IFormFile) ultrapassa o limite permitido em megabytes.
    /// Lança uma exceção se o tamanho for excedido.
    /// </summary>
    /// <param name="file">Arquivo a ser validado.</param>
    /// <param name="maxMegabytes">Limite máximo em MB permitido.</param>
    /// <exception cref="InvalidOperationException">Lançada quando o arquivo excede o tamanho permitido.</exception>
    public static void ValidateMaxSizeFile(IFormFile? file, int maxMegabytes)
    {
        if (file is null)
        {
            throw new ArgumentNullException(nameof(file), "Arquivo não pode ser nulo.");
        }

        long maxBytes = maxMegabytes * 1024L * 1024L;

        if (file.Length > maxBytes)
        {
            throw new InvalidOperationException($"O arquivo excede o limite de {maxMegabytes} MBs.");
        }
    }

    /// <summary>
    /// Converte um array de bytes em uma string Base64 no formato Data URI.
    /// Útil para exibir arquivos binários (como imagens) diretamente no front-end
    /// sem a necessidade de salvar em disco ou expor um endpoint de download.
    /// </summary>
    /// <param name="bytes">Array de bytes representando o conteúdo do arquivo.</param>
    /// <param name="contentType">
    /// Tipo MIME do conteúdo (exemplo: "image/png", "image/jpeg", "application/pdf").
    /// Se for <c>null</c> ou vazio, será usado <c>application/octet-stream</c>.
    /// </param>
    /// <returns>
    /// Uma string no formato <c>data:[contentType];base64,[dados]</c> que pode ser usada
    /// diretamente como valor em atributos como <c>src</c> de uma tag HTML <c>&lt;img&gt;</c>.
    /// </returns>
    public static string ConvertBytesToBase64(byte[] bytes, string? contentType)
    {
        if (bytes is null || bytes.Length == 0)
        {
            throw new ArgumentException("O array de bytes não pode ser nulo ou vazio.", nameof(bytes));
        }

        string finalContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType;
        string base64 = $"data:{finalContentType};base64,{Convert.ToBase64String(bytes)}";

        return base64;
    }
}
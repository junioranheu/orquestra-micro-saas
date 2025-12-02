using Microsoft.AspNetCore.Http;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static Orquestra.Utils.Fixtures.RegexPatterns;

namespace Orquestra.Utils.Fixtures;

public static partial class Get
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

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
        long result = value == long.MinValue ? 0 : Math.Abs(value);

        return result;
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
        string withoutAccents = new([.. formD.Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)]);

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

    /// <summary>
    /// Retorna uma lista contendo todos os valores de um enum, juntamente com seus nomes e descrições.
    /// </summary>
    /// <typeparam name="TEnum">O tipo do enum a ser processado.</typeparam>
    /// <returns>
    /// Uma lista de tuplas no formato: (Valor, Nome, Descrição).
    /// Caso o enum não tenha atributo <see cref="DescriptionAttribute"/>, a descrição será igual ao nome.
    /// </returns>
    public static List<(TEnum Value, string Name, string Description)> GetEnumListWithDescriptions<TEnum>() where TEnum : struct, Enum
    {
        return [.. Enum.GetValues<TEnum>().Cast<TEnum>().Select(value => {
                  string name = value.ToString();
                  string description = value.GetType().GetField(name)?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? name;

                  return (value, name, description);
               })];
    }

    /// <summary>
    /// Valida se um CPF (Cadastro de Pessoa Física) é válido.
    /// O método remove todos os caracteres não numéricos, verifica se o CPF possui 11 dígitos,
    /// descarta sequências com todos os números iguais (como 11111111111).
    /// e calcula os dois dígitos verificadores conforme a regra oficial do CPF.
    /// Retorna <c>true</c> se o CPF for válido ou <c>false</c> caso contrário.
    /// </summary>

    public static bool IsValidCPF(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
        {
            return false;
        }

        // Remove everything except digits;
        string digitsOnly = new([.. cpf.Where(char.IsDigit)]);

        // Must have 11 digits; 
        if (digitsOnly.Length != 11)
        {
            return false;
        }

        // Invalid if all digits are equal (e.g., 11111111111);
        if (digitsOnly.Distinct().Count() == 1)
        {
            return false;
        }

        // Calculate first check digit;
        int[] firstMultiplier = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] secondMultiplier = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        var tempCpf = digitsOnly[..9];
        int sum = tempCpf.Select((t, i) => (t - '0') * firstMultiplier[i]).Sum();
        int remainder = sum % 11;
        int firstCheckDigit = remainder < 2 ? 0 : 11 - remainder;

        tempCpf += firstCheckDigit;
        sum = tempCpf.Select((t, i) => (t - '0') * secondMultiplier[i]).Sum();
        remainder = sum % 11;
        int secondCheckDigit = remainder < 2 ? 0 : 11 - remainder;

        string checkDigits = $"{firstCheckDigit}{secondCheckDigit}";
        bool isValid = digitsOnly.EndsWith(checkDigits);

        return isValid;
    }

    /// <summary>
    /// Remove todas as tags HTML de uma string, retornando apenas o texto limpo.
    /// </summary>
    /// <param name="input">O texto que pode conter tags HTML.</param>
    /// <returns>O texto sem nenhuma tag HTML. Caso a entrada seja nula, retorna string vazia.</returns>
    /// <remarks>
    /// Esse método utiliza expressão regular para remover qualquer conteúdo entre
    /// &lt; e &gt;. Também faz trim no resultado final.
    /// 
    /// Exemplo:
    /// <code>
    /// string result = TextSanitizer.RemoveHtmlTags("&lt;b&gt;Olá&lt;/b&gt; mundo!");
    /// // Resultado: "Olá mundo!"
    /// </code>
    /// </remarks>
    public static string RemoveHtmlTags(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        string output = RegexRemoveHtml().Replace(input, string.Empty);
        return output.Trim();
    }

    /// <summary>
    /// Normaliza um JSON de mudança, ajustando os nomes dos campos "Before" e "After"
    /// para "Antes" e "Depois", e formatando o JSON com identação.
    /// </summary>
    /// <param name="json">JSON original com campos "Before" e "After".</param>
    /// <returns>JSON formatado com campos "Antes" e "Depois".</returns>
    public static string NormalizeJsonLog(string json)
    {
        using var doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        var normalized = new
        {
            Antes = root.GetProperty("Before"),
            Depois = root.GetProperty("After")
        };

        return JsonSerializer.Serialize(normalized, _jsonOptions);
    }

    /// <summary>
    /// Tenta extrair uma propriedade de um JSON e converter para o tipo desejado.
    /// </summary>
    /// <typeparam name="T">Tipo de retorno desejado (ex: Guid, int, string, bool).</typeparam>
    /// <param name="json">O conteúdo JSON em string.</param>
    /// <param name="propertyName">Nome da propriedade a ser extraída.</param>
    /// <returns>O valor convertido, ou default(T) caso a propriedade não exista ou não seja válida.</returns>
    public static T GetPropertyValueFromStringJson<T>(string? json, string propertyName)
    {
        if (string.IsNullOrEmpty(json))
        {
            return default!;
        }

        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty(propertyName, out JsonElement element))
            {
                return element.Deserialize<T>()!;
            }
        }
        catch (JsonException)
        {
            // JSON inválido, retorna default;
        }
        catch (InvalidOperationException)
        {
            // Falha na conversão, retorna default;
        }

        return default!;
    }

    /// <summary>
    /// Verifica se o código está sendo executado dentro de um ambiente de teste do xUnit.
    /// </summary>
    /// <returns>
    /// Retorna <see langword="true"/> se o assembly do xUnit runner estiver carregado no domínio atual da aplicação; 
    /// caso contrário, retorna <see langword="false"/>.
    /// </returns>
    public static bool IsRunningFromXUnit()
    {
        return AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName?.StartsWith("xunit.runner", StringComparison.OrdinalIgnoreCase) == true);
    }

    /// <summary>
    /// Normaliza um número de telefone brasileiro para o formato E.164.
    /// Exemplos:
    /// <list type="bullet">
    /// <item><description>"12982716339" → "+5512982716339"</description></item>
    /// <item><description>"982716339" → "+5512982716339"</description></item>
    /// <item><description>"+55 12 982716339" → "+5512982716339"</description></item>
    /// <item><description>"+55 (12) 98271-6339" → "+5512982716339"</description></item>
    /// </list>
    /// </summary>
    /// <param name="rawPhone">Número bruto informado pelo usuário.</param>
    /// <returns>Número normalizado no formato +55DDDNÚMERO. Retorna null se o número for inválido.</returns>
    public static string? NormalizeBrazilianPhone(string? rawPhone)
    {
        if (string.IsNullOrWhiteSpace(rawPhone))
        {
            return null;
        }

        // Remove tudo que não for dígito;
        string digits = RegexRemoveAllButDigits().Replace(rawPhone, "");

        // Remove zeros à esquerda (casos bizarros tipo 00982716339);
        digits = digits.TrimStart('0');

        // Se começar com 55 e tiver 12 ou 13 dígitos → já é formato BR completo;
        if (digits.StartsWith("55") && digits.Length is >= 12 and <= 13)
        {
            return $"+{digits}";
        }

        // Se tiver 11 dígitos → assume que já tem DDD;
        if (digits.Length == 11)
        {
            return $"+55{digits}";
        }

        // Se tiver 10 dígitos → assume que é fixo com DDD;
        if (digits.Length == 10)
        {
            return $"+55{digits}";
        }

        // Se tiver 9 dígitos (sem DDD), não dá pra normalizar;
        // ou se não couber em nenhum caso, retorna null;
        return null;
    }

    /// <summary>
    /// Carrega um arquivo de template HTML e substitui os placeholders pelos valores fornecidos.
    /// Dictionary<string, string> values = new()
    /// {
    ///    { "[UserName]", "Junior" },
    ///    { "[CompanyName]", "Orquestra" },
    ///    { "[ConfirmLink]", "orquestra.com/confirm?token=123" }
    /// };
    /// </summary>
    /// <param name="templatePath">Caminho completo do arquivo HTML do template.</param>
    /// <param name="values">Dicionário contendo os placeholders e seus respectivos valores. 
    /// Cada chave deve corresponder a um placeholder no template, por exemplo [Name].</param>
    /// <returns>Retorna uma string com o conteúdo do template já com os placeholders substituídos pelos valores.</returns>
    public static string RenderTemplate(string templateName, Dictionary<string, string> values)
    {
        string basePath = Path.Combine(AppContext.BaseDirectory, "Services", "Email", "Templates");
        string templatePath = Path.Combine(basePath, templateName);

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template não encontrado: {templatePath}");
        }

        string template = File.ReadAllText(templatePath);

        // Substitui os placeholders;
        foreach (var kv in values)
        {
            template = template.Replace(kv.Key, kv.Value);
        }

        return template;
    }

    /// <summary>
    /// Recebe um JSON no formato { "Before": {...}, "After": {...} } e retorna um JSON
    /// contendo apenas os campos que mudaram entre "Before" e "After".
    /// - Compara valor a valor e ignora campos especificados.
    /// - Retorna JSON serializado, pronto para enviar pro front.
    /// </summary>
    /// <param name="json">O JSON de entrada com os objetos "Before" e "After".</param>
    /// <returns>JSON string contendo apenas os campos alterados, no formato:
    /// {
    ///   "Campo1": { "Before": "valor antigo", "After": "valor novo" },
    ///   "Campo2": { "Before": "...", "After": "..." }
    /// }
    /// </returns>
    public static string GetChangedFieldsFromBeforeAndAfter(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return string.Empty;
        }

        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        root.TryGetProperty("Before", out JsonElement before);
        root.TryGetProperty("After", out JsonElement after);

        bool hasBefore = before.ValueKind != JsonValueKind.Undefined && before.ValueKind != JsonValueKind.Null;
        bool hasAfter = after.ValueKind != JsonValueKind.Undefined && after.ValueKind != JsonValueKind.Null;

        if (!hasBefore && !hasAfter)
        {
            return string.Empty;
        }

        HashSet<string> ignoredProps = ["LastModificationDate", "Status"];
        bool ShouldIgnore(string propName) => ignoredProps.Contains(propName) | propName.EndsWith("Id", StringComparison.OrdinalIgnoreCase) | propName.EndsWith("By", StringComparison.OrdinalIgnoreCase);

        // #1 - Fluxo de novos registros: só AFTER → retorna tudo como new record;
        if (!hasBefore && hasAfter)
        {
            var createdObj = after.EnumerateObject().
                             Where(x =>
                                !(ShouldIgnore(x.Name)) &&
                                !string.IsNullOrWhiteSpace(x.Value.ToString())
                             ).ToDictionary(
                                x => x.Name,
                                x => x.Value.ToString()
                             );

            return JsonSerializer.Serialize(createdObj, _jsonOptions);
        }

        // #2 Fluxo de modificação: comparar before vs after;
        Dictionary<string, (string Before, string After)> changed = [];

        foreach (var prop in before.EnumerateObject())
        {
            string name = prop.Name;

            if (ShouldIgnore(name))
            {
                continue;
            }

            string beforeVal = prop.Value.ToString();

            if (!after.TryGetProperty(name, out JsonElement afterEl))
            {
                continue;
            }

            string afterVal = afterEl.ToString();

            if (!string.Equals(beforeVal, afterVal, StringComparison.Ordinal))
            {
                changed[name] = (beforeVal, afterVal);
            }
        }

        var changedObj = changed.ToDictionary(
            x => x.Key,
            x => new
            {
                Antes = x.Value.Before ?? string.Empty,
                Depois = x.Value.After ?? string.Empty
            }
        );

        return JsonSerializer.Serialize(changedObj, _jsonOptions);
    }

    /// <summary>
    /// Checa se uma lista (List ou IEnumerable) é vazia;
    /// </summary>
    public static bool IsEmptyList<T>(T value)
    {
        if (value is IEnumerable enumerable)
        {
            return !enumerable.Cast<object>().Any();
        }

        return false;
    }

    /// <summary>
    /// Obtém a descrição de um enum utilizando uma string, que pode ou não existir no Enum;
    /// Há uma avaliação inicial para checar se o parâmetro passado é realmente um int;
    /// string description = GetEnumDescByIdString<LogTypeEnum>("1.0");
    /// </summary>
    public static string? GetEnumDescByIdString<T>(string? value, string key = ".") where T : Enum
    {
        if (!IsStringActuallyNumber(value))
        {
            return value;
        }

        int id = Convert.ToInt32(GetSubstringBeforeKey(input: value, key));
        string? enumValue = GetEnumDescById<T>(id);

        return enumValue;
    }

    /// <summary>
    /// Obtem toda a string anterior à palavra-chave;
    /// string input = "4.0";
    /// string result = GetSubstringBeforeKey(input, "."); // Output: "4";
    /// </summary>
    public static string? GetSubstringBeforeKey(string? input, string key)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(key))
        {
            return input;
        }

        int keyIndex = input.IndexOf(key);

        return keyIndex >= 0 ? input[..keyIndex] : input;
    }

    /// <summary>
    /// Checa se a string é um número;
    /// </summary>
    public static bool IsStringActuallyNumber(string? input)
    {
        return double.TryParse(input, out _);
    }

    /// <summary>
    /// Obtém a descrição de um enum utilizando um de seus IDs;
    /// string description = GetEnumDescById<LogTypeEnum>(id);
    /// </summary>
    public static string? GetEnumDescById<T>(int value) where T : Enum
    {
        T enumValue = (T)(object)value;
        FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo is not null)
        {
            DescriptionAttribute? descriptionAttribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();

            if (descriptionAttribute is not null)
            {
                return descriptionAttribute.Description;
            }
        }

        return enumValue.ToString();
    }
}
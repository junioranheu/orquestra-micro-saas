using Orquestra.Utils.Fixtures;

namespace Orquestra.UnitTests.Tests.Utils;

public sealed class RegexPatternsTests
{
    [Theory]
    [InlineData("junior roberto de SOUZA")]
    [InlineData("João Silva")]
    [InlineData("Maria de Souza")]
    [InlineData("Ana dos Santos Pereira")]
    public void RegexName_ShouldMatch_ValidNames(string name)
    {
        Assert.Matches(RegexPatterns.RegexName(), name);
    }

    [Theory]
    [InlineData("junior 1234")]   // contém números
    [InlineData("@@@")]           // caracteres inválidos
    [InlineData(" ")]             // vazio
    [InlineData("")]              // vazio
    [InlineData("Junior_")]       // underscore não permitido
    public void RegexName_ShouldNotMatch_InvalidNames(string name)
    {
        Assert.DoesNotMatch(RegexPatterns.RegexName(), name);
    }

    [Theory]
    [InlineData("12982716339")]        // Celular simples;
    [InlineData("12 982716339")]       // Celular com espaço;
    [InlineData("(12) 982716339")]     // Celular com DDD entre parênteses;
    [InlineData("(12) 98271-6339")]    // Celular com parênteses e hífen;
    [InlineData("(12)982716339")]      // Celular junto sem espaços;
    [InlineData("1234567890")]         // Fixo simples;
    [InlineData("12 34567890")]        // Fixo com espaço;
    [InlineData("(12) 34567890")]      // Fixo com DDD entre parênteses;
    [InlineData("(12) 3456-7890")]     // Fixo com hífen;
    [InlineData("(12)34567890")]       // Fixo sem espaço;
    public void RegexPhone_ShouldMatch_ValidPhones(string phone)
    {
        Assert.Matches(RegexPatterns.RegexPhone(), phone);
    }

    [Theory]
    [InlineData("01987654321")]        // DDD não pode começar com 0;
    [InlineData("0012345678")]         // DDD começando com 00;
    [InlineData("abcdefghijk")]        // Letras;
    [InlineData("11 12345678")]        // Começando com 1 inválido;
    [InlineData("(11 982716339")]      // Parênteses desbalanceados;
    [InlineData("(11)) 982716339")]    // Parênteses desbalanceados;
    [InlineData("11) 982716339")]      // Parênteses incorretos;
    [InlineData("(11 98271-6339")]     // Parêntese aberto sem fechar;
    [InlineData("(12) 98271-633")]     // Faltando dígito;
    [InlineData("(12) 98271-63390")]   // Dígito a mais;
    public void RegexPhone_ShouldNotMatch_InvalidPhones(string phone)
    {
        Assert.DoesNotMatch(RegexPatterns.RegexPhone(), phone);
    }

    [Theory]
    [InlineData("teste@email.com")]
    [InlineData("john.doe@empresa.org")]
    [InlineData("a@b.co")]
    public void RegexEmail_ShouldMatch_ValidEmails(string email)
    {
        Assert.Matches(RegexPatterns.RegexEmail(), email);
    }

    [Theory]
    [InlineData("testeemail.com")]
    [InlineData("john@")]
    [InlineData("@empresa.com")]
    public void RegexEmail_ShouldNotMatch_InvalidEmails(string email)
    {
        Assert.DoesNotMatch(RegexPatterns.RegexEmail(), email);
    }

    [Theory]
    [InlineData("https://site.com/logo.png")]
    [InlineData("http://cdn.meuapp.com/img.jpg")]
    public void RegexLogoUrl_ShouldMatch_ValidUrls(string url)
    {
        Assert.Matches(RegexPatterns.RegexLogoUrl(), url);
    }

    [Theory]
    [InlineData("ftp://site.com/logo.png")]
    [InlineData("site.com/logo.png")]
    public void RegexLogoUrl_ShouldNotMatch_InvalidUrls(string url)
    {
        Assert.DoesNotMatch(RegexPatterns.RegexLogoUrl(), url);
    }

    [Theory]
    [InlineData("12345678")]
    [InlineData("89012345")]
    public void RegexZipCode_ShouldMatch_ValidZip(string zip)
    {
        Assert.Matches(RegexPatterns.RegexZipCode(), zip);
    }

    [Theory]
    [InlineData("12345-678")]
    [InlineData("1234")]
    [InlineData("abcdefghi")]
    public void RegexZipCode_ShouldNotMatch_InvalidZip(string zip)
    {
        Assert.DoesNotMatch(RegexPatterns.RegexZipCode(), zip);
    }

    [Theory]
    [InlineData("https://meusite.com")]
    [InlineData("http://meuapp.dev/rota")]
    public void RegexCustomUrl_ShouldMatch_ValidCustomUrls(string url)
    {
        Assert.Matches(RegexPatterns.RegexCustomUrl(), url);
    }

    [Theory]
    [InlineData("ftp://meusite.com")]
    [InlineData("meusite.com")]
    public void RegexCustomUrl_ShouldNotMatch_InvalidCustomUrls(string url)
    {
        Assert.DoesNotMatch(RegexPatterns.RegexCustomUrl(), url);
    }

    [Theory]
    [InlineData("Senha@123")]
    [InlineData("abcde123!")]
    [InlineData("Teste1234")]
    public void RegexPassword_ShouldMatch_ValidPasswords(string pwd)
    {
        Assert.Matches(RegexPatterns.RegexPassword(), pwd);
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("abcdefghi")]
    [InlineData("senha")]
    public void RegexPassword_ShouldNotMatch_InvalidPasswords(string pwd)
    {
        Assert.DoesNotMatch(RegexPatterns.RegexPassword(), pwd);
    }

    [Theory]
    [InlineData("<b>Olá mundo</b>", "Olá mundo")]
    [InlineData("<div>Teste</div>", "Teste")]
    [InlineData("<p>Com <span>tags</span> dentro</p>", "Com tags dentro")]
    [InlineData("Sem tags", "Sem tags")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void RegexRemoveHtml_ShouldRemoveHtmlTags(string? input, string expected)
    {
        // Act;
        string result = input is null ? string.Empty : RegexPatterns.RegexRemoveHtml().Replace(input, string.Empty).Trim();

        // Assert;
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("1234567890")] // Só dígitos — não deve casar;
    [InlineData("5512982716339")] // Só dígitos — não deve casar;
    public void NonDigitRegex_ShouldNotMatch_WhenOnlyDigits(string input)
    {
        Assert.DoesNotMatch(RegexPatterns.RegexRemoveAllButDigits(), input);
    }

    [Theory]
    [InlineData("+55 (12) 98271-6339")]
    [InlineData("55 12 98271-6339")]
    [InlineData("(12)98271-6339")]
    [InlineData("12.98271.6339")]
    [InlineData("telefone: 12982716339")]
    public void NonDigitRegex_ShouldMatch_WhenContainsNonDigits(string input)
    {
        Assert.Matches(RegexPatterns.RegexRemoveAllButDigits(), input);
    }
}
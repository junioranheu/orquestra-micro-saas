using static Orquestra.Utils.Fixtures.Encrypt;

namespace Orquestra.UnitTests.Tests.Utils;

public sealed class EncryptTests
{
    [Fact]
    public void EncryptPassword_DeveRetornarHashDiferenteDaSenhaOriginal()
    {
        // Arrange;
        string password = "MinhaSenha123";

        // Act;
        string hash = EncryptPassword(password);

        // Assert;
        Assert.NotEqual(password, hash);
        Assert.False(string.IsNullOrEmpty(hash));
    }

    [Fact]
    public void CheckPassword_DeveRetornarTrue_QuandoSenhaCorreta()
    {
        // Arrange;
        string password = "SenhaSegura";
        string hash = EncryptPassword(password);

        // Act;
        bool result = CheckPassword(password, hash);

        // Assert;
        Assert.True(result);
    }

    [Fact]
    public void CheckPassword_DeveRetornarFalse_QuandoSenhaIncorreta()
    {
        // Arrange;
        string password = "SenhaOriginal";
        string wrongPassword = "OutraSenha";
        string hash = EncryptPassword(password);

        // Act;
        bool result = CheckPassword(wrongPassword, hash);

        // Assert;
        Assert.False(result);
    }

    [Fact]
    public void EncryptPassword_DeveLancarExcecao_QuandoSenhaNula()
    {
        // Act + Assert;
        Assert.Throws<ArgumentNullException>(() => EncryptPassword(null!));
    }
}
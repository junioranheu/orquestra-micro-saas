namespace Orquestra.Utils.Fixtures;

public static class Encrypt
{
    /// <summary>
    /// Criptografa a senha;
    /// </summary>
    public static string EncryptPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verifica a senha;
    /// </summary>
    public static bool CheckPassword(string password, string passwordEncrypted)
    {
        if (!BCrypt.Net.BCrypt.Verify(password, passwordEncrypted))
        {
            return false;
        }

        return true;
    }
}
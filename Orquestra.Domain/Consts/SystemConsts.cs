namespace Orquestra.Domain.Consts;

public static class SystemConsts
{
    public const string NameApi = "Orquestra.API™";
    public const string NameApp = "Orquestra";
    public const string Email = "orquestra.saas@gmail.com";
    public const string Author = "@junioranheu";
    public const string Slogan = "Orquestra: harmonia na gestão do seu negócio";
    public const string MainColor = "#f9fff6";

    public const int OneMinuteInSec = 60;
    public const int TenMinutesInSec = 600;
    public const int OneHourInSec = 3600;
    public const int OneDayInSec = 86400;
    public const int OneMonthInSec = 2629800;

    public const string PolicyRateLimiting = "_policyRateLimiting";

    public const string RefreshTokenJWTCustomHeader = "X-New-JWT";

    public const string Warn_Simple_UserNotAuth = "Usuário não autenticado.";
    public const string Warn_NeedToVerifyCompany = "Antes de prosseguir com qualquer alteração, por favor, verifique a autenticidade de sua empresa seguindo as instruções enviadas por e-mail.";

    public const bool DO_NOT_SEND_EMAIL_IF_ENV_DEV = false;

    public const string ScreenCompanyHasBeenVerified = "empresa/verificada";
    public const string ScreenCompanyUserHasBeenVerified = "usuario/verificada";
}
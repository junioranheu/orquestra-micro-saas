namespace Orquestra.Domain.Consts;

public static class SystemConsts
{
    public const string NameApi = "Orquestra.API";
    public const string NameApp = "Orquestra";
    public const string Email = "orquestra.saas@gmail.com";
    public const string Author = "@junioranheu";
    public const string Slogan = "Harmonia na gestão do seu negócio";
    public const string MainColor = "#f9fff6";

    public const int OneMinuteInSec = 60;
    public const int TenMinutesInSec = 600;
    public const int OneHourInSec = 3600;
    public const int HalfDayInSec = 43200;
    public const int OneDayInSec = 86400;
    public const int OneMonthInSec = 2629800;
    public const int OneYearInSec = 31536000;

    public const int PlanDurationInDays = 30;

    public const string PolicyRateLimiting = "_policyRateLimiting";

    public const string CookieName = "auth";
    public const string CookieRefreshedTokenName = "auth_refreshedToken";

    public const string Warn_Invalid_LinkedCompanyUser = "Você não faz parte de nenhuma empresa.";
    public const string Warn_NotAuth_Simple_User = "Usuário não autenticado.";
    public const string Warn_NeedToVerify_User = "A sua conta ainda não foi verificada ou está desativada. Verifique-a e tente novamente.";
    public const string Warn_NeedToVerify_Company = "Antes de prosseguir com qualquer alteração, por favor, verifique a autenticidade de sua empresa seguindo as instruções enviadas por e-mail.";
    public const string Warn_VerifyToken_Invalid = "Código de verificação inválido ou inexistente.";
    public const string Warn_NotFound_Company = "A empresa não foi encontrada na base de dados.";
    public const string Warn_NotFound_CompanyInvoice = "A fatura não foi encontrada na base de dados.";
    public const string Warn_NotFound_Client = "O cliente não foi encontrado na base de dados.";
    public const string Warn_NotFound_User = "O usuário não foi encontrado na base de dados.";
    public const string Warn_NotFound_Schedule = "O agendamento não foi encontrado na base de dados.";

    public const bool DO_NOT_SEND_EMAIL_IF_ENV_DEV = false;

    public const string ScreenDashboard = "dashboard";
    public const string ScreenUserHasBeenVerified = "usuario/verificado";
    public const string ScreenCompanyHasBeenVerified = "empresa/verificada";
    public const string ScreenCompanyUserHasBeenVerified = "usuario/verificado";

    public const string TemplateEmailSchedule = "EmailSchedule.html";
    public const string TemplateEmailVerifyCompany = "EmailVerifyCompany.html";
    public const string TemplateEmailCreateInvoice = "EmailCreateInvoice.html";
    public const string TemplateEmailVerifyUser = "EmailVerifyUser.html";
    public const string TemplateEmailVerifyCompanyUser = "EmailVerifyCompanyUser.html";
}
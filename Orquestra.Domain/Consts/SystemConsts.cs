namespace Orquestra.Domain.Consts;

public static class SystemConsts
{
    public static class App
    {
        public const string NameApi = "Orquestra.API";
        public const string NameApp = "Orquestra";
        public const string Email = "orquestra.saas@gmail.com";
        public const string Author = "@junioranheu";
        public const string Slogan = "Harmonia na gestão do seu negócio";
        public const string MainColor = "#f9fff6";
    }

    public static class Time
    {
        public const int OneSecond = 1;
        public const int OneMinute = 60;
        public const int TenMinutes = 600;
        public const int OneHour = 3600;
        public const int HalfDay = 43200;
        public const int OneDay = 86400;
        public const int OneMonth = 2629800;
        public const int OneYear = 31536000;

        public const int PlanDurationDaysFree = 14;
        public const int PlanDurationDays = 30;
    }

    public static class Policies
    {
        public const string RateLimiting = "_policyRateLimiting";
    }

    public static class Cookies
    {
        public const string Auth = "auth";
        public const string Refresh = "auth_refreshedToken";
    }

    public static class Screens
    {
        public const string Dashboard = "dashboard";
        public const string UserVerified = "usuario/verificado";
        public const string UserPasswordReset = "usuario/senha-redefinida";
        public const string CompanyVerified = "empresa/verificada";
        public const string CompanyUserVerified = "usuario/verificado";
    }

    public static class Templates
    {
        public const string EmailSchedule = "EmailSchedule.html";
        public const string EmailVerifyCompany = "EmailVerifyCompany.html";
        public const string EmailCreateInvoice = "EmailCreateInvoice.html";
        public const string EmailVerifyUser = "EmailVerifyUser.html";
        public const string EmailVerifyCompanyUser = "EmailVerifyCompanyUser.html";
        public const string EmailResetPassword = "EmailResetPassword.html";
    }

    public static class Warnings
    {
        public const string InvalidLinkedCompanyUser = "Você não tem permissão para prosseguir porque não faz parte dessa empresa.";
        public const string NotAuthSimpleUser = "Usuário não autenticado.";
        public const string NeedToVerifyUser = "A sua conta ainda não foi verificada ou está desativada. Verifique-a e tente novamente.";
        public const string NeedToVerifyCompany = "Antes de prosseguir com qualquer alteração, por favor, verifique a autenticidade de sua empresa seguindo as instruções enviadas por e-mail.";
        public const string VerifyTokenInvalid = "Código de verificação inválido ou inexistente.";
        public const string NotFoundData = "A informação não foi encontrada na base de dados.";
        public const string NotFoundCompany = "A empresa não foi encontrada na base de dados.";
        public const string NotFoundCompanyInvoice = "A fatura não foi encontrada na base de dados.";
        public const string NotFoundClient = "O cliente não foi encontrado na base de dados.";
        public const string NotFoundUser = "O usuário não foi encontrado na base de dados.";
        public const string NotFoundSchedule = "O agendamento não foi encontrado na base de dados.";
        public const string NotFoundVerification = "O código de verificação não foi encontrado na base de dados.";
        public const string AlreadyAuth = "Você já está autenticado no sistema, portanto não pode prosseguir com esta requisição.";
        public const string NotLinkedOrDontHaveCompany = "No momento, você não está vinculado a nenhuma empresa ou não há nenhuma definida como principal.";
    }

    public static class Brevo
    {
        public const string SmtpHost = "smtp-relay.brevo.com";
        public const int SmtpPort = 587;
        public const string SenderName = "Orquestra";
        public const string SenderEmail = App.Email;
        public const string Username = "953807001@smtp-brevo.com";
        public  const bool EnableSsl = true;
        public const bool DoNotSendEmailIfDev = false;
    }
}
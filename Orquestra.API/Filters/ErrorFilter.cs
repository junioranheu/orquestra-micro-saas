using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Orquestra.API.Filters.Base;
using Orquestra.Application.UseCases.Logs.Create;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.API.Filters;

public sealed class ErrorFilter(ILogger<ErrorFilter> logger, ICreateLog createLog) : ExceptionFilterAttribute
{
    private readonly ILogger _logger = logger;
    private readonly ICreateLog _createLog = createLog;

    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        Exception ex = context.Exception;
        string errorDetailed = $"Ocorreu um erro ao processar sua requisição. Data: {GetDateDetails()}. Caminho: {context.HttpContext.Request.Path}. {(!string.IsNullOrEmpty(ex.InnerException?.Message) ? $"Mais informações: {ex.InnerException.Message}" : $"Mais informações: {ex.Message}")}";
        string errorSimple = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException.Message : ex.Message;

        var result = new BadRequestObjectResult(new
        {
            Code = StatusCodes.Status500InternalServerError,
            Date = GetDateDetails(),
            context.HttpContext.Request.Path,
            Messages = new string[] { errorSimple },
            IsError = true
        });

        (Guid? userId, string _, UserRoleEnum[] _) = new BaseFilter().GetUserInfo(context);
        await CreateLog(context, errorSimple, errorDetailed, userId);
        Logger(ex, errorDetailed);

        context.Result = result;
        context.ExceptionHandled = true;
    }

    private async Task CreateLog(ExceptionContext context, string errorSimple, string errorDetailed, Guid? userId)
    {
        Log log = new()
        {
            LogType = LogTypeEnum.Exception,
            RequestType = context.HttpContext.Request.Method ?? string.Empty,
            Endpoint = context.HttpContext.Request.Path.ToString() ?? string.Empty,
            Parameters = string.Empty,
            Exception = errorSimple,
            Description = errorDetailed,
            Status = StatusCodes.Status500InternalServerError,
            UserId = userId is null || userId == Guid.Empty ? null : userId
        };

        await _createLog.Execute(log);
    }

    private void Logger(Exception ex, string error)
    {
        _logger.LogError(ex, "{error}", error);
    }
}
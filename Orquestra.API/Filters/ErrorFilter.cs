using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Orquestra.API.Filters.Base;
using Orquestra.Application.UseCases.Logs.Create;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using static junioranheu_utils_package.Fixtures.Get;

namespace Orquestra.API.Filters;

public sealed class ErrorFilter(ILogger<ErrorFilter> logger, ICreateLog createLog) : ExceptionFilterAttribute
{
    private readonly ILogger _logger = logger;
    private readonly ICreateLog _createLog = createLog;

    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        Exception ex = context.Exception;
        string error = $"Ocorreu um erro ao processar sua requisição. Data: {ObterDetalhesDataHora()}. Caminho: {context.HttpContext.Request.Path}. {(!string.IsNullOrEmpty(ex.InnerException?.Message) ? $"Mais informações: {ex.InnerException.Message}" : $"Mais informações: {ex.Message}")}";
        string errorSimple = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException.Message : ex.Message;

        var result = new BadRequestObjectResult(new
        {
            Code = StatusCodes.Status500InternalServerError,
            Date = ObterDetalhesDataHora(),
            context.HttpContext.Request.Path,
            Messages = new string[] { errorSimple },
            IsError = true
        });

        (Guid? userId, string _, UserRoleEnum[] _) = new BaseFilter().GetUserInfo(context);
        await CreateLog(context, error, userId);
        Logger(ex, error);

        context.Result = result;
        context.ExceptionHandled = true;
    }

    private async Task CreateLog(ExceptionContext context, string error, Guid? userId)
    {
        Log log = new()
        {
            RequestType = context.HttpContext.Request.Method ?? string.Empty,
            Endpoint = context.HttpContext.Request.Path.ToString() ?? string.Empty,
            Parameters = string.Empty,
            Description = error,
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
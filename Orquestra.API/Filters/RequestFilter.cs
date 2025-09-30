using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Orquestra.API.Filters.Base;
using Orquestra.Application.UseCases.Logs.Create;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Orquestra.API.Filters;

public sealed class RequestFilter(ICreateLog createLog) : ActionFilterAttribute
{
    private readonly ICreateLog _createLog = createLog;

    public override async Task OnActionExecutionAsync(ActionExecutingContext contextExecuting, ActionExecutionDelegate next)
    {
        if (contextExecuting.HttpContext.RequestAborted.IsCancellationRequested)
        {
            contextExecuting.Result = new StatusCodeResult(StatusCodes.Status400BadRequest);
            return;
        }

        HttpRequest request = contextExecuting.HttpContext.Request;

        if (HttpMethods.IsGet(request.Method))
        {
            await next();
            return;
        }

        ActionExecutedContext contextExecuted = await next();
        int? statusCode = (contextExecuted.Result as ObjectResult)?.StatusCode;

        (Guid? userId, string _, UserRoleEnum[] _) = new BaseFilter().GetUserInfo(contextExecuted);

        string parameters = GetRequestParameters(contextExecuting);
        string parametersNormalized = NormalizeParameters(parameters);

        await CreateLog(request, parametersNormalized, statusCode, userId);
    }

    private async Task CreateLog(HttpRequest request, string parameters, int? statusCode, Guid? userId)
    {
        Log log = new()
        {
            LogType = LogTypeEnum.Request,
            RequestType = request.Method ?? string.Empty,
            Endpoint = request.Path.ToString() ?? string.Empty,
            Parameters = parameters,
            Exception = string.Empty,
            Description = string.Empty,
            Status = statusCode is null ? StatusCodes.Status204NoContent : statusCode > 0 ? (int)statusCode : StatusCodes.Status500InternalServerError,
            UserId = userId is null || userId == Guid.Empty ? null : userId
        };

        await _createLog.Execute(log);
    }

    #region extras
    private static string GetRequestParameters(ActionExecutingContext context)
    {
        if (context.ActionArguments.Count == 0)
        {
            return string.Empty;
        }

        object? parameters = context.ActionArguments.First().Value;

        if (parameters is null)
        {
            return string.Empty;
        }

        try
        {
            return parameters is string str ? str : JsonSerializer.Serialize(parameters);
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string NormalizeParameters(string parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters))
        {
            return string.Empty;
        }

        try
        {
            JsonNode? json = JsonNode.Parse(parameters);

            if (json is null)
            {
                return parameters;
            }

            string[] keysToHide = ["Senha", "Password"];

            foreach (var key in keysToHide)
            {
                HideKeyInParameter(json, key);
            }

            return json.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = false
            });
        }
        catch
        {
            return string.Empty;
        }
    }

    private static void HideKeyInParameter(JsonNode? parameterJson, string key)
    {
        if (parameterJson is JsonObject obj)
        {
            obj.Remove(key);
        }
    }
    #endregion
}
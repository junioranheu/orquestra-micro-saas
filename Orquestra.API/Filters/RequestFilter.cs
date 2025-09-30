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
        object parameters = context.ActionArguments.FirstOrDefault().Value ?? string.Empty;

        try
        {
            string serializedParameters = !string.IsNullOrEmpty(parameters.ToString())   ? JsonSerializer.Serialize(parameters) : string.Empty;

            return serializedParameters;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    private static string NormalizeParameters(string parameters)
    {
        try
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                string[] keysToHide = ["Senha", "Password"];
                bool needsToHideKeys = keysToHide.Any(x => parameters.Contains($"\"{x}\":", StringComparison.OrdinalIgnoreCase));

                if (needsToHideKeys)
                {
                    JsonNode? parametersJson = JsonNode.Parse(parameters);

                    foreach (var key in keysToHide)
                    {
                        HideKeyInParameter(parametersJson, key);
                    }

                    string? parametersJsonStr = parametersJson?.ToJsonString(new JsonSerializerOptions
                    {
                        WriteIndented = false
                    });

                    return parametersJsonStr ?? string.Empty;
                }
            }

            return parameters;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    private static void HideKeyInParameter(JsonNode? parameterJson, string key)
    {
        if (parameterJson is JsonObject obj && obj.ContainsKey(key))
        {
            obj.Remove(key);
        }
    }
    #endregion
}
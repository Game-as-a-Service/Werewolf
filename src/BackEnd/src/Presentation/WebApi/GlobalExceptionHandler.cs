using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.WebApi;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            GameNotFoundException => HttpStatusCode.NotFound,
            GameException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError,
        };

        ctx.Response.StatusCode = (int)statusCode;
        ctx.Response.ContentType = "application/problem+json";

        await ctx.Response.WriteAsJsonAsync(new
        {
            Status = statusCode.ToString(),
            Code = (int)statusCode,
            Reason = exception.Message,
        }, cancellationToken);

        return true;
    }
}

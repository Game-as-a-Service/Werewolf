using FastEndpoints;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.WebApi.Extensions
{
    public static class ExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app, ILogger? logger = null, bool logStructuredException = false)
        {
            return app.UseExceptionHandler(errApp =>
            {
                errApp.Run(async ctx =>
                {
                    if (ctx.Features.Get<IExceptionHandlerFeature>() is IExceptionHandlerFeature exHandlerFeature)
                    {
                        var statusCode = exHandlerFeature.Error switch
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
                            Reason = exHandlerFeature.Error.Message,
                        });
                    }
                });
            });

        }
    }
}

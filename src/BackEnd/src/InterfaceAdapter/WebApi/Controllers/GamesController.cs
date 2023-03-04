using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;

namespace Wsa.Gaas.Werewolf.WebApi.Controllers
{
    public class GamesController : Controller, IPresenter<GameCreatedEvent>
    {
        [HttpPost("/games")]
        [AllowAnonymous]
        public IActionResult Create(
            CreateGameRequest request,
            [FromServices] UseCase<CreateGameRequest, GameCreatedEvent> useCase
        )
        {
            useCase.ExecuteAsync(request, this, default);

            return Ok();
        }

        public Task PresentAsync(GameCreatedEvent gameEvent, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}

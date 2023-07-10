﻿using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{
    public class CreateGameEndpoint : WebApiEndpoint<CreateGameRequest, GameCreatedEvent, GetGameResponse>
    {
        public override void Configure()
        {
            Post("/games");
            AllowAnonymous();
        }

        public override async Task<GetGameResponse> ExecuteAsync(CreateGameRequest req, CancellationToken ct)
        {
            await UseCase.ExecuteAsync(req, this, ct);

            if (ViewModel == null)
            {
                throw new Exception("View Model is null");
            }

            return ViewModel;
        }

        public override Task PresentAsync(GameCreatedEvent gameEvent, CancellationToken cancellationToken = default)
        {
            ViewModel = new GetGameResponse(gameEvent);

            return Task.CompletedTask;
        }
    }
}

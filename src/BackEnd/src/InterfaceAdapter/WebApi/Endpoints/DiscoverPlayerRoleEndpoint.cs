using FluentValidation.Results;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{
    public record DiscoverPlayerRoleResponse(string GameId, string PlayerId, string Role);


    public class DiscoverPlayerRoleEndpoint : WebApiEndpoint<DiscoverPlayerRoleRequest, SeerDiscoveredEvent, DiscoverPlayerRoleResponse>
    {
        public override void Configure()
        {
            Post("/games/{DiscordVoiceChannelId}/players/{PlayerId}/DiscoverRole");
            AllowAnonymous();
        }

        public override async Task<DiscoverPlayerRoleResponse> ExecuteAsync(DiscoverPlayerRoleRequest req, CancellationToken ct)
        {
            await UseCase.ExecuteAsync(req, this, ct);
            
            if (ViewModel == null)
            {
                throw new Exception("View Model is null");
            }

            return ViewModel;
        }

        public override Task PresentAsync(SeerDiscoveredEvent gameEvent, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}

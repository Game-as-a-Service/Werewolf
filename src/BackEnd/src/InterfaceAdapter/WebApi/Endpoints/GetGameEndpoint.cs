using System.ComponentModel;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{
    // 依照 miro 格式回傳
    [Description("""
        {
            id: "{discordVoiceChannelId}"
            players: [{
                userId: "{userId}",
                playerNumber: [1 .. 12],
                role: { "狼人" | "預言家" | "女巫" | ... },

            }],
            status: "{ Created, ...xx階段 }",
        }
        """)]
    public class GetGameResponse
    {
        public GetGameResponse(GameEvent gameEvent)
        {
            Id = gameEvent.Data.DiscordVoiceChannelId;
            Players = gameEvent.Data.Players.Select(p => new PlayerDto
            {
                UserId = p.UserId,
                Role = p.Role.Name,
                PlayerNumber = p.PlayerNumber
            }).ToList();
            Status = gameEvent.Data.Status;
        }
        public ulong Id { get; set; }
        public List<PlayerDto> Players { get; set; } = new List<PlayerDto>();
        public GameStatus Status { get; set; }
    }

    public class PlayerDto
    {
        public ulong UserId { get; set; }
        public int PlayerNumber { get; set; }
        public string Role { get; set; } = null!;
    }

    public class GetGameEndpoint : WebApiEndpoint<GetGameRequest, GetGameEvent, GetGameResponse>
    {
        public override void Configure()
        {
            Get("/games/{DiscordVoiceChannelId}");
            AllowAnonymous();
        }

        public override async Task<GetGameResponse> ExecuteAsync(GetGameRequest req, CancellationToken ct)
        {
            // 執行 UseCase.Execute();

            await UseCase.ExecuteAsync(req, this, ct);

            if (ViewModel == null)
            {
                throw new Exception("View Model is null");
            }

            return ViewModel;
        }

        public override Task PresentAsync(GetGameEvent gameEvent, CancellationToken cancellationToken = default)
        {
            // Present ViewModel
            ViewModel = new GetGameResponse(gameEvent);

            return Task.CompletedTask;
        }
    }
}

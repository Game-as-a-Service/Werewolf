using FastEndpoints;
using Was.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Common
{
    public abstract class WebApiEndpoint<TRequest, TGameEvent, TViewModel> : Endpoint<TRequest, TViewModel>
        where TRequest : notnull, new()
        where TViewModel : notnull
        where TGameEvent : GameEvent
    {
        public required UseCase<TRequest, TGameEvent> UseCase { get; set; }
    }
}

using Wsa.Gaas.Werewolf.Domain.Common;

namespace Was.Gaas.Werewolf.Application.Common
{
    public interface IEventPublisher
    {
        Task PublishAsync(GameEvent gameEvent);
    }
}

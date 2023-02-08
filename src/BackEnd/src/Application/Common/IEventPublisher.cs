using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.Application.Common
{
    public interface IEventPublisher
    {
        Task PublishAsync(GameEvent gameEvent);
    }
}

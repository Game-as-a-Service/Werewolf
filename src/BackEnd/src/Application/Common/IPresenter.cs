using Wsa.Gaas.Werewolf.Domain.Common;

namespace Was.Gaas.Werewolf.Application.Common
{
    public interface IPresenter<TGameEvent>
        where TGameEvent : GameEvent
    {
        public Task PresentAsync(TGameEvent gameEvent);
    }
}

namespace Wsa.Gaas.Werewolf.Application.Common;
public interface IPresenter<TGameEvent>
    where TGameEvent : GameEvent
{
    public Task PresentAsync(TGameEvent gameEvent, CancellationToken cancellationToken = default);
}

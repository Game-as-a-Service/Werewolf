namespace Wsa.Gaas.Werewolf.Application.Common;

public interface ITaskService
{
    Task Delay(TimeSpan timeSpan);
}
namespace Wsa.Gaas.Werewolf.Application.Common;

public class TaskService : ITaskService
{
    public Task Delay(TimeSpan timeSpan)
    {
        return Task.Delay(timeSpan);
    }
}
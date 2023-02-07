using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks.Dataflow;
using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common
{
    internal class WebApiTestServer : WebApplicationFactory<Program>
    {
        public HttpClient Client { get; init; }
        public HubConnection Connection { get; init; }
        public BufferBlock<GameEvent> EventBuffer { get; } = new BufferBlock<GameEvent>();

        public WebApiTestServer()
        {
            Client = CreateClient();
            Connection = CreateHubConnection();
        }

        public Task StartAsync()
        {
            return Connection.StartAsync();
        }

        public void ListenOn<T>()
            where T : GameEvent
        {
            Connection.On<T>("Publish", e => EventBuffer.Post(e));
        }

        public T GetRequiredService<T>()
            where T : notnull
        {
            return Server.Services.CreateScope().ServiceProvider.GetRequiredService<T>();
        }

        private HubConnection CreateHubConnection()
        {
            var uri = new UriBuilder(Client.BaseAddress!)
            {
                Path = "/events"
            }.Uri;

            return new HubConnectionBuilder()
                .WithUrl(uri, opt =>
                {
                    opt.HttpMessageHandlerFactory = _ => Server.CreateHandler();
                })
                .Build();
        }
    }
}

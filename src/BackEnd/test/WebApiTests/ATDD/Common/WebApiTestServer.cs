using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks.Dataflow;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.WebApi;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common
{
    /// <summary>
    /// Start a test server which the entry point is at `Program`
    /// </summary>
    internal class WebApiTestServer : WebApplicationFactory<Program>
    {
        // Restful API Client
        public HttpClient Client { get; init; }

        // SignalR Client
        public HubConnection Connection { get; init; }

        // Buffer for storing GameEvent received
        public BufferBlock<GameVm> EventBuffer { get; } = new();

        public WebApiTestServer()
        {
            Client = CreateClient();
            Connection = CreateHubConnection();
        }

        public Task StartAsync()
        {
            return Connection.StartAsync();
        }

        // Listen to specific GameEvent `T`
        public void ListenOn<T>()
            where T : GameEvent
        {
            // Store received GameVm to EventBuffer
            Connection.On<GameVm>(typeof(T).Name, e => EventBuffer.Post(e));
        }

        // Resolve Dependency Injection, Get Required Service
        public T GetRequiredService<T>()
            where T : notnull
        {
            return Server.Services.CreateScope().ServiceProvider.GetRequiredService<T>();
        }

        // Create SignalR Client connecting to the test server
        private HubConnection CreateHubConnection()
        {
            var uri = new UriBuilder(Client.BaseAddress!)
            {
                Path = WebApiDefaults.SignalrEndpoint,
            }.Uri;

            return new HubConnectionBuilder()
                .WithUrl(uri, opt => opt.HttpMessageHandlerFactory = _ => Server.CreateHandler())
                .Build();
        }

        public GameBuilder CreateGameBuilder()
        {
            return new GameBuilder(GetRequiredService<IRepository>());
        }
    }
}

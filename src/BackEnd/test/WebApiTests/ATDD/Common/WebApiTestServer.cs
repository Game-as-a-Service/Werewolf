using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks.Dataflow;
using Wsa.Gaas.Werewolf.Application;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.Options;
using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.WebApiTests.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;
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

    private readonly Random _random = new();

    public WebApiTestServer()
    {
        Client = CreateClient();
        Connection = CreateHubConnection();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.PostConfigure<GameSettingOptions>(opt =>
            {
                opt.PlayerRoleConfirmationTimer
                    = opt.WerewolfRoundTimer
                    = opt.SeerRoundTimer
                    = opt.WitchAntidoteRoundTimer
                    = opt.WitchPoisonRoundTimer
                    = TimeSpan.Zero;
            });
        });
    }

    public Task StartAsync()
    {
        return Connection.StartAsync();
    }

    // Listen to specific GameEvent `T`
    public void ListenOn<T>()
        where T : GameEvent
    {
        var eventName = typeof(T).Name;
        // Store received GameVm to EventBuffer
        Connection.On<GameVm>(eventName, e =>
        {
            var s = eventName;
            EventBuffer.Post(e);
        });
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

    public ulong[] RandomDistinctPlayers(int n)
    {
        var result = new HashSet<ulong>();

        if (n > 0)
        {
            while (result.Count < n)
            {
                result.Add((ulong)_random.Next());
            }
        }

        return result.ToArray();
    }

    internal void ListenAll()
    {
        var types = typeof(GameEvent).Assembly.GetTypes()
            .Where(x => x.IsAssignableTo(typeof(GameEvent)))
            ;

        foreach (var type in types)
        {
            var eventName = type.Name;

            // Store received GameVm to EventBuffer
            Connection.On<GameVm>(eventName, e =>
            {
                var s = eventName;
                EventBuffer.Post(e);
            });
        }
    }
}

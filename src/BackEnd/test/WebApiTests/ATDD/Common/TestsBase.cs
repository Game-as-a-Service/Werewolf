using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Entities;
using Wsa.Gaas.Werewolf.WebApi;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;

#pragma warning disable CS8618
//field always be init in SetUp()

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

public class TestsBase
{
    protected Action<GameVm> FakeAction;
    protected ITaskService FakeTaskService;
    protected HttpClient HttpClient;
    private HubConnection _hubConnection;
    private IRepository _repository;
    private WebApplicationFactory<Program> _webApplicationFactory;
    private protected GameBuilder GameBuilder;
    private const int NORMAL_PLAYER_COUNT = 9;

    [SetUp]
    public async Task SetUp()
    {
        FakeAction = Substitute.For<Action<GameVm>>();
        FakeTaskService = Substitute.For<ITaskService>();

        _webApplicationFactory = new WebApplicationFactory<Program>()
           .WithWebHostBuilder(builder => { builder.ConfigureTestServices(s => { ServiceCollectionServiceExtensions.AddSingleton(s, FakeTaskService); }); });

        _repository = _webApplicationFactory.Services
                                            .GetRequiredService<IRepository>();

        GameBuilder = new GameBuilder(_repository);


        HttpClient = _webApplicationFactory.CreateClient();

        _hubConnection = new HubConnectionBuilder()
                        .WithUrl(new UriBuilder(HttpClient.BaseAddress!)
                                 {
                                     Path = WebApiDefaults.SIGNALR_ENDPOINT,
                                 }.Uri,
                                 opt => opt.HttpMessageHandlerFactory = _ => _webApplicationFactory.Server.CreateHandler())
                        .Build();

        await _hubConnection.StartAsync();
    }

    protected Game? GetGame(Game game) => GetGame(game.RoomId);

    protected Game? GetGame(long roomId)
    {
        return _repository
              .FindByRoomIdAsync(roomId)
              .GetAwaiter()
              .GetResult();
    }

    protected long[] RandomDistinctPlayers(int n = NORMAL_PLAYER_COUNT)
    {
        var result = new HashSet<long>();

        if (n > 0)
        {
            while (result.Count < n)
            {
                result.Add(new Random().Next());
            }
        }

        return result.ToArray();
    }

    protected void HubListenOn<T>() => HubListenOn(typeof(T).Name);
    protected void HubListenOn(string methodName) => _hubConnection.On(methodName, FakeAction);


    protected static async Task WaitNetworkTransmission()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
    }
}
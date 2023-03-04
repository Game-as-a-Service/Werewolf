using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

public class TestsBase
{
    protected Action<GameVm> _fakeAction;
    protected ITaskService _fakeTaskService;
    protected HttpClient _httpClient;
    private HubConnection _hubConnection;
    private IRepository _repository;
    private WebApplicationFactory<Program> _webApplicationFactory;
    private protected GameBuilder _gameBuilder;
    private const int NORMAL_PLAYER_COUNT = 9;

    [SetUp]
    public async Task SetUp()
    {
        _fakeAction = Substitute.For<Action<GameVm>>();
        _fakeTaskService = Substitute.For<ITaskService>();

        _webApplicationFactory = new WebApplicationFactory<Program>()
           .WithWebHostBuilder(builder => { builder.ConfigureTestServices(s => { ServiceCollectionServiceExtensions.AddSingleton(s, _fakeTaskService); }); });

        _repository = _webApplicationFactory.Services
                                            .GetRequiredService<IRepository>();

        _gameBuilder = new GameBuilder(_repository);


        _httpClient = _webApplicationFactory.CreateClient();

        _hubConnection = new HubConnectionBuilder()
                        .WithUrl(new UriBuilder(_httpClient.BaseAddress!)
                                 {
                                     Path = WebApiDefaults.SignalrEndpoint,
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
                result.Add((long) new Random().Next());
            }
        }

        return result.ToArray();
    }

    protected void HubListenOn<T>() => HubListenOn(typeof(T).Name);
    protected void HubListenOn(string methodName) => _hubConnection.On(methodName, _fakeAction);


    protected static async Task WaitNetworkTransmission()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
    }
}
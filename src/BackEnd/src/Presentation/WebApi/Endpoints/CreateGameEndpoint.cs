namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;
public class CreateGameEndpoint : WebApiEndpoint<CreateGameRequest, CreateGameResponse>
{
    public override void Configure()
    {
        Post("/games");
        AllowAnonymous();
    }
}

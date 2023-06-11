using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Wsa.Gaas.Werewolf.DiscordBot.Options;

namespace Wsa.Gaas.Werewolf.DiscordBot.Controllers
{
    [Route("[controller]")]
    public class InviteController : Controller
    {
        [HttpGet]
        public IActionResult Index(
            [FromServices] IOptions<DiscordBotOptions> options
        )
        {
            var builder = new UriBuilder(options.Value.DiscordOAuthUrl)
            {
                Query = new QueryString()
                    .Add("client_id", options.Value.ClientId)
                    .Add("permissions", options.Value.Permissions)
                    .Add("scope", options.Value.Scope)
                    .ToString()
            };

            var url = builder.Uri.ToString();

            return Redirect(url);
        }
    }
}

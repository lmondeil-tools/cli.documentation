using McMaster.Extensions.CommandLineUtils;

using Microsoft.Extensions.Logging;

namespace cli.slndoc.Commands.Pings
{
    [Command]
    class Ping
    {
        private void OnExecute(IConsole console, ILogger<Ping> logger)
        {
            logger.LogInformation("pong CurrentDomain : {curDomFolder} \n AppContext: {appConFolder}", AppDomain.CurrentDomain.BaseDirectory, AppContext.BaseDirectory);
        }

    }
}

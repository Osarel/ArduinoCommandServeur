using EmbedIO;
using Microsoft.Extensions.Logging;
using Swan.Logging;
using System;
using System.IO;

namespace Web
{
    public class WebServeurHandler
    {
        private static Microsoft.Extensions.Logging.ILogger log = ArduinoCommand.loggerProvider.CreateLogger("Web");
        public WebServeurHandler()
        {
            Logger.UnregisterLogger<ConsoleLogger>();
            var url = "http://localhost:80/";
            // Our web server is disposable.
            using var server = CreateWebServer(url);
            // Once we've registered our modules and configured them, we call the RunAsync() method.
            server.RunAsync();
            Console.ReadKey(true);
        }

        private WebServer CreateWebServer(string url)
        {
            var server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                // First, we will configure our web server by adding Modules.
                .WithLocalSessionManager()
                .WithStaticFolder("/", Directory.GetCurrentDirectory() + "\\Data\\Website", true);
            // Listen for state changes.
            server.StateChanged += (s, e) => { log.LogDebug("Etat du serveur web : " + e.NewState); };

            return server;
        }

    }
}
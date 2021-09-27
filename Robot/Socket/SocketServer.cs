using System;
using System.Collections.Generic;
using Fleck;

namespace Robot.Serveur 
{
    enum SocketType
    {
        AUTHENTIFICATION,
        AUTHENTIFICATIONREQUEST,
        PORTLISTE,
        SYSTEMSTATUS,
        START,
        STOP,
        STOPMOVE,
        RESTART,
        CHAT,

        CREATE,
        UPDATE,
        GETLIST,
        GET,
        REMOVE,
        ELEMENTSINFO,

        SERVOCONTROL,
        LEDCONTROL,
        STARTANIMATION,
        IACHATUPDATE,
        IACHATGET,
        IACHATEXECUTE,
        ERROR,
    }



    public class SocketServer
    {
        private static readonly string authentificationPremadeRequest = new SocketReply(SocketType.AUTHENTIFICATIONREQUEST, false).Build();
        private static readonly string errorPremade = new SocketReply(SocketType.ERROR, true).AddErrorMessage("Erreur dans la formulation de la demande").Build();
        public readonly WebSocketServer server;
        public IDictionary<IWebSocketConnection, bool> connected = new Dictionary<IWebSocketConnection, bool>();

        public SocketServer(int port)
        {
            server = new WebSocketServer("ws://0.0.0.0:" + port);
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Connexion ouverte avec " + socket.ConnectionInfo.ClientIpAddress);
                    connected[socket] = false;
                    _ = socket.Send(authentificationPremadeRequest);
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine("Close connexion with web user!");
                    connected.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    _ = DecodeMessage(socket, message);
                };
            });


        }

        private bool DecodeMessage(IWebSocketConnection client, string message)
        {
            //déconnexion de l'utilisateur si il ne veux plus être connecter
            if (message == "!")
            {
                client.Close();
                return false;
            }
            SocketReader reader = new SocketReader(client, message);
            if (!reader.Validate())
            {
                //Envoie de l'erreur de validation du formulaire
                client.Send(errorPremade);
                return false;
            }
            return reader.Executor.Execute();
        }
    }
}

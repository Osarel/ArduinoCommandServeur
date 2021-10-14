using System;
using System.Threading;
using Robot.Serveur;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Onova;
using Onova.Services;
using Robot.Event;
using Robot.Event.Args;
namespace Robot
{

    class ArduinoCommand
    {
        public static RobotMain robot;
        public static SocketServer server;
        public static GlobalEvent eventG;
        public static bool demande_arret = false;
        public static bool demande_restart = false;
        private static UpdateService _updateService = new UpdateService();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            //GESTION DES EVENTS
            eventG = new GlobalEvent();
            eventG.FireRobotPreStartEvent();

            StartProg(args);
            // On empeche la fermeture du programme
            while (!demande_arret && !demande_restart)
            {
            }

            StopProg();


        }


        public static async void Update()
        {
            try
            {
                Assembly a = Assembly.GetExecutingAssembly();
                Console.WriteLine("Version actuelle : {0}" , a.GetName().Version);
                Console.WriteLine("Recherche d'une mise à jour...");
                // Recherche d'une mise à jour
                var updateVersion = await _updateService.CheckForUpdatesAsync();
                if (updateVersion == null)
                    return;

                Console.WriteLine("Une mise à jour est disponible !");
                // Preparation de la mise à jour
                Console.WriteLine($"Telechargement de la mise à jour v{updateVersion}...");
                await _updateService.PrepareUpdateAsync(updateVersion);

                // Redemarrage du programme -> redemarrage géré par le service de mise à jour ...
                Console.WriteLine("Redemarrage pour installation");
                _updateService.FinalizeUpdate(true);

                //Arret du programme
                demande_arret = true;
                StopProg();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                // Failure to update shouldn't crash the application
                Console.WriteLine("Echec de la mise à jour");
            }
        }

        public static void StartProg(string[] args)
        {
            //Chargement des fichiers du robot
            Load();
            Update();
            Speaker.say("Démarrage du robot en cours..");
            Console.WriteLine("Chargement du fichier fini");
            //Lancement du serveur Websocketg
            Thread robotThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                bool started = false;
                while (!started)
                {
                    try
                    {
                        server = new SocketServer(args.Length > 0 ? int.Parse(args[0]) : 8800);
                        started = true;
                    }
                    catch
                    {
                        Console.WriteLine("Echec du démarrage port ouvert .. nouvelle essais dans 5 secondes");
                        Thread.Sleep(5000);
                    }
                }
            });
            robotThread.Start();

            //Demarrage du serveur WEB
            Thread webThread = new  Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                new Web.WebServeurHandler();
            });
            webThread.Start();
                if (robot.Option.autoStart)
            {
                robot.StartRobot();
            } else
            {
                Console.WriteLine("Attente démarrage du robot");
            }

            //Déclanchement de l'event start
            eventG.FireRobotStartedEvent();
        }
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (!demande_arret && !demande_restart)
            {
                Console.WriteLine("Forcer l'extinction");
                StopProg();
            }
        }

        public static void StopProg()
        {
            //FIRE EVENT STOP
            eventG.FireRobotStopping();
            //extinction du robot
            robot.StopRobot();
            // Stop the server
            Console.Write("Arret de la liaison serveur / web...");
            server.server.Dispose();
            Console.WriteLine("liaison arrêté !");
            //Sauvegarde des fichiers du robot
            Console.WriteLine("Sauvegarde des fichiers du robot");
            Save();
            Console.WriteLine("Sauvegarde fini");
            if (demande_restart)
            {
                var fileName = Assembly.GetExecutingAssembly().Location;
                fileName = fileName.Remove(fileName.Length - 4);
                fileName = fileName + ".exe";
                Console.Write("Redemarrage en cours !");
                Process.Start(fileName);
            }
            Environment.Exit(0);
        }
        public static void Save()
        {

        }
         
        public static void Load()
        {
            Directory.CreateDirectory("Elements");
            Directory.CreateDirectory("Animations");
            Directory.CreateDirectory("Arduinos");
            Directory.CreateDirectory("cache/vocal/");
            robot = new RobotMain();
            robot.LoadDataFromConfig();
            Speaker.LoadVocalCache();
        }
    }


}
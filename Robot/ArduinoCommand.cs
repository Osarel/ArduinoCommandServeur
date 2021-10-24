using Microsoft.Extensions.Logging;
using Robot;
using Robot.Event;
using Robot.Serveur;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

class ArduinoCommand
{
    public static RobotMain robot;
    public static SocketServer server;
    public static GlobalEvent eventG;
    public static ConsoleLoggerProvider loggerProvider;
    public static ILogger log;
    public static bool demande_arret = false;
    public static bool demande_restart = false;
    public static bool shutdown = false;
    private static readonly UpdateService _updateService = new UpdateService();

    static void Main(string[] args)
    {
        //Définition du style console
        loggerProvider = new ConsoleLoggerProvider();
        log = loggerProvider.CreateLogger("Général");
        WELCOME();
        //Event de fermeture de la console
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

        //GESTION DES EVENTS
        eventG = new GlobalEvent();
        eventG.FireRobotPreStartEvent();

        StartProg(args);
        // On empeche la fermeture du programme
        while (!demande_arret && !demande_restart && !shutdown)
        {
        }

        StopProg();
    }


    public static void WELCOME()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Démarrage du programme...\n");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("          ===========================================\n");
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine("               _       _             \n" +
                          "              /\\           | |     (_)            \n" +
                          "             /  \\   _ __ __| |_   _ _ _ __   ___  \n" +
                          "            / /\\ \\ | '__/ _` | | | | | '_ \\ / _ \\ \n" +
                          "           / ____ \\| | | (_| | |_| | | | | | (_) |\n" +
                          "          /_/    \\_\\_|  \\__,_|\\__,_|_|_| |_|\\___/ \n");
        Console.WriteLine("            _____                                          _ \n" +
                          "           / ____|                                        | |\n" +
                          "          | |     ___  _ __ ___  _ __ ___   __ _ _ __   __| |\n" +
                          "          | |    / _ \\| '_ ` _ \\| '_ ` _ \\ / _` | '_ \\ / _` |\n" +
                          "          | |___| (_) | | | | | | | | | | | (_| | | | | (_| |\n" +
                          "           \\_____\\___/|_| |_| |_|_| |_| |_|\\__,_|_| |_|\\__,_|\n");

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("          ===========================================\n");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("All right reserved © craphael.fr / MIT Licence\n\n");

        Console.ForegroundColor = ConsoleColor.Gray;

    }

    public static async void Update(bool Force)
    {
        try
        {
            Assembly a = Assembly.GetExecutingAssembly();
            log.LogInformation("Version actuelle : {0}", a.GetName().Version);
            log.LogInformation("Recherche d'une mise à jour...");
            // Recherche d'une mise à jour
            var updateVersion = await _updateService.CheckForUpdatesAsync();
            if (updateVersion == null)
                return;

            log.LogWarning("Une mise à jour est disponible !");
            // Preparation de la mise à jour
            if (!robot.Options.autoUpdate && !Force)
            {
                return;
            }
            log.LogWarning($"Telechargement de la mise à jour v{updateVersion}...");
            await _updateService.PrepareUpdateAsync(updateVersion);

            // Redemarrage du programme -> redemarrage géré par le service de mise à jour ...
            log.LogWarning("Téléchargement terminer.");
            log.LogWarning("Redemarrage pour installation");
            _updateService.FinalizeUpdate(true);

            //Arret du programme
            demande_arret = true;
            StopProg();
        }
        catch (Exception e)
        {
            // Echec de la mise à jour
            log.LogError("Echec de la mise à jour");
            log.LogError(e.Message);
            log.LogTrace(e.StackTrace);
        }
    }

    public static void StartProg(string[] args)
    {
        log.LogInformation("Chargement des fichiers robots.");
        //Chargement des fichiers du robot
        Load();
        log.LogInformation("Chargement des fichiers fini.");
        Update(false);
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
                    log.LogError("Echec du démarrage du serveur.. nouvelle essais dans 5 secondes");
                }
            }
        });
        robotThread.Start();

        if (robot.Options.autoStart)
        {
            robot.StartRobot();
        }
        else
        {
            log.LogInformation("En attente du démarrage robot");
        }

        //Déclanchement de l'event start
        eventG.FireRobotStartedEvent();
    }

    static void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        if (!demande_arret && !demande_restart)
        {
            log.LogWarning("Ordre d'extinction reçu....");
            StopProg();
        }
    }

    public static void StopProg()
    {
        log.LogWarning("Arrêt du programme..");
        //FIRE EVENT STOP
        eventG.FireRobotStopping();
        //extinction du robot
        robot.StopRobot();
        // Stop the server
        log.LogInformation("Arret du serveur websocket...");
        server.server.Dispose();
        log.LogInformation("Serveur websocket arrêté !");
        //Sauvegarde des fichiers du robot
        log.LogInformation("Sauvegarde des fichiers du robot");
        Save();
        log.LogInformation("Sauvegarde des fichiers du robot terminer");
        if (demande_restart)
        {
            var fileName = Assembly.GetExecutingAssembly().Location;
            fileName = fileName.Remove(fileName.Length - 4);
            fileName += ".exe";
            log.LogError("Redemarrage en cours !");
            Process.Start(fileName);
        }
        if (shutdown)
        {
            Process.Start("shutdown", "/s /t 0");
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


    public static bool SystemAction(string action)
    {
        switch (action)
        {
            case "STOP_MOVE":
                robot.StopMoveRobot();
                break;
            case "START_ROBOT":
                robot.StartRobot();
                break;
            case "STOP_ROBOT":
                demande_arret = true;
                break;
            case "RESTART_ROBOT":
                demande_restart = true;
                break;
            case "STOP_COMPUTER":
                shutdown = true;
                break;
            case "FORCE_UPDATE":
                Update(true);
                break;
            default:
                return false;
        }
        return true;
    }
}

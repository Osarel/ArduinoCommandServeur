﻿using System;
using System.Threading;
using Robot.Serveur;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace Robot
{

    class ArduinoCommand
    {
        public static RobotMain robot;
        public static SocketServer server;
        static string version = "1_0_4";
        public static bool demande_arret = false;
        public static bool demande_restart = false;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            StartProg(args);
            // Perform text input
            while (!demande_arret && !demande_restart)
            {
            }
            StopProg();


        }

        public static void StartProg(string[] args)
        {
            //Chargement des fichiers du robot
            Load();
            Speaker.say("Démarrage du robot en cours..");
            Console.WriteLine("Chargement du fichier fini");
            //Lancement du serveur Websocketg
            Thread thread = new Thread(() =>
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
            thread.Start();
            if (robot.Option.autoStart)
            {
                robot.StartRobot();
            } else
            {
                Console.WriteLine("Attente démarrage du robot");
            }
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
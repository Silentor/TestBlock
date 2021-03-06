﻿using System;
using System.Threading;
using NLog;
using Silentor.TB.Common.Network;

namespace Silentor.TB.Server.Console
{
    class Program
    {
        private const string LoggerAddr = "192.168.0.100";

        private const int LoggerPort = 9998;

        private static Logger Log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException_Handler;

            System.Console.WriteLine("Starting server...");

            var config = new Bootloader.Config()
            {
                LoggerPort = LoggerPort,
                LoggerAddr = LoggerAddr,
                Port = Settings.Port,
                SecurityPort = Settings.SecurityPort,
            };

            var bootloader = new Bootloader(config);

            bootloader.Start();

            System.Console.WriteLine("...server started");

            while (true)
            {
                if (System.Console.KeyAvailable)
                    if (System.Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        bootloader.Stop();
                        Thread.Sleep(1000);
                    }
                    else if (System.Console.ReadKey().Key == ConsoleKey.P)
                        if (bootloader.IsPaused)
                            bootloader.Resume();
                        else
                            bootloader.Pause();
                Thread.Sleep(1000);
            }
        }

        private static void UnhandledException_Handler(object sender, UnhandledExceptionEventArgs e)
        {
            System.Console.WriteLine(e);
            Log.Fatal("Unhandled exception {0}", e);
        }
        
    }


}

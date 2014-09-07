using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Code;
using NLog;
using NLog.Config;
using NLog.Targets;
using Silentor.FBRLogger.NLogTarget;
using Silentor.TB.Client.Config;
using Silentor.TB.Client.Tools;
using UnityEngine;

namespace Silentor.TB.Client.Debug
{
    /// <summary>
    /// Logging initialization, instantiate at entry point
    /// </summary>
    public class Logging : LoggingBase
    {
        private static Logger Console;

        public Logging(ISystemConfig config) : base(config)
        {
            Log.Debug("Application.persistentDataPath = " + Application.persistentDataPath);
            Log.Debug("Application.dataPath = " + Application.dataPath);
            Log.Debug("Application.temporaryCachePath = " + Application.temporaryCachePath);
            Log.Debug("Application.systemLanguage = " + Application.systemLanguage);
            Log.Debug("Application.platform = " + Application.platform);
            Log.Debug("Application.unityVersion = " + Application.unityVersion);

            Log.Debug("SystemInfo.operatingSystem = " + SystemInfo.operatingSystem);
            Log.Debug("SystemInfo.processorCount = " + SystemInfo.processorCount);
            Log.Debug("SystemInfo.deviceModel = " + SystemInfo.deviceModel);
            Log.Debug("SystemInfo.deviceUniqueIdentifier = " + SystemInfo.deviceUniqueIdentifier);
            Log.Debug("SystemInfo.systemMemorySize = " + SystemInfo.systemMemorySize);

            //Setup Unity console messages interception
            Console = LogManager.GetLogger("Unity.Console");
            Application.RegisterLogCallback(UnityLogHandler);
            Application.RegisterLogCallbackThreaded(UnityLogHandler);

            UnityEngine.Debug.Log("Logging initialized");
        }

        private static void UnityLogHandler(string condition, string stackTrace, LogType type)
        {
            var logLevel = LogLevel.Info;
            switch (type)
            {
                case LogType.Assert:
                    logLevel = LogLevel.Fatal;
                    break;
                case LogType.Error:
                    logLevel = LogLevel.Error;
                    break;
                case LogType.Exception:
                    logLevel = LogLevel.Error;
                    break;
                case LogType.Log:
                    logLevel = LogLevel.Info;
                    break;
                case LogType.Warning:
                    logLevel = LogLevel.Warn;
                    break;
            }

            //var logEntry = new LogEventInfo(logLevel, Log.Name, condition) { };

            Console.Log(logLevel, "[{0}], message \"{1}\", stack:\n{2}", type.ToString(), condition, stackTrace);
        }
    }
}

using System;
using System.Collections;
using NLog;
using NLog.Config;
using NLog.Targets;
using Silentor.FBRLogger;
using Silentor.FBRLogger.NLogTarget;
using UnityEngine;

namespace Assets.Code.Benchmarks
{
    /// <summary>
    /// UnityEngine.Debug.Log vs NLog udp logging vs FBR.NLogTarget
    /// </summary>
    public class LogSystemBenchmark : MonoBehaviour 
    {
        private LogMessageSender _sender;

        private string LogViewerAddr = "192.168.0.100";
        private string NLogViewerAddr = "udp://192.168.0.100:9997";
        private int FBRPort = 9998;

        // Use this for initialization
        void Start ()
        {
            _sender = new LogMessageSender(LogViewerAddr, 9998);
            InitNLog();

            StartCoroutine(Bench());
        }

        private void InitNLog()
        {
            //Avoid xml configuration to prevent System.Xml overhead
            var logConfig = new LoggingConfiguration();

            var remoteTargetNLog = new NLogViewerTarget { Address = NLogViewerAddr };
            logConfig.AddTarget("remoteNLog", remoteTargetNLog);
            logConfig.LoggingRules.Add(new LoggingRule("NLog", LogLevel.Trace, remoteTargetNLog));

            var remoteTargetFBR = new FBRTarget { Host = LogViewerAddr, Port = FBRPort };
            logConfig.AddTarget("remoteFBR", remoteTargetFBR);
            logConfig.LoggingRules.Add(new LoggingRule("FBR", LogLevel.Trace, remoteTargetFBR));

            LogManager.Configuration = logConfig;
        }

        IEnumerator Bench()
        {
            var nlogLogger = LogManager.GetLogger("NLog");
            var fbrLogger = LogManager.GetLogger("FBR");
            var e = new MissingReferenceException("Test");


            while (true)
            {
                //Unity3d logging
                UnityEngine.Debug.Log("My message unity3d", this);
            
                yield return null;

                //Raw fbr logger
                _sender.Send(new LogMessage("RawFBR", "My message no stack no exception", LogMessage.LogLevel.Log, false));

                yield return null;

                //Raw fbr logger
                _sender.Send(new LogMessage("RawFBR", "My message with stack no exception", LogMessage.LogLevel.Log));

                yield return null;

                //Raw fbr logger
                try
                {
                    throw e;
                }
                catch (Exception ex)
                {
                    _sender.Send(new LogMessage("RawFBR", "My message with stack with exception", LogMessage.LogLevel.Log, true, ex));
                }

                yield return null;
            
                //FBR to NLog simple message
                fbrLogger.Info("My message to FBR-NLog");

                yield return null;

                //FBR to NLog message with exception
                try
                {
                    throw e;
                }
                catch (Exception ex)
                {
                    fbrLogger.Trace("My message FBR-NLog with exception", ex);
                }

                yield return null;

                nlogLogger.Info("My message NLog");

                yield return null;

                try
                {
                    throw e;
                }
                catch (Exception ex)
                {
                    nlogLogger.Trace("My message NLog with exception", ex);
                }

                yield return null;
                yield return null;
                yield return null;
                yield return null;
            }
        }

    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Silentor.TB.Client.Config;
using Silentor.TB.Client.Network;
using Silentor.TB.Client.Tools;
using Silentor.TB.Client.Tools;
using Silentor.TB.Common.Network.Messages;
using UnityEngine;
using Random = System.Random;

namespace Silentor.TB.Client.Console
{
    /// <summary>
    /// Client with simplest autonomus behaviour
    /// </summary>
    public class BotClient
    {
        public BotClient(ISystemConfig sysConfig, IApplicationEvents appEvents, string name)
        {
            Log = LogManager.GetLogger("Silentor.TB.Client.BotClient." + name);

            _name = name;
            _server = new RemoteServer(sysConfig, appEvents);

            _server.Connected += ServerOnConnected;
            _server.Logined += ServerOnLogined;
            _server.ChunkReceived += ServerOnChunkReceived;
            _server.PositionUpdated += ServerOnPositionUpdated;

            _workload = Task.Run(() => Workload());
        }

        private async Task Workload()
        {
            var rnd = new Random();

            while (true)
            {
                await Task.Delay(rnd.Next(1000, 2000));

                //Move
                Vector3 walkVector;
                if (Vector3.Distance(_startPosition, _myPosition) > 10)
                    walkVector = (_startPosition - _myPosition).normalized;
                else
                    walkVector = new Vector3(1 - 2*(float) rnd.NextDouble(), 0, 1 - 2*(float) rnd.NextDouble());

                _server.MovePlayer(walkVector, Vector2.zero, rnd.NextDouble() > 0.5);
                await Task.Delay(rnd.Next(1000, 2000));
                _server.MovePlayer(Vector3.zero, Vector2.zero);
            }
        }

        private void ServerOnPositionUpdated(EntityUpdate entityUpdate)
        {
            if (!entityUpdate.IsRemoved)
            {
                Log.Info("Position of {0} updated to {1}", entityUpdate.Id, entityUpdate.Position);
                if (entityUpdate.Id == _myId)
                    _myPosition = entityUpdate.Position.ToVector();
            }
            else
                Log.Info("Entity {0} removed", entityUpdate.Id);
        }

        private void ServerOnChunkReceived(ChunkContents chunkContents)
        {
            Log.Info("Received chunk {0}", chunkContents);
        }

        private void ServerOnLogined(LoginResponce loginResponce)
        {
            Log.Info("Logined to game with id {0} at {1}", loginResponce.Id, loginResponce.Position);
            _startPosition = loginResponce.Position.ToVector();
            _myId = loginResponce.Id;
        }

        private void ServerOnConnected()
        {
            Log.Info("Connected to server");
            _server.ServerConnection.AddPlayer(_name);
        }

        private readonly Logger Log;
        private readonly RemoteServer _server;
        private Task _workload;
        private readonly string _name;
        private Vector3 _startPosition;
        private Vector3 _myPosition;
        private int _myId;
    }
}

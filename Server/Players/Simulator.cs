using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;
using Wob.Server.Maps;
using Wob.Server.Network;
using Wob.Server.Tools;

namespace Wob.Server.Players
{
    /// <summary>
    /// Combines player, map and all its environment, responsible for Player <-> Hero relations
    /// </summary>
    public class Simulator
    {
        public const int SimulationWindowRadius = 6;

        public Map Map { get; private set; }

        public PlayerController Controller { get; private set; }

        public Player Player { get; private set; }

        public readonly Session Session;

        public Simulator(Player player, Map map, PlayerController controller, Session session)
        {
            Map = map;
            Controller = controller;
            Player = player;
            Session = session;

            _lastChunkPosition = Chunk.ToChunkPosition(Player.Position.ToMapPosition());

            Log = LogManager.GetLogger("Wob.Server.Players.Simulator" + player.Id);
            Log.Info("Created player simulator {0}", player.Id);
        }

        public void ProcessAction(Message message)
        {
            switch (message.Header)
            {
                case Headers.PlayerMovement:
                {
                    var data = (PlayerMovement) message;
                    Player.SetAcceleration(data.Movement.ToVector());
                    Player.SetRotation(data.Rotation.ToVector());
                    if (data.Jump)
                        Player.Jump();
                }
                    break;
            }
        }

        public void SimulatePlayer()
        {
            Player.Simulate();
            UpdateMap();
            
        }

        //Update Map according to changes in player position
        public void UpdateMap()
        {
            //Translate and update map
            var chunkPosition = Chunk.ToChunkPosition(Player.Position.ToMapPosition());
            if (chunkPosition != _lastChunkPosition)
            {
                Map.Slide(chunkPosition - _lastChunkPosition);
                _lastChunkPosition = chunkPosition;
            }
        }

        public void Dispose()
        {
            Session.Close();
            Log.Info("Destroyed player simulator {0}", Player.Id);
        }

        private Vector2i _lastChunkPosition;

        private readonly Logger Log;

        public override string ToString()
        {
            return String.Format("Simulator for {0} from {1}", Player, Session);
        }
    }
}

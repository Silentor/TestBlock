using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using NLog;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Server.Maps;
using Silentor.TB.Server.Network;
using Silentor.TB.Server.Players;
using Silentor.TB.Server.Tools;
using Timer = Silentor.TB.Server.Time.Timer;

namespace Silentor.TB.Server
{
    public class Engine
    {
        /// <summary>
        /// Dispatch client messages, makes client management
        /// </summary>
        /// <param name="server"></param>
        /// <param name="world"></param>
        /// <param name="timer"></param>
        public Engine(Network.Server server, World world, Timer timer)
        {
            Log.Trace("Start engine");
            
            _server = server;
            _world = world;
            _timer = timer;

            //Link engine message processing to server
            _dispatch = new ActionBlock<IncomingEnvelop>(im => DispatchMessages(im));
            server.EngineMessageReceived.LinkTo(_dispatch);
        }

        private void DispatchMessages(IncomingEnvelop envelop)
        {
            Log.Trace("Dispatching message {0}", envelop.Message.Header);

            try
            {
                switch (envelop.Message.Header)
                {
                    case Headers.Login:
                        ProcessLogin((LoginData) envelop.Message, envelop.Connection);
                        break;

                    case Headers.Disconnect:
                        ProcessDisconnect((Disconnect) envelop.Message, envelop.Hero);
                        break;

                    default:
                        Log.Warn("Undefined message {0} received", envelop.Message.Header);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in message dispatcher", ex);
            }
            finally
            {
                envelop.Recycle();
            }
        }

        private void ProcessLogin(LoginData data, NetConnection connection)
        {
            Log.Debug("->... Received login request from \"{0}\", {1}", data.Name, connection.RemoteEndPoint);

            //Create player session environment
            var sessionId = Interlocked.Increment(ref _sessionIdCounter);
            var client = _server.CreateClient(connection, sessionId);

            //Load player simulator data or create them
            //Create bc loading is not implemented
            //Create map for player
            var startGlobe = _world.Globes.First();
            var centerOfTheGlobe1 = new Vector2i(startGlobe.Bounds.Size.X / 2 + startGlobe.Bounds.Min.X, startGlobe.Bounds.Size.Z / 2 + startGlobe.Bounds.Min.Z);
            var playerStartPosition = Chunk.ToWorldPosition(centerOfTheGlobe1, new Vector3i(0, 40, 0)).ToObjectPosition();
            var mapBounds = new Bounds2i(Chunk.ToChunkPosition(playerStartPosition.ToMapPosition()), Simulator.SimulationWindowRadius);
            var map = new Map(sessionId, startGlobe, mapBounds);

            //Create player and his proxy
            var player = new Player(sessionId, data.Name, map, _timer, playerStartPosition);
            var playerController = new HeroController(client, player, map);

            startGlobe.AddPlayer(playerController);

            //Confirm login
            client.LoginAccept(player.Id, player.Position, Quaternion.Identity, Simulator.SimulationWindowRadius, playerController);
        }

        private void ProcessDisconnect(Disconnect data, HeroController hero)
        {
            Log.Info("->... Received disconnect request from \"{0}\"", hero.Player.Name);

            //Close not network parts of simulator...

            //Remove player from gameworld and close network stuff
            hero.Map.Globe.RemovePlayer(hero);
            hero.Client.Close();
            hero.Dispose();
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly Network.Server _server;
        private readonly World _world;
        private readonly Timer _timer;
        private int _sessionIdCounter;

        private readonly ActionBlock<IncomingEnvelop> _dispatch;
    }
}

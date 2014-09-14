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
        public Engine(Silentor.TB.Server.Network.Server server, World world, Timer timer)
        {
            Log.Trace("Start engine");
            
            _server = server;
            _world = world;
            _timer = timer;

            _dispatch = new ActionBlock<IncomingEnvelop>(im => DispatchMessages(im));

            server.MessageReceived.Subscribe(_dispatch.AsObserver());
        }

        private async Task DispatchMessages(IncomingEnvelop envelop)
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
                        ProcessDisconnect((Disconnect) envelop.Message, envelop.Client);
                        break;

                    case Headers.PlayerMovement:
                    {
                        //Do not async call player actions, bc they are should be very short
                        var simulator = envelop.Client;
                        simulator.ProcessAction(envelop.Message);
                    }
                        break;

                    case Headers.GetChunk:
                    {
                        var data = (ChunkRequestMessage) envelop.Message;
                        var simulator = envelop.Client;
                        var chunk = await simulator.Map.GetChunkAsync(data.Position);
                        if (chunk != null)
                            simulator.Session.SendChunk(chunk.ToChunkContents());
                    }
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
            Log.Info("->... Received login request from \"{0}\", {1}", data.Name, connection.RemoteEndPoint);

            //Create player session environment
            var sessionId = Interlocked.Increment(ref _sessionIdCounter);
            var session = new Session(connection, _server, sessionId);

            //Load player simulator data or create them
            //Create bc loading is not implemented
            //Create map for player
            var startGlobe = _world.Globes.First();
            var centerOfTheGlobe1 = new Vector2i(startGlobe.Bounds.Size.X / 2 + startGlobe.Bounds.Min.X, startGlobe.Bounds.Size.Z / 2 + startGlobe.Bounds.Min.Z);
            var playerStartPosition = Chunk.ToWorldPosition(centerOfTheGlobe1, new Vector3i(0, 40, 0)).ToObjectPosition();
            var mapBounds = new Bounds2i(Chunk.ToChunkPosition(playerStartPosition.ToMapPosition()), Simulator.SimulationWindowRadius);
            var map = new Map(sessionId, startGlobe, mapBounds);

            //Create player and his proxy
            var player = new Player(sessionId, data.Name, map, _timer, playerStartPosition, _world);    //todo World reference must be removed
            var playerController = new PlayerController(session, player, map);

            //Create simulator
            var simulator = new Simulator(player, map, playerController, session);

            _world.AddPlayer(simulator);

            //Confirm login
            session.LoginAccept(player.Id, player.Position, Quaternion.Identity, Simulator.SimulationWindowRadius, simulator);
        }

        private void ProcessDisconnect(Disconnect data, Simulator player)
        {
            Log.Info("->... Received disconnect request from \"{0}\", {1}", player.Player.Name, player.Session.Connection.RemoteEndPoint);

            //Close not network parts of simulator...

            //Remove player and close network stuff
            _world.RemovePlayer(player);
            player.Dispose();
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly Silentor.TB.Server.Network.Server _server;
        private readonly World _world;
        private readonly Timer _timer;
        private int _sessionIdCounter;

        private readonly ActionBlock<IncomingEnvelop> _dispatch;
    }
}

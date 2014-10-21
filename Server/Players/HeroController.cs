using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NLog;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Server.Maps;
using Silentor.TB.Server.Network;
using Silentor.TB.Server.Tools;

namespace Silentor.TB.Server.Players
{
    /// <summary>
    /// Updates client when player is changed, translates client actions to player
    /// </summary>
    public class HeroController
    {
        public readonly Player Player;
        public readonly Client Client;
        public readonly Map Map;

        public HeroController(Client client, Player player, Map map)
        {
            Log = LogManager.GetLogger(GetType().FullName + player.Id);

            //Processing of not urgent client requests
            _clientRequestProcessBlock = new ActionBlock<ClientRequest>(msg => ProcessClientRequest(msg), 
                new ExecutionDataflowBlockOptions {SingleProducerConstrained = true});

            //Setup client event handlers
            Client = client;
            Client.HeroActionReceived += ProcessAction;
            Client.ClientRequestReceived.LinkTo(_clientRequestProcessBlock, new DataflowLinkOptions());

            Player = player;
            Map = map;
            //_map.ChunkAdded += MapOnChunkAdded;
            //player.PositionChanged += PlayerOnPositionChanged;

            _lastChunkPosition = Chunk.ToChunkPosition(Player.Position.ToMapPosition());
        }

        private async void ProcessAction(Message message)
        {
            switch (message.Header)
            {
                case Headers.HeroMovement:
                    {
                        var data = (HeroMovement)message;
                        Player.SetAcceleration(data.Movement.ToVector());
                        Player.SetRotation(data.Rotation.ToVector());
                        if (data.Jump)
                            Player.Jump();
                    }
                    break;
                
            }
        }

        public void Simulate()
        {
            Player.Simulate();
            UpdateMap();
        }

        /// <summary>
        /// Update client with changed data from player for the last tick
        /// </summary>
        public void Update()
        {
            //Update self position
            if (Player.IsPositionChanged || Player.IsRotationChanged)
                Client.SendPosition(Player.Id, Player.Position, Player.Rotation);

            //Update positions of another players around
            var newSensed = Player.Sensor.Collect().ToArray();
            var compare = new Sensor.SnapshotCompare(_oldSensed, newSensed);
            _oldSensed = newSensed;

            foreach (var player in compare.Same)
                if (player.IsPositionChanged)
                    Client.SendPosition(player.Id, player.Position, player.Rotation);

            foreach (var player in compare.Added)
                Client.SendPosition(player.Id, player.Position, player.Rotation);

            foreach (var player in compare.Removed)
                Client.SendPosition(player.Id, player.Position, player.Rotation, isRemoved: true);
        }

        public void Dispose()
        {
            Log.Info("Destroyed player simulator {0}", Player.Id);
        }

        private Vector2i _lastChunkPosition;
        
        private IEnumerable<IPlayer> _oldSensed = new IPlayer[0];
        private readonly Logger Log;
        private readonly ActionBlock<ClientRequest> _clientRequestProcessBlock;

        /// <summary>
        /// Update Map according to changes in player position
        /// </summary>
        private void UpdateMap()
        {
            //Translate and update map
            var chunkPosition = Chunk.ToChunkPosition(Player.Position.ToMapPosition());
            if (chunkPosition != _lastChunkPosition)
            {
                Map.Slide(chunkPosition - _lastChunkPosition);
                _lastChunkPosition = chunkPosition;
            }
        }

        private async Task ProcessClientRequest(ClientRequest msg)
        {
            Log.Debug("Process of client request: {0}", msg);

            switch (msg.Header)
            {
                case Headers.GetChunk:
                {
                    var data = (ChunkRequestMessage)msg;
                    var chunk = await Map.GetChunkAsync(data.Position);
                    if (chunk != null)
                        Client.SendChunk(chunk.ToChunkContents());
                }
                    break;

                default:
                    Log.Error("Unknown client request {0}", msg);
                    break;
            }
        }
    }
}

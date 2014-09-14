using System.Collections.Generic;
using System.Linq;
using NLog;
using Silentor.TB.Server.Maps;
using Silentor.TB.Server.Players;

namespace Silentor.TB.Server.Network
{
    /// <summary>
    /// Updates client when player is changed, translates client actions to player
    /// </summary>
    public class PlayerController
    {
        public readonly Player Player;

        public PlayerController(Session client, Player player, Map map)
        {
            Log = LogManager.GetLogger("Wob.Server.Network.PlayerController" + player.Id);

            _client = client;
            Player = player;
            _map = map;
            //_map.ChunkAdded += MapOnChunkAdded;
            //player.PositionChanged += PlayerOnPositionChanged;
        }

        /// <summary>
        /// Update client with changed data from player for the last tick
        /// </summary>
        public void UpdateClient()
        {
            //Update self position
            if (Player.IsPositionChanged || Player.IsRotationChanged)
                _client.SendPosition(Player.Id, Player.Position, Player.Rotation);

            //Update positions of another players around
            var newSensed = Player.Sensor.Collect().ToArray();
            var compare = new Sensor.SnapshotCompare(_oldSensed, newSensed);
            _oldSensed = newSensed;

            foreach (var player in compare.Same)
                if (player.IsPositionChanged)
                    _client.SendPosition(player.Id, player.Position, player.Rotation);

            foreach (var player in compare.Added)
                _client.SendPosition(player.Id, player.Position, player.Rotation);

            foreach (var player in compare.Removed)
                _client.SendPosition(player.Id, player.Position, player.Rotation, isRemoved: true);
        }

        private readonly Session _client;

        private readonly Map _map;
        private IEnumerable<IPlayer> _oldSensed = new IPlayer[0];
        private readonly Logger Log;
    }
}

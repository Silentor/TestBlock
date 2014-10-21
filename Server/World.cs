using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NLog;
using OpenTK.Graphics.OpenGL;
using Silentor.TB.Server.Maps;
using Silentor.TB.Server.Network;
using Silentor.TB.Server.Players;

namespace Silentor.TB.Server
{
    /// <summary>
    /// Manages game world stuff: all globes, players, mobs etc
    /// </summary>
    public class World
    {
        /// <summary>
        /// All globes of the world
        /// </summary>
        public readonly IEnumerable<Globe> Globes;

        /// <summary>
        /// All players of the world
        /// </summary>
        public IReadOnlyList<HeroController> Players
        {
            get
            {
                if(_playersChanged)
                    lock (_players)
                        if (_playersChanged)
                            _playersCache = _players.ToArray();

                return _playersCache; 
            }
        }

        public World(Globe globe)
        {
            Globes = new[] {globe};
        }

        public void AddPlayer(HeroController player)
        {
            lock (_players)
            {
                Debug.Assert(!_players.Contains(player));

                _players.Add(player);
                _playersChanged = true;
            }

            Log.Debug("Added player {0} to gameworld", player.Player.Name);
        }

        public void RemovePlayer(HeroController player)
        {
            lock (_players)
            {
                Debug.Assert(_players.Contains(player));

                _players.Remove(player);
                _playersChanged = true;
            }

            Log.Debug("Removed player {0} from gameworld", player.Player.Name);
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly List<HeroController> _players = new List<HeroController>();
        private bool _playersChanged = true;
        private IReadOnlyList<HeroController> _playersCache;

    }
}

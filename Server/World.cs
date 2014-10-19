using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NLog;
using Silentor.TB.Server.Maps;
using Silentor.TB.Server.Players;

namespace Silentor.TB.Server
{
    /// <summary>
    /// Manages all-players stuff
    /// </summary>
    public class World
    {
        private readonly Network.Server _server;
        private readonly Globe _globe;
        private readonly Time.Timer _timer;

        /// <summary>
        /// All globes of the world
        /// </summary>
        public readonly IEnumerable<Globe> Globes;

        public IReadOnlyList<Simulator> Players
        {
            get
            {
                var changes = Interlocked.Exchange(ref _playersLock, 0);
                if (_playersCache == null || changes > 0)
                    _playersCache = _players.Values.ToArray();

                return _playersCache; 
            }
        }

        public World(Silentor.TB.Server.Network.Server server, Globe globe, Time.Timer timer)
        {
            _server = server;
            _globe = globe;
            _timer = timer;
            Globes = new[] {_globe};
        }

        public void AddPlayer(Simulator simulator)
        {
            if (!_players.TryAdd(simulator, simulator))
                throw new InvalidOperationException("Players simulator already added");
            Interlocked.Increment(ref _playersLock);

            Log.Info("Added player {0}", simulator.Player.Name);
        }

        public void RemovePlayer(Simulator player)
        {
            if(_players.TryRemove(player, out player))
                Interlocked.Increment(ref _playersLock);        //Not very concurrently
        }


        //private void ProcessGetChunk(ChunkRequestMessage message, Simulator client)
        //{
        //    var chunk = client.Map.GetChunk(message.Position);

        //    //todo chunk must be locked while queuing on send
        //    if (chunk != null)
        //        client.Session.SendChunk(chunk.ToChunkContents());
        //    else
        //        client.Controller.StoreChunkRequest(message);
        //}

        private static Logger Log = LogManager.GetCurrentClassLogger();

        private ConcurrentDictionary<Simulator, Simulator> _players = new ConcurrentDictionary<Simulator, Simulator>();
        private Simulator[] _playersCache;
        private int _playersLock;
        private static int _playersCounter = 0;

    }
}

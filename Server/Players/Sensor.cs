using System;
using System.Collections.Generic;
using System.Linq;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Server.Maps;
using Silentor.TB.Server.Tools;

namespace Silentor.TB.Server.Players
{
    /// <summary>
    /// Collect object around
    /// </summary>
    public class Sensor
    {
        private readonly Map _map;
        private readonly int _range;
        private readonly Time.Timer _timer;
        private readonly Globe _globe;
        private readonly Player _owner;
        private readonly World _world;

        private int _cachedTick = -1;
        private Player[] _cachedCollect;
        

        public Sensor(Map map, World world, int range, Time.Timer timer, Player owner)
        {
            _map = map;
            _range = range;
            _timer = timer;
            _world = world;             //todo Instead of World use Globe!
            _owner = owner;
        }

        /// <summary>
        /// Get all sensed targets
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IPlayer> Collect()
        {
            if (_cachedTick != _timer.TickCount)
            {
                _cachedTick = _timer.TickCount;
                var lookBound = new Bounds3i(_owner.Position.ToMapPosition(), _range);

                var targets = new List<Player>();
                var allEntities = _world.Players;
                foreach (var entity in allEntities)
                {
                    if (lookBound.Contains(entity.Player.Position.ToMapPosition()) && entity.Player != _owner)
                        targets.Add(entity.Player);
                }

                _cachedCollect = targets.ToArray();
            }

            return _cachedCollect;
        }

        public struct SnapshotCompare
        {
            private List<IPlayer> _same;
            private List<IPlayer> _added;
            private List<IPlayer> _removed;

            public IEnumerable<IPlayer> Same { get { return _same; } }
            public IEnumerable<IPlayer> Added { get { return _added; } }
            public IEnumerable<IPlayer> Removed { get { return _removed; } }

            public SnapshotCompare(IEnumerable<IPlayer> oldList, IEnumerable<IPlayer> newList)
                : this()
            {
                _same = new List<IPlayer>(Math.Max(oldList.Count(), newList.Count()));
                _added = new List<IPlayer>(Math.Min(oldList.Count(), newList.Count()));
                _removed = new List<IPlayer>(Math.Min(oldList.Count(), newList.Count()));

                using (var old = oldList.GetEnumerator())
                {
                    using (var @new = newList.GetEnumerator())
                    {
                        bool oldNext;
                        bool newNext;

                        if ((oldNext = old.MoveNext()) & (newNext = @new.MoveNext()))
                        {
                            while (true)
                            {
                                var compared = old.Current.Id.CompareTo(@new.Current.Id);
                                if (compared == 0)
                                {
                                    _same.Add(old.Current);
                                    if (!(oldNext = old.MoveNext()) | !(newNext = @new.MoveNext())) break;
                                }
                                else if (compared == -1)
                                {
                                    _removed.Add(old.Current);
                                    // Keep searching A
                                    if ((oldNext = old.MoveNext()) == false) break;
                                }
                                else
                                {
                                    _added.Add(@new.Current);
                                    // Keep searching B
                                    if ((newNext = @new.MoveNext()) == false) break;
                                }
                            }
                        }

                        if (oldNext != newNext)
                            if (!oldNext)
                                do
                                {
                                    _added.Add(@new.Current);
                                } while (@new.MoveNext());
                            else
                                do
                                {
                                    _removed.Add(old.Current);
                                } while (old.MoveNext());
                    }
                }
            }
        }
    }
}

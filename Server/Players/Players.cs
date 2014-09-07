using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Wob.Server.Players
{
    /// <summary>
    /// Manages list of heroes in the world
    /// </summary>
    public class Players
    {
        public int GetNewId()
        {
            return Interlocked.Increment(ref _id);
        }

        public void AddHero([NotNull] Simulator player)
        {
            if (player == null) throw new ArgumentNullException("player");

            _heroes.Add(player);
        }

        public void RemoveHero([NotNull] Simulator player)
        {
            if (player == null) throw new ArgumentNullException("player");

            _heroes.Remove(player);
        }

        public Simulator[] GetAll()
        {
            //todo naive implementation
            return _heroes.ToArray();
        }

        private static int _id;
        private List<Simulator> _heroes = new List<Simulator>();
    }
}

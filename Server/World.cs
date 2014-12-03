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

        

        public World(Globe globe)
        {
            Globes = new[] {globe};
        }

        

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

       

    }
}

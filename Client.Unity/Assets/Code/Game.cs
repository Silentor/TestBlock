using Assets.Scripts;
using Assets.Scripts.Visualization;
using NLog;
using Silentor.TB.Client;
using Silentor.TB.Client.Input;
using Silentor.TB.Client.Maps;
using Silentor.TB.Client.Network;
using Silentor.TB.Client.Players;
using UnityEngine;

namespace Silentor.TB.Client
{
    public class Game
    {
        private readonly IServer _server;
        private readonly IInput _input;
        private readonly IWorld _world;
        private readonly IClientApp _clientApp;
        private readonly IPlayer _player;
        private readonly IMapLoader _mapLoader;

        private static readonly Logger Log = LogManager.GetLogger("Client.Game");

        private Game(IServer server, IInput input, IClientApp clientApp, IMapLoader mapLoader)
        {
            Log.Info("Game started");

            _server = server;

            _input = input;
            _clientApp = clientApp;
            _mapLoader = mapLoader;

            _input.Moved += InputOnMoved;
        }

        private void InputOnMoved(Vector3 movement, Vector2 rotation, bool jump = false)
        {
            _server.ServerConnection.MovePlayer(movement, rotation, jump);
        }

        

    }
}

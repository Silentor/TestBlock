using Assets.Scripts;
using ModestTree.Zenject;
using Silentor.TB.Client.Debug;
using Silentor.TB.Client.Network;
using Silentor.TB.Common.Network.Messages;


namespace Silentor.TB.Client
{
    /// <summary>
    /// Start game when login was successful
    /// </summary>
    public class GameRoot : IDependencyRoot
    {
        [Inject]
        public Logging Logging;

        [Inject]
        public IServer Server;

        [Inject]
        public GameModule GameModule;

        private Game _game;

        public void Start()
        {
            //Login
            Server.ClientConnection.Connected += ClientConnectionOnConnected;
            Server.ClientConnection.Logined += ClientConnectionOnLogined;
        }

        private void ClientConnectionOnConnected()
        {
            Server.ServerConnection.AddPlayer("Test");
        }

        private void ClientConnectionOnLogined(LoginResponce loginResponce)
        {
            GameModule.RegisterGameSettings(loginResponce);

            //True game root
            _game = GameModule.CreateGame();
        }

        public void Dispose()
        {
            
        }
    }
}

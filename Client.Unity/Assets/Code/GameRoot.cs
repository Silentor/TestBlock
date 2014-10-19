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
    public class GameRoot : IInitializable
    {
        [Inject]
        public Logging Logging;

        [Inject]
        public IServer Server;

        [Inject]
        public GameModule GameModule;

        public void Initialize()
        {
            Server.ClientConnection.Connected += ClientConnectionOnConnected;
            Server.ClientConnection.Logined += ClientConnectionOnLogined;
        }

        private Game _game;

        private void ClientConnectionOnConnected()
        {
            Server.ServerConnection.AddPlayer("Test");
        }

        private void ClientConnectionOnLogined(LoginResponce loginResponce)
        {
            GameModule.RegisterGameSettings(loginResponce);

            //True game root resolved and started after login 
            //todo connect to server and login should be processed in another (login) scene
            _game = GameModule.CreateGame();
        }
    }
}

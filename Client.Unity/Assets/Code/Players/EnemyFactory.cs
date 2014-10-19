using ModestTree.Zenject;
using Silentor.TB.Client.Players;

namespace Silentor.TB.Client.Players
{
    public class EnemyFactory : IEnemyFactory
    {
        private readonly DiContainer _container;
        private readonly Factory<Enemy> _factory;

        public EnemyFactory(DiContainer container)
        {
            _container = container;
            _factory = new Factory<Enemy>();
        }

        public IActorEditor Create(ActorConfig config)
        {
            using (var scope = _container.CreateScope())
            {
                scope.Bind<ActorConfig>().To(config).WhenInjectedInto<Enemy>();
                return _factory.Create();
            }            
        }
    }
}

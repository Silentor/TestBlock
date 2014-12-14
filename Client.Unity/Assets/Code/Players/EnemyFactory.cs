using ModestTree.Zenject;
using Silentor.TB.Client.Players;

namespace Silentor.TB.Client.Players
{
    public class EnemyFactory : IEnemyFactory
    {
        private readonly DiContainer _container;
        private readonly Factory _factory;

        public EnemyFactory(DiContainer container, Factory factory)
        {
            _container = container;
            _factory = factory;
        }

        public IActorEditor Create(ActorConfig config)
        {
            using (var scope = _container.CreateScope())
            {
                scope.Bind<ActorConfig>().To(config).WhenInjectedInto<Enemy>();
                return _factory.Create();
            }            
        }

        public class Factory : Factory<Enemy>
        { }
    }
}

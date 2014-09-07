using ModestTree.Zenject;
using Silentor.TB.Client.Players;
using Silentor.TB.Client.Visualization.Views;

namespace Assets.Code.Visualization
{
    /// <summary>
    /// Create another players views
    /// </summary>
    public interface IEnemyViewFactory
    {
        /// <summary>
        /// Create player editor
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        TestEnemyView Create(IActor actor);
    }

    public class EnemyViewFactory : IEnemyViewFactory
    {
        private readonly DiContainer _container;
        private readonly Factory<TestEnemyView> _factory;

        public EnemyViewFactory(DiContainer container)
        {
            _container = container;
        }

        public TestEnemyView Create(IActor actor)
        {
            using (var scope = _container.CreateScope())
            {
                scope.Bind<IActor>().To(actor);
                var enemy = _container.Resolve<TestEnemyView>();
                enemy.transform.parent = null;
                return enemy;
            }
        }
    }
}

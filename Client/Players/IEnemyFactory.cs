namespace Silentor.TB.Client.Players
{
    public interface IEnemyFactory
    {
        /// <summary>
        /// Create player editor
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        IActorEditor Create(ActorConfig config);
    }
}
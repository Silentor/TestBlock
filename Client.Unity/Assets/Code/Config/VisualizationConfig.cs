using Assets.Code.Visualization;
using Assets.Code.Visualization.Views;
using Assets.Scripts.Visualization;

namespace Silentor.TB.Client.Config
{
    public interface IVisualizationConfig
    {
        BlocksAtlas Blocks { get; }

        TestPlayerView PlayerPrefab { get; }

        MapGizmos MapVisual { get; }
    }
}

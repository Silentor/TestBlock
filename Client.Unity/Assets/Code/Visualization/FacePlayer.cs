using Assets.Code.Visualization.Views;
using ModestTree.Zenject;
using UnityEngine;

namespace Silentor.TB.Client.Visualization
{
    /// <summary>
    /// Rotate GO to face Player view
    /// </summary>
    public class FacePlayer : MonoBehaviour
    {
        [Inject] 
        private TestPlayerView PlayerView;

        private Transform _transform;

        void Awake()
        {
            _transform = transform;
        }

        void LateUpdate()
        {
            _transform.LookAt(PlayerView.Position);
        }

    }
}

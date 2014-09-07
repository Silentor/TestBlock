using System.Collections;
using Assets.Code.Visualization.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Silentor.TB.Client.Debug
{
    [RequireComponent(typeof(Text))]
    public class PositionDebugLabel : MonoBehaviour 
    {
        public float UpdateInterval = 1F;

        private Text _label;
        private TestPlayerView _player;

        void Awake()
        {
            if (!UnityEngine.Debug.isDebugBuild)
            {
                Destroy(gameObject);
                return;
            }

            _label = GetComponent<Text>();
            _label.text = "Wait for player view";

            StartCoroutine(UpdateLabel());
        }

        IEnumerator UpdateLabel()
        {
            while (true)
            {
                if (_player == null)
                    _player = FindObjectOfType<TestPlayerView>();

                if(_player != null)
                    _label.text = string.Format("{0} position: {1}", _player.Id, _player.Position);

                yield return new WaitForSeconds(UpdateInterval);
            }
        }
    }
}

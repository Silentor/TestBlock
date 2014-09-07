using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Silentor.TB.Client.Debug
{
    [RequireComponent(typeof(Text))]
    public class MemDebugLabel : MonoBehaviour 
    {
        public float UpdateInterval = 1F;

        private Text _label;

        void Awake()
        {
            if (!UnityEngine.Debug.isDebugBuild)
            {
                Destroy(gameObject);
                return;
            }

#if ENABLE_PROFILER
            _label = GetComponent<Text>();
            StartCoroutine(UpdateLabel());
#else
        GetComponent<Text>().text = "Profiler disabled";
#endif

        }

        IEnumerator UpdateLabel()
        {
            while (true)
            {
                _label.text = string.Format("Mem: {0:F1}/{1:F1}", Profiler.GetTotalAllocatedMemory() / (1024 * 1024), Profiler.GetTotalReservedMemory() / (1024 * 1024));
                yield return new WaitForSeconds(UpdateInterval);
            }
        }
    }
}
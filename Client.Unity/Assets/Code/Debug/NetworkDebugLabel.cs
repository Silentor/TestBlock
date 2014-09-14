using System;
using ModestTree.Zenject;
using Silentor.TB.Client.Network;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Silentor.TB.Client.Debug
{
    [RequireComponent(typeof(Text))]
    public class NetworkDebugLabel : MonoBehaviour
    {
        public float UpdateInterval = 1F;

        private Text _label;
        private IServer _server;

        void Awake()
        {
            if (!UnityEngine.Debug.isDebugBuild)
            {
                Destroy(gameObject);
                return;
            }

            _label = GetComponent<Text>();

        }

        void Start()
        {
            var container = FindObjectOfType<CompositionRoot>().Container;
            _server = container.Resolve<IServer>();

            StartCoroutine(UpdateLabel());
        }

        IEnumerator UpdateLabel()
        {
            while (true)
            {
                var status = _server.IsConnected;
                if (status)
                {
                    var rtt = _server.RTT;
                    var sendBytes = _server.SendBytes;
                    var recvBytes = _server.RecvBytes;

                    _label.color = Color.green;
                    _label.text = String.Format("Online, RTT: {0} msec, recv: {1} b, send: {2} b", (int) (rtt*1000),
                        recvBytes, sendBytes);
                }
                else
                {
                    _label.color = Color.red;
                    _label.text = "Offline";
                }

                yield return new WaitForSeconds(UpdateInterval);
            }
        }
    }
}
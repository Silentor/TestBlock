using UnityEngine;

namespace Assets.Code.Benchmarks
{
    public class NetworkBenchmark : MonoBehaviour {

        // Use this for initialization
        void Start ()
        {
            //var connectionInfo = new ConnectionInfo("192.168.0.101", 10000);
            //var conn = TCPConnection.GetConnection(connectionInfo, NetworkOptions.Options);
            //StartCoroutine(Benchmark(conn));
        }

        //private IEnumerator Benchmark(Connection conn)
        //{
        //    print("Start benchmark");

        //    var auth = conn.SendReceiveObject<string>("Login", "Auth", 1000, "Name");
        //    print("Auth: " + auth);

        //    int i = 0;
        //    long totaltime = 0;
        //    while (true)
        //    {
        //        var chunkReq = new ChunkRequestMessage() {Position = new Vector2i(i, i)};

        //        var sw = Stopwatch.StartNew();
        //        var chunk = conn.SendReceiveObject<ChunkResponceMessage>("ChunkRequest", "ChunkResponce", 1000, chunkReq);
        //        //conn.SendObject("ChunkRequest", chunkReq);

        //        totaltime += sw.ElapsedMilliseconds;
        //        print("time " + totaltime / i);

        //        yield return new WaitForSeconds(0.1f);
        //    }
        //}

        void OnDestroy()
        {
            //NetworkComms.Shutdown();
        }
	
    }
}

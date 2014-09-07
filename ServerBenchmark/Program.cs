using NLog;

namespace Silentor.TB.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.EnableLogging();

            //NetworkComms.EnableLogging();
            //NetworkComms.DefaultSendReceiveOptions = NetworkOptions.Options;

            //NetworkComms.AppendGlobalConnectionEstablishHandler(connection =>
            //{
            //    Console.WriteLine("Connected " + connection.ConnectionInfo.RemoteEndPoint.ToString());
            //    //connection.EstablishConnection();
            //});

            //NetworkComms.AppendGlobalIncomingPacketHandler<ChunkRequestMessage>("ChunkRequest", (header, connection, chunkRequest) =>
            //{
            //    Console.WriteLine("Received request for chunk " + chunkRequest.Position);

            //    var chunk = new ChunkResponceMessage() {Blocks = new byte[32768], HeightMap = new byte[256]};
            //    //var chunk = new ChunkResponceMessage();
            //    connection.SendObject("ChunkResponce", chunk);
            //});

            //NetworkComms.AppendGlobalIncomingPacketHandler<string>("Login", (header, connection, message) =>
            //{
            //    Console.WriteLine("Received login " + message);

            //    connection.SendObject("Auth", "Ok");
            //});

            //NetworkComms.AppendGlobalConnectionCloseHandler(connection =>
            //{
            //    Console.WriteLine("Closed " + connection.ConnectionInfo.ToString());
            //});

            //Console.WriteLine("WoB server started, version 1");

            ////Start listening for incoming connections
            //TCPConnection.StartListening(true);

            ////Print out the IPs and ports we are now listening on
            //Console.WriteLine("Server listening for TCP connection on: ");
            //foreach (var localEndPoint in TCPConnection.ExistingLocalListenEndPoints()) 
            //    Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);

            //Console.ReadLine();

            ////We have used NetworkComms so we should ensure that we correctly call shutdown
            //NetworkComms.Shutdown();
        }
    }
}

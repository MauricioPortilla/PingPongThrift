using System;
using Thrift.Server;
using Thrift.Transport;
using Thrift.Transport.Server;
using System.Threading;
using System.Threading.Tasks;

namespace PingPongGameServer {
    class Program {
        static void Main(string[] args) {
            var handler = new PingPongServiceHandler();
            var processor = new PingPongService.AsyncProcessor(handler);
            TServerTransport transport = new TServerSocketTransport(5000);
            TServer server = new TThreadPoolAsyncServer(processor, transport);
            Console.WriteLine(">> Starting Ping Pong Server...");
            server.ServeAsync(new CancellationToken());
            Console.WriteLine("> Press enter key to stop server...");
            Task.Run(() => {
                while (true) {
                    if (handler.players.Count == 2) {
                        if (!handler.MoveBall()) {
                            Thread.Sleep(1000);
                            handler.Reset();
                            Console.WriteLine(">> Game ended. Players disconnected.");
                        }
                        Thread.Sleep(100);
                    }
                }
            });
            while (true) {
                if (Console.Read() == '\n') {
                    break;
                }
            }
            server.Stop();
        }
    }
}

using System;
using Thrift;
using PingPongGameServer;
using Thrift.Server;
using Thrift.Transport;
using Thrift.Transport.Server;
using Thrift.Protocol;
using System.Threading;
using System.Threading.Tasks;

namespace PingPongGameServer {
    class Program {
        static void Main(string[] args) {
            var handler = new PingPongServiceHandler();
            var processor = new PingPongService.AsyncProcessor(handler);
            TServerTransport transport = new TServerSocketTransport(5000);
            TServer server = new TThreadPoolAsyncServer(processor, transport);
            Console.WriteLine(">> Starting server...");
            server.ServeAsync(new CancellationToken());
            Console.WriteLine("> Press enter key to stop server...");
            Task.Run(() => {
                while (true) {
                    if (handler.players.Count == 2) {
                        if (!handler.MoveBall()) {
                            handler.Reset();
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

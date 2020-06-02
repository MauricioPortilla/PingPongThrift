using System;
using System.Threading;
using System.Threading.Tasks;
using Thrift;
using Thrift.Protocol;
using Thrift.Transport;
using Thrift.Transport.Client;

namespace PingPongGameClient {
    class Program {
        private const int CLIENT_WINDOW_WIDTH = 100;
        private const int CLIENT_WINDOW_HEIGHT = 30;
        private static PingPongService.Client client;
        private static PlayerPadPosition[] players = {
            new PlayerPadPosition {
                PlayerId = 0,
                Position = new Position {
                    X = 0,
                    Y = CLIENT_WINDOW_HEIGHT / 2
                }
            },
            new PlayerPadPosition {
                PlayerId = 1,
                Position = new Position {
                    X = CLIENT_WINDOW_WIDTH - 1,
                    Y = CLIENT_WINDOW_HEIGHT / 2
                }
            }
        };
        private static string headerText = "";
        private static int selfPlayerId = 0;
        private static int opponentPlayerId = 0;
        private static int playerOneScore = 0;
        private static int playerTwoScore = 0;
        private static Position ballPosition = new Position {
            X = CLIENT_WINDOW_WIDTH / 2,
            Y = CLIENT_WINDOW_HEIGHT / 2
        };
        private static bool hasGameFinished = false;
        private const int MAX_SCORE = 1;

        static async Task Main(string[] args) {
            bool isGameRunning = false;
            bool hasGameStarted = false;
            Console.SetWindowSize(CLIENT_WINDOW_WIDTH, CLIENT_WINDOW_HEIGHT);
            Console.BufferHeight = CLIENT_WINDOW_HEIGHT;
            Console.BufferWidth = CLIENT_WINDOW_WIDTH;
            try {
                TTransport transport = new TSocketTransport("localhost", 5000);
                TProtocol protocol = new TBinaryProtocol(transport);
                client = new PingPongService.Client(protocol);
                while (true) {
                    if (!isGameRunning) {
                        selfPlayerId = await client.GetPlayerIdAsync();
                        opponentPlayerId = selfPlayerId == 0 ? 1 : 0;
                        await client.SendPlayerPadPositionAsync(players[selfPlayerId]);
                        isGameRunning = true;
                    }
                    if (!Console.KeyAvailable) {
                        try {
                            var opponentPlayerData = await client.GetLatestPlayerPadPositionAsync(opponentPlayerId);
                            players[opponentPlayerId] = opponentPlayerData;
                            hasGameStarted = true;
                        } catch (TException) {
                            // Opponent is not available.
                            if (hasGameStarted) {
                                headerText = "You won!";
                                hasGameStarted = false;
                            } else {
                                headerText = "Waiting for an opponent...";
                            }
                        }
                        DrawPads();
                        if (hasGameFinished) {
                            break;
                        }
                        if (hasGameStarted) {
                            DrawBall();
                            MoveBall();
                            //CheckScore();
                        }
                        Thread.Sleep(100);
                        continue;
                    }
                    switch (Console.ReadKey().Key) {
                        case ConsoleKey.DownArrow:
                            if (players[selfPlayerId].Position.Y < CLIENT_WINDOW_HEIGHT) {
                                players[selfPlayerId].Position.Y++;
                                await client.SendPlayerPadPositionAsync(players[selfPlayerId]);
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            if (players[selfPlayerId].Position.Y > 0) {
                                players[selfPlayerId].Position.Y--;
                                await client.SendPlayerPadPositionAsync(players[selfPlayerId]);
                            }
                            break;
                    }
                }
                transport.Close();
                Console.Read();
            } catch (TApplicationException tApplicationException) {
                Console.Clear();
                Console.WriteLine(tApplicationException.StackTrace);
                Console.Read();
            } catch (Exception exception) {
                Console.Clear();
                Console.WriteLine(">> No hay conexión con el servidor.\n\n> Stack trace:");
                Console.WriteLine(exception.StackTrace);
                Console.Read();
            }
        }

        private static void DrawHeader() {
            Console.SetCursorPosition((CLIENT_WINDOW_WIDTH / 2) - (headerText.Length / 2), 0);
            Console.Write(headerText);
        }

        private static void DrawPads() {
            Console.Clear();
            Console.SetCursorPosition(players[0].Position.X, players[0].Position.Y);
            Console.Write("|\n|\n|\n|\n|");
            for (int i = 0; i < 5; i++) {
                Console.SetCursorPosition(players[1].Position.X, players[1].Position.Y++);
                Console.Write("|");
            }
            players[1].Position.Y -= 5;
            DrawHeader();
        }

        private static void DrawBall() {
            headerText = playerOneScore + " - " + playerTwoScore;
            DrawPads();
            Console.SetCursorPosition(ballPosition.X, ballPosition.Y);
            Console.Write("*");
        }

        private static async Task CheckScore() {
            int selfPlayerScore = await client.GetPlayerScoreAsync(selfPlayerId);
            int opponentPlayerScore = await client.GetPlayerScoreAsync(opponentPlayerId);
            if (selfPlayerScore == MAX_SCORE) {
                headerText = "You win!";
                hasGameFinished = true;
            } else if (opponentPlayerScore == MAX_SCORE) {
                headerText = "Opponent wins!";
                hasGameFinished = true;
            }
        }

        private static async void MoveBall() {
            ballPosition = await client.GetBallPositionAsync();
            if (ballPosition.X == (CLIENT_WINDOW_WIDTH / 2) - 1 && ballPosition.Y == (CLIENT_WINDOW_HEIGHT / 2) - 1) {
                await CheckScore();
            }
        }
    }
}

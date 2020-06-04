/**
 * Ping pong game: a classic game made using Apache Thrift.
 * 
 * Created by:
 * > Mauricio Cruz Portilla <mauricio.portilla@hotmail.com>
 * 
 * June 03, 2020
 */

using System;
using System.Runtime.InteropServices;
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
        private const int MAX_SCORE = 2;

        private static PingPongService.Client client;
        private static readonly PlayerPadPosition[] players = {
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
        private static bool isGameRunning = false;
        private static bool hasGameFinished = false;
        private static bool hasGameStarted = false;

        /// <summary>
        /// Creates a game instance.
        /// </summary>
        /// <param name="args">If first value is given, it will be considered as Server IP</param>
        /// <returns>Task</returns>
        static async Task Main(string[] args) {
            string serverIp = "localhost";
            if (args.Length > 0) {
                serverIp = args[0];
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Console.SetWindowSize(CLIENT_WINDOW_WIDTH, CLIENT_WINDOW_HEIGHT);
                Console.BufferHeight = CLIENT_WINDOW_HEIGHT;
                Console.BufferWidth = CLIENT_WINDOW_WIDTH;
            }
            try {
                TTransport transport = new TSocketTransport(serverIp, 5000);
                TProtocol protocol = new TBinaryProtocol(transport);
                client = new PingPongService.Client(protocol);
                while (true) {
                    if (!isGameRunning) {
                        selfPlayerId = await client.JoinGameAsync();
                        opponentPlayerId = selfPlayerId % 2 == 0 ? selfPlayerId + 1 : selfPlayerId - 1;
                        await client.SendPlayerPadPositionAsync(players[selfPlayerId]);
                        isGameRunning = true;
                    }
                    if (!Console.KeyAvailable) {
                        try {
                            var opponentPlayerData = await client.GetLatestPlayerPadPositionAsync(opponentPlayerId);
                            players[opponentPlayerId] = opponentPlayerData;
                            hasGameStarted = true;
                        } catch (PlayerNotFoundException) {
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
                        }
                        Thread.Sleep(100);
                        continue;
                    }
                    switch (Console.ReadKey().Key) {
                        case ConsoleKey.DownArrow:
                            if (players[selfPlayerId].Position.Y < (CLIENT_WINDOW_HEIGHT - 4)) {
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
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.InnerException);
                Console.Read();
            }
        }

        /// <summary>
        /// Draws header.
        /// </summary>
        private static void DrawHeader() {
            Console.SetCursorPosition((CLIENT_WINDOW_WIDTH / 2) - (headerText.Length / 2), 0);
            Console.Write(headerText);
        }

        /// <summary>
        /// Draws player pads.
        /// </summary>
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

        /// <summary>
        /// Draws game ball and redraws player pads. Sets header text to player scores.
        /// </summary>
        private static void DrawBall() {
            headerText = playerOneScore + " - " + playerTwoScore;
            DrawPads();
            Console.SetCursorPosition(ballPosition.X, ballPosition.Y);
            Console.Write("*");
        }

        /// <summary>
        /// Retrieves latest players score and checks if any player has reached the maximum score to win the game.
        /// If any player wins, the header text will show the winner and the game will end.
        /// </summary>
        /// <returns>Task</returns>
        private static async Task CheckScore() {
            int selfPlayerScore = await client.GetPlayerScoreAsync(selfPlayerId);
            int opponentPlayerScore = await client.GetPlayerScoreAsync(opponentPlayerId);
            if (selfPlayerId % 2 == 0) {
                playerOneScore = selfPlayerScore;
                playerTwoScore = opponentPlayerScore;
            } else {
                playerOneScore = opponentPlayerScore;
                playerTwoScore = selfPlayerScore;
            }
            if (selfPlayerScore == MAX_SCORE) {
                headerText = "You win!";
                hasGameFinished = true;
            } else if (opponentPlayerScore == MAX_SCORE) {
                headerText = "Opponent wins!";
                hasGameFinished = true;
            }           
        }

        /// <summary>
        /// Retrieves ball position from server, so it can be moved when it gets redrawn.
        /// If ball is in the middle, player scores will be rechecked.
        /// </summary>
        private static async void MoveBall() {
            ballPosition = await client.GetBallPositionAsync();
            if (ballPosition.X == (CLIENT_WINDOW_WIDTH / 2) - 1 && ballPosition.Y == (CLIENT_WINDOW_HEIGHT / 2) - 1) {
                await CheckScore();
            }
        }
    }
}

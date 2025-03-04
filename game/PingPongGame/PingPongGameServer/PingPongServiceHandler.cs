﻿/**
 * Ping pong game: a classic game made using Apache Thrift.
 * 
 * Created by:
 * > Mauricio Cruz Portilla <mauricio.portilla@hotmail.com>
 * 
 * June 03, 2020
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PingPongGameServer {
    class PingPongServiceHandler : PingPongService.IAsync {
        private const int CLIENT_WINDOW_WIDTH = 100;
        private const int CLIENT_WINDOW_HEIGHT = 30;
        private const int MAX_SCORE = 2;
        public readonly List<PlayerPadPosition> players = new List<PlayerPadPosition>();
        private readonly List<int> playersScore = new List<int>();
        private static Position ballPosition = new Position {
            X = CLIENT_WINDOW_WIDTH / 2,
            Y = CLIENT_WINDOW_HEIGHT / 2
        };
        private static bool isBallGoingUp = true;
        private static bool isBallGoingLeft = true;

        /// <summary>
        /// Clears latest players data.
        /// </summary>
        public void Reset() {
            players.Clear();
            playersScore.Clear();
        }

        /// <summary>
        /// Puts game ball in the middle of screen.
        /// </summary>
        private void ResetBallPosition() {
            ballPosition.X = CLIENT_WINDOW_WIDTH / 2;
            ballPosition.Y = CLIENT_WINDOW_HEIGHT / 2;
            isBallGoingUp = true;
            isBallGoingLeft = true;
        }

        /// <summary>
        /// Moves ball through game field.
        /// </summary>
        /// <returns>true if no player has reached the maximum score to win; false if a player reached it.</returns>
        public bool MoveBall() {
            int playerOneScore = playersScore[0];
            int playerTwoScore = playersScore[1];
            if (playerOneScore == MAX_SCORE || playerTwoScore == MAX_SCORE) {
                return false;
            }
            if (ballPosition.Y == 0) {
                isBallGoingUp = false;
            } else if (ballPosition.Y == CLIENT_WINDOW_HEIGHT - 1) {
                isBallGoingUp = true;
            }
            if (ballPosition.X == 0) {
                playersScore[1] = playersScore[1] + 1;
                ResetBallPosition();
            } else if (ballPosition.X == CLIENT_WINDOW_WIDTH - 1) {
                playersScore[0] = playersScore[0] + 1;
                ResetBallPosition();
            }
            if (ballPosition.X == players[0].Position.X + 1 &&
                (ballPosition.Y >= players[0].Position.Y && ballPosition.Y <= players[0].Position.Y + 4)
            ) {
                isBallGoingLeft = false;
            } else if (ballPosition.X == players[1].Position.X - 1 &&
                (ballPosition.Y >= players[1].Position.Y && ballPosition.Y <= players[1].Position.Y + 4)
            ) {
                isBallGoingLeft = true;
            }
            if (isBallGoingLeft) {
                ballPosition.X--;
            } else {
                ballPosition.X++;
            }
            if (isBallGoingUp) {
                ballPosition.Y--;
            } else {
                ballPosition.Y++;
            }
            return true;
        }

        public Task<Position> GetBallPositionAsync(CancellationToken cancellationToken = default) {
            return Task.FromResult(ballPosition);
        }

        public Task<PlayerPadPosition> GetLatestPlayerPadPositionAsync(int playerId, CancellationToken cancellationToken = default) {
            if (players.Find(player => player.PlayerId == playerId) == null) {
                throw new PlayerNotFoundException {
                    Message = "There is no player with ID: " + playerId
                };
            }
            return Task.FromResult(players[playerId]);
        }

        public Task<int> GetPlayerScoreAsync(int playerId, CancellationToken cancellationToken = default) {
            if (players.Find(player => player.PlayerId == playerId) == null) {
                throw new PlayerNotFoundException {
                    Message = "There is no player with ID: " + playerId
                };
            }
            return Task.FromResult(playersScore[playerId]);
        }

        public Task<int> JoinGameAsync(CancellationToken cancellationToken = default) {
            var playerId = players.Count;
            players.Add(new PlayerPadPosition {
                PlayerId = playerId,
                Position = new Position {
                    X = 0,
                    Y = 0
                }
            });
            playersScore.Add(0);
            Console.WriteLine(">> Player " + players.Count + " connected.");
            return Task.FromResult(playerId);
        }

        public Task SendPlayerPadPositionAsync(PlayerPadPosition playerPadPosition, CancellationToken cancellationToken = default) {
            int playerId = playerPadPosition.PlayerId;
            players[playerId] = playerPadPosition;
            return Task.CompletedTask;
        }
    }
}

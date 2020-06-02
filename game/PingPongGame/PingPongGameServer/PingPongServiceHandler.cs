using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PingPongGameServer {
    class PingPongServiceHandler : PingPongService.IAsync {
        private const int CLIENT_WINDOW_WIDTH = 100;
        private const int CLIENT_WINDOW_HEIGHT = 30;
        private const int MAX_SCORE = 1;
        public readonly List<PlayerPadPosition> players = new List<PlayerPadPosition>();
        private readonly List<int> playersScore = new List<int>();
        private static Position ballPosition = new Position {
            X = CLIENT_WINDOW_WIDTH / 2,
            Y = CLIENT_WINDOW_HEIGHT / 2
        };
        private static bool isBallGoingUp = true;
        private static bool isBallGoingLeft = true;

        public void Reset() {
            players.Clear();
            playersScore.Clear();
        }

        private void ResetBallPosition() {
            ballPosition.X = CLIENT_WINDOW_WIDTH / 2;
            ballPosition.Y = CLIENT_WINDOW_HEIGHT / 2;
            isBallGoingUp = true;
            isBallGoingLeft = true;
        }

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
            if (players.Count >= playerId + 1) {
                return Task.FromResult(players[playerId]);
            }
            return Task.FromResult<PlayerPadPosition>(null);
        }

        public Task<int> GetPlayerIdAsync(CancellationToken cancellationToken = default) {
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

        public Task<int> GetPlayerScoreAsync(int playerId, CancellationToken cancellationToken = default) {
            return Task.FromResult(playersScore[playerId]);
        }

        public Task IncreaseScoreAsync(int playerId, CancellationToken cancellationToken = default) {
            playersScore[playerId] = playersScore[playerId] + 1;
            return Task.CompletedTask;
        }

        public Task SendPlayerPadPositionAsync(PlayerPadPosition playerPadPosition, CancellationToken cancellationToken = default) {
            int playerId = playerPadPosition.PlayerId;
            players[playerId] = playerPadPosition;
            return Task.CompletedTask;
        }
    }
}

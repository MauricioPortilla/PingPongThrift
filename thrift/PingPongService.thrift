struct Position  {
    1: i32 x;
    2: i32 y;
}

struct PlayerPadPosition {
    1: i32 playerId;
    2: Position position;
}

service PingPongService {
    // Sends self player pad position.
    void SendPlayerPadPosition(1: PlayerPadPosition playerPadPosition);
    // Retrieves latest player pad position.
    PlayerPadPosition GetLatestPlayerPadPosition(1: i32 playerId);
    // Increases player score by 1.
    void IncreaseScore(1: i32 playerId);
    // Retrieves player score.
    i32 GetPlayerScore(1: i32 playerId);
    // Retrieves ball position.
    Position GetBallPosition();
    // Retrieves the assigned self player ID.
    i32 GetPlayerId();
}

/**
 * Represents a position in game.
 */
struct Position {
    1: i32 x;
    2: i32 y;
}

/**
 * Represents a player pad position.
 */
struct PlayerPadPosition {
    1: i32 playerId;
    2: Position position;
}

/**
 * Player with given ID was not found.
 */
exception PlayerNotFoundException {
    1: string message;
}

/**
 * Ping pong service.
 */
service PingPongService {
    /**
     * Sends self player pad position.
     */
    void SendPlayerPadPosition(1: PlayerPadPosition playerPadPosition);

    /**
     * Retrieves latest player pad position. Throws PlayerNotFoundException if there is no a player with given ID.
     */
    PlayerPadPosition GetLatestPlayerPadPosition(1: i32 playerId) throws (1: PlayerNotFoundException playerNotFoundException);

    /**
     * Retrieves player score.
     */
    i32 GetPlayerScore(1: i32 playerId);
    
    /**
     * Retrieves ball position.
     */
    Position GetBallPosition();

    /**
     * Joins to the game and retrieves the assigned self player ID.
     */
    i32 JoinGame();
}

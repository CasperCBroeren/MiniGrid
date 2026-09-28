
# Server
The server is the host to multiple games and is used for persistence (in memory) of the games.
This will host the signalr hub for the clients to connect to and send and receive messages.
The server will be built using the following technologies:
- .NET 10
- SignalR

This is MiniGrid.Server

When a game is started, the game.RunAsync will be called and the action will send the game state to the clients


## SignalR API
- GameHub 
  - StartGame
  - JoinGame(GameCode)
  - LeaveGame
  - PauseGame
  - ResumeGame
  - SetGameSpeed(GameSpeed)   
  - PlayerBuyAsset(Player,Asset)

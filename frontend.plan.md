# Frontend plan
The Minigrid minigame will a client which  connects to a server with websockets.
This is MiniGrid.Frontend
The frontend will be built using the following technologies:
- Vue 3
- Electron
- Typscript
- Vite
- Pinia
- SignalR

## Design
The frontend is primaryly clear colors like
- White
- Light Gray
- Green
- Yellow

## UX
The user will be able to start a new game or join an existing game.
Starting a new game will generate a unique game code that can be shared with other players to join the game. 
Joining an existing game will require the user to enter the game code. The person who starts the game will be the gameleader.
On the server a new instance and a new game will be created. 
The gameleader will be able to start the game once all players have joined. 
The game leader can pause, resume and determine game speed.
Once the game is started, players can no longer join

# Server
The server is the host to multiple games and is used for persistence (in memory) of the games.
This will host the signalr hub for the clients to connect to and send and receive messages.
The server will be built using the following technologies:
- .NET 10
- SignalR

This is MiniGrid.Server
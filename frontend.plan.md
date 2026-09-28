# Frontend plan
The MiniGrid minigame will a client which  connects to a server with websockets.
This is MiniGrid.Frontend
The frontend will be built using the following technologies:
- Vue 3
- Electron
- Typscript
- Vite
- Pinia
- SignalR

## Design
The frontend is light/bright colors like
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
Once the game is started, players can no longer join.
The server instance of the game will be progressed and the players will get updates from the server about the gamestate

When playing the player will see their assets and their clients. Also the data of the current day is shown


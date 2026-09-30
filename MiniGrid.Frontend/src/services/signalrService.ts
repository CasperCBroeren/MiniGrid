import * as signalR from '@microsoft/signalr';
import type {
  CreateGameResponse,
  JoinGameRequest,
  JoinGameResponse,
  StartGameRequest,
  GameStateUpdate,
} from '@/types/game';

class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private serverUrl: string;

  constructor(serverUrl: string = 'https://localhost:5001') {
    this.serverUrl = serverUrl;
  }

  async connect(): Promise<void> {
    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.serverUrl}/gameHub`)
      .withAutomaticReconnect()
      .build();

    try {
      await this.connection.start();
      console.log('SignalR connected');
    } catch (error) {
      console.error('SignalR connection error:', error);
      throw error;
    }
  }

  async disconnect(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
    }
  }

  onGameStateUpdated(callback: (update: GameStateUpdate) => void): void {
    if (this.connection) {
      this.connection.on('GameStateUpdated', callback);
    }
  }

  offGameStateUpdated(callback: (update: GameStateUpdate) => void): void {
    if (this.connection) {
      this.connection.off('GameStateUpdated', callback);
    }
  }

  async createGame(playerName: string): Promise<CreateGameResponse> {
    if (!this.connection) {
      throw new Error('Not connected to SignalR hub');
    }
    return this.connection.invoke('CreateGame', playerName);
  }

  async joinGame(request: JoinGameRequest): Promise<JoinGameResponse> {
    if (!this.connection) {
      throw new Error('Not connected to SignalR hub');
    }
    return this.connection.invoke('JoinGame', request);
  }

  async startGame(request: StartGameRequest): Promise<boolean> {
    if (!this.connection) {
      throw new Error('Not connected to SignalR hub');
    }
    return this.connection.invoke('StartGame', request);
  }

  async pauseGame(gameCode: string, playerId: string, pause: boolean): Promise<boolean> {
    if (!this.connection) {
      throw new Error('Not connected to SignalR hub');
    }
    return this.connection.invoke('PauseGame', { gameCode, playerId, pause });
  }

  async setGameSpeed(gameCode: string, playerId: string, gameSpeed: number): Promise<boolean> {
    if (!this.connection) {
      throw new Error('Not connected to SignalR hub');
    }
    return this.connection.invoke('SetGameSpeed', { gameCode, playerId, gameSpeed });
  }

  getConnectionState(): signalR.HubConnectionState | null {
    return this.connection?.state ?? null;
  }

  isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected;
  }
}

const signalRService = new SignalRService();
export default signalRService;

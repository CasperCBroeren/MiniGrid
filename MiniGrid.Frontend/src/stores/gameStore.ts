import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import signalRService from '@/services/signalrService';
import type {
  CreateGameResponse,
  JoinGameRequest,
  JoinGameResponse,
  GameStateUpdate,
  PlayerState,
} from '@/types/game';

export const useGameStore = defineStore('game', () => {
  const playerName = ref<string>('');
  const gameCode = ref<string>('');
  const playerId = ref<string>('');
  const isGameLeader = ref<boolean>(false);
  const isConnected = ref<boolean>(false);
  const connectionError = ref<string | null>(null);
  const gameState = ref<GameStateUpdate | null>(null);
  const players = ref<PlayerState[]>([]);
  const gameStarted = ref<boolean>(false);

  const hasGameCode = computed(() => gameCode.value.length > 0);
  const canStartGame = computed(() => isGameLeader.value && players.value.length >= 0 && !gameStarted.value);

  async function connectToServer(serverUrl?: string): Promise<void> {
    try {
      connectionError.value = null;
      if (serverUrl) {
        // Recreate service with custom URL
        // For now, we use the default service
      }
      await signalRService.connect();
      isConnected.value = true;
      
      signalRService.onGameStateUpdated((update: GameStateUpdate) => {
        gameState.value = update;
        players.value = update.players;
        gameStarted.value = update.gameState === 'Running';
      });
    } catch (error) {
      connectionError.value = error instanceof Error ? error.message : 'Connection failed';
      isConnected.value = false;
      throw error;
    }
  }

  async function disconnect(): Promise<void> {
    await signalRService.disconnect();
    isConnected.value = false;
    resetGameState();
  }

  async function createGame(name: string): Promise<CreateGameResponse | null> {
    try {
      playerName.value = name;
      const response = await signalRService.createGame(name);
      gameCode.value = response.gameCode;
      playerId.value = response.playerId;
      isGameLeader.value = response.isGameLeader;
      return response;
    } catch (error) {
      console.error('Create game error:', error);
      throw error;
    }
  }

  async function joinGame(request: JoinGameRequest): Promise<JoinGameResponse | null> {
    try {
      playerName.value = request.playerName;
      const response = await signalRService.joinGame(request);
      if (response.success && response.playerId) {
        gameCode.value = request.gameCode;
        playerId.value = response.playerId;
        isGameLeader.value = false;
      }
      return response;
    } catch (error) {
      console.error('Join game error:', error);
      throw error;
    }
  }

  async function startGame(): Promise<boolean> {
    if (!gameCode.value || !playerId.value) {
      return false;
    }
    try {
      const result = await signalRService.startGame({
        gameCode: gameCode.value,
        playerId: playerId.value,
      });
      if (result) {
        // Game has started, update local state
        gameStarted.value = true;
      }
      return result;
    } catch (error) {
      console.error('Start game error:', error);
      return false;
    }
  }

  async function pauseGame(pause: boolean): Promise<boolean> {
    if (!gameCode.value || !playerId.value) {
      return false;
    }
    try {
      return await signalRService.pauseGame(gameCode.value, playerId.value, pause);
    } catch (error) {
      console.error('Pause game error:', error);
      return false;
    }
  }

  async function setGameSpeed(speed: string): Promise<boolean> {
    if (!gameCode.value || !playerId.value) {
      return false;
    }
    try {
      return await signalRService.setGameSpeed(gameCode.value, playerId.value, speed);
    } catch (error) {
      console.error('Set game speed error:', error);
      return false;
    }
  }

  function resetGameState(): void {
    playerName.value = '';
    gameCode.value = '';
    playerId.value = '';
    isGameLeader.value = false;
    gameState.value = null;
    players.value = [];
    gameStarted.value = false;
  }

  return {
    // State
    playerName,
    gameCode,
    playerId,
    isGameLeader,
    isConnected,
    connectionError,
    gameState,
    players,
    gameStarted,
    
    // Computed
    hasGameCode,
    canStartGame,
    
    // Actions
    connectToServer,
    disconnect,
    createGame,
    joinGame,
    startGame,
    pauseGame,
    setGameSpeed,
    resetGameState,
  };
});

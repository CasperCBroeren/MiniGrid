export interface CreateGameResponse {
  gameCode: string;
  playerId: string;
  isGameLeader: boolean;
}

export interface JoinGameRequest {
  gameCode: string;
  playerName: string;
}

export interface JoinGameResponse {
  success: boolean;
  playerId?: string;
  error?: string;
}

export interface StartGameRequest {
  gameCode: string;
  playerId: string;
}

export interface PauseGameRequest {
  gameCode: string;
  playerId: string;
  pause: boolean;
}

export interface SetGameSpeedRequest {
  gameCode: string;
  playerId: string;
  gameSpeed: number;
}

export interface PlayerState {
  playerId: string;
  name: string;
  isGameLeader: boolean;
  cashInEuro: number;
  clients: any[];
  totalProductionKWh: number;
  totalConsumptionKWh: number;
  totalCarbonEmitted: number;
  balance: number;
  assets: string[];
}

export interface GameStateUpdate {
  gameCode: string;
  gameState: string;
  currentDay: number;
  currentHour: number;
  currentMonth: number;
  isPaused: boolean;
  gameSpeed: string;
  players: PlayerState[];
  solarRadiation?: number;
  windSpeed?: number;
}

export interface GamePlayer {
  playerId: string;
  name: string;
  connectionId: string;
  isGameLeader: boolean;
}

export interface ServerGame {
  gameCode: string;
  players: GamePlayer[];
  isPaused: boolean;
  gameSpeed: string;
}

export interface Asset {
  name: string;
  price: number;
}

export interface PlayerAsset extends Asset {
  maxKiloWattHour: number;
}

export interface BuyAssetRequest {
  gameCode: string;
  playerId: string;
  assetType: string;
}

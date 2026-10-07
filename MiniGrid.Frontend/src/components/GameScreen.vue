<script setup lang="ts">
import { computed } from 'vue';
import { useGameStore } from '@/stores/gameStore';

const gameStore = useGameStore();

// Format the current date
const formattedDate = computed(() => {
  const month = gameStore.gameState?.currentMonth ?? 1;
  const day = gameStore.gameState?.currentDay ?? 1;
  const hour = gameStore.gameState?.currentHour ?? 0;
  
  const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 
                     'July', 'August', 'September', 'October', 'November', 'December'];
  const monthName = monthNames[month - 1] || 'January';
  
  return ` ${pad(hour,2)}:00 on ${monthName} ${pad(day,2)}`;
});
function pad (num:number, size:number) {
    let outNum = num.toString();
    while (outNum.length < size) outNum = "0" + outNum;
    return outNum;
}
// Get solar radiation with formatting
const solarRadiation = computed(() => {
  const radiation = gameStore.gameState?.solarRadiation;
  if (radiation === undefined || radiation === null) return 'N/A';
  return radiation.toFixed(1);
});

// Get wind speed with formatting
const windSpeed = computed(() => {
  const speed = gameStore.gameState?.windSpeed;
  if (speed === undefined || speed === null) return 'N/A';
  return speed.toFixed(1);
});

// Game state display
const gameStateDisplay = computed(() => {
 return gameStore.gameState?.gameState;  
});

// Check if game is paused
const isPaused = computed(() => gameStore.gameState?.isPaused || false);

// Current game speed display
const gameSpeedDisplay = computed(() => {
  return gameStore.gameState?.gameSpeed;   
});
</script>

<template>
  <div class="game-screen">
    <!-- Header with game info -->
    <header class="game-header">
      <div class="game-info">
        <h1 class="game-title">MiniGrid</h1>
        <p class="game-code">Game: {{ gameStore.gameCode }}</p>
      </div>
      <div class="controls" v-if="gameStore.isGameLeader">
        <button @click="gameStore.pauseGame(!isPaused)" class="btn" :class="{ 'btn-warning': !isPaused, 'btn-success': isPaused }">
          {{ isPaused ? 'Resume' : 'Pause' }}
        </button>
        <button @click="gameStore.setGameSpeed('Normal')" class="btn btn-secondary" title="Normal">
          Normal
        </button>
        <button @click="gameStore.setGameSpeed('VeryFast')" class="btn btn-secondary" title="Fast">
          Fast
        </button>
        <button @click="gameStore.setGameSpeed('UltraFast')" class="btn btn-secondary" title="Ultra">
          Ultra
        </button>
      </div>
      <div class="game-status">
        <span class="status-badge" :class="{ paused: isPaused, running: !isPaused && gameStateDisplay === 'Running' }">
          {{ gameStateDisplay }}
        </span>
        <span class="speed-badge">{{ gameSpeedDisplay }}</span>
      </div>
    </header>

    <!-- Main Game Display -->
    <main class="game-main">
      
      <div class="game-card">
        
      </div>
 
      <div class="weather-card">
        <h2 class="card-title">Today</h2> 
        <div class="date-display">
          <span class="date-text">{{ formattedDate }}</span>
        </div>
        <div class="weather-grid">
          <div class="weather-item">
            <div class="weather-icon solar">
              ☀️
            </div>
            <div class="weather-value">
              <span class="value">{{ solarRadiation }}</span>
              <span class="unit">W/m²</span>
            </div>
            <span class="weather-label">Solar Radiation</span>
          </div>
          <div class="weather-item">
            <div class="weather-icon wind">
             🌬️
            </div>
            <div class="weather-value">
              <span class="value">{{ windSpeed }}</span>
              <span class="unit">m/s</span>
            </div>
            <span class="weather-label">Wind Speed</span>
          </div>
        </div>

        <h2 class="card-title">Game Stats</h2>
        <div class="stats-grid">
          <div class="stat-item">
            <span class="stat-value">{{ gameStore.players.length }}</span>
            <span class="stat-label">Players</span>
          </div>
          <div class="stat-item">
            <span class="stat-value">€{{ gameStore.players.reduce((sum, p) => sum + (p.cashInEuro || 0), 0) }}</span>
            <span class="stat-label"> Cash</span>
          </div>
          <div class="stat-item">
            <span class="stat-value">{{ gameStore.players.reduce((sum, p) => sum + (p.totalProductionKWh || 0), 0) }} kWh</span>
            <span class="stat-label">Production</span>
          </div>
        </div>
      </div> 
    </main> 
  </div>
</template>

<style scoped>
.game-screen {
  display: flex;
  flex-direction: column;
  /* min-height: 100vh; */
  background: linear-gradient(135deg, #f0f4f8 0%, #e2e8f0 100%);
}

.game-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem 2rem;
  background: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  border-radius: 20px;
  width: 100%;
}

.game-info {
  display: flex;
  flex-direction: column;
}

.game-title {
  font-size: 1.75rem;
  font-weight: 700;
  color: #1a5f3f;
  margin: 0;
}

.game-code {
  font-size: 0.875rem;
  color: #6b7280;
  margin: 0.25rem 0 0 0;
}

.game-status {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.status-badge {
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.875rem;
  font-weight: 600;
}

.status-badge.running {
  background: #d1fae5;
  color: #065f46;
}

.status-badge.paused {
  background: #fef3c7;
  color: #92400e;
}

.speed-badge {
  padding: 0.5rem 1rem;
  background: #e0e7ff;
  color: #3730a3;
  border-radius: 20px;
  font-size: 0.875rem;
  font-weight: 600;
}

.game-main {
  flex: 1;
  display: grid;
  grid-template-columns: 59% 39%;
  gap: 2%;
  padding-top: 2rem;
  margin: 0 auto;
  width: 100%;
}

.card-title {
  font-size: 1rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 1rem;
}

.day-card {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  text-align: center;
  grid-column: span 2;
}

.date-display {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: rem;
}

.date-text {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1a5f3f;
}

.weather-card {
  background: white;
  padding: 1.5rem;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.weather-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;
  margin-bottom: 5%;
}

.weather-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 8px;
}

.weather-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 48px;
  height: 48px;
  border-radius: 12px;
}

.weather-icon.solar {
  background: #fef3c7;
  color: #92400e;
}

.weather-icon.wind {
  background: #dbeafe;
  color: #1e40af;
}

.weather-value {
  display: flex;
  align-items: baseline;
  gap: 0.25rem;
}

.value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1f2937;
}

.unit {
  font-size: 0.875rem;
  color: #6b7280;
}

.weather-label {
  font-size: 0.75rem;
  font-weight: 500;
  color: #6b7280;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.game-card {
  background: white;
  padding: 1.5rem;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.25rem;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 8px;
}

.stat-value {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1a5f3f;
}

.stat-label {
  font-size: 0.75rem;
  font-weight: 500;
  color: #6b7280;
}

.game-footer {
  padding: 1.5rem 2rem;
  background: white;
  border-top: 1px solid #e5e7eb;
}

.controls {
  display: flex;
  justify-content: center;
  gap: 1rem;
  max-width: 400px;
  margin: 0 auto;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-primary {
  background: #1a5f3f;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: #11402a;
}

.btn-secondary {
  background: #f3f4f6;
  color: #374151;
}

.btn-secondary:hover:not(:disabled) {
  background: #e5e7eb;
}

.btn-success {
  background: #059669;
  color: white;
}

.btn-success:hover:not(:disabled) {
  background: #047857;
}

.btn-warning {
  background: #d97706;
  color: white;
}

.btn-warning:hover:not(:disabled) {
  background: #b45309;
}

.btn-danger {
  background: #dc2626;
  color: white;
}

.btn-danger:hover:not(:disabled) {
  background: #b91c1c;
}

@media (max-width: 768px) {
  .game-main {
    grid-template-columns: 1fr;
  }
  
  .day-card {
    grid-column: span 1;
  }
  
  .weather-grid {
    grid-template-columns: 1fr;
  }
  
  .stats-grid {
    grid-template-columns: 1fr;
  }
  
  .date-text {
    font-size: 2rem;
  }
}
</style>

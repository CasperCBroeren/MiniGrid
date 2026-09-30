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
  
  return `${monthName} ${day}, ${hour}:00`;
});

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
  const state = gameStore.gameState?.gameState;
  switch (state) {
    case 'Playing':
      return 'Running';
    case 'Pauzed':
      return 'Paused';
    case 'Ended':
      return 'Game Over';
    default:
      return state || 'Unknown';
  }
});

// Check if game is paused
const isPaused = computed(() => gameStore.gameState?.isPaused || false);

// Current game speed display
const gameSpeedDisplay = computed(() => {
  const speed = gameStore.gameState?.gameSpeed;
  switch (speed) {
    case 1:
      return 'Ultra Fast';
    case 2:
      return 'Very Fast';
    case 3:
      return 'Fast';
    case 208:
      return 'Normal';
    default:
      return `Speed: ${speed}`;
  }
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
      <div class="game-status">
        <span class="status-badge" :class="{ paused: isPaused, running: !isPaused && gameStateDisplay === 'Running' }">
          {{ gameStateDisplay }}
        </span>
        <span class="speed-badge">{{ gameSpeedDisplay }}</span>
      </div>
    </header>

    <!-- Main Game Display -->
    <main class="game-main">
      <!-- Current Day Card -->
      <div class="day-card">
        <h2 class="card-title">Current Day</h2>
        <div class="date-display">
          <span class="date-text">{{ formattedDate }}</span>
        </div>
      </div>

      <!-- Weather Conditions Card -->
      <div class="weather-card">
        <h2 class="card-title">Weather Conditions</h2>
        <div class="weather-grid">
          <div class="weather-item">
            <div class="weather-icon solar">
              <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="currentColor">
                <circle cx="12" cy="12" r="4" />
                <path d="M12 2v2" />
                <path d="M12 20v2" />
                <path d="m4.93 4.93 1.41 1.41" />
                <path d="m17.66 17.66 1.41 1.41" />
                <path d="M2 12h2" />
                <path d="M20 12h2" />
                <path d="m6.34 17.66-1.41 1.41" />
                <path d="m19.07 4.93-1.41 1.41" />
              </svg>
            </div>
            <div class="weather-value">
              <span class="value">{{ solarRadiation }}</span>
              <span class="unit">W/m²</span>
            </div>
            <span class="weather-label">Solar Radiation</span>
          </div>
          <div class="weather-item">
            <div class="weather-icon wind">
              <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="currentColor">
                <path d="M18 10h-6" />
                <path d="M12 6v8" />
                <path d="M21 11H9" />
                <path d="M15 6.5a2.5 2.5 0 1 0 0 5 2.5 2.5 0 0 0 0-5z" />
              </svg>
            </div>
            <div class="weather-value">
              <span class="value">{{ windSpeed }}</span>
              <span class="unit">m/s</span>
            </div>
            <span class="weather-label">Wind Speed</span>
          </div>
        </div>
      </div>

      <!-- Player Stats Preview -->
      <div class="stats-card">
        <h2 class="card-title">Player Stats</h2>
        <div class="stats-grid">
          <div class="stat-item">
            <span class="stat-value">{{ gameStore.players.length }}</span>
            <span class="stat-label">Players</span>
          </div>
          <div class="stat-item">
            <span class="stat-value">€{{ gameStore.players.reduce((sum, p) => sum + (p.cashInEuro || 0), 0) }}</span>
            <span class="stat-label">Total Cash</span>
          </div>
          <div class="stat-item">
            <span class="stat-value">{{ gameStore.players.reduce((sum, p) => sum + (p.totalProductionKWh || 0), 0) }} kWh</span>
            <span class="stat-label">Total Production</span>
          </div>
        </div>
      </div>
    </main>

    <!-- Game Controls Footer -->
    <footer class="game-footer" v-if="gameStore.isGameLeader">
      <div class="controls">
        <button @click="gameStore.pauseGame(!isPaused)" class="btn" :class="{ 'btn-warning': !isPaused, 'btn-success': isPaused }">
          {{ isPaused ? 'Resume' : 'Pause' }}
        </button>
        <button @click="gameStore.setGameSpeed(2)" class="btn btn-secondary" title="Normal Speed">
          Normal
        </button>
        <button @click="gameStore.setGameSpeed(3)" class="btn btn-secondary" title="Fast Speed">
          Fast
        </button>
      </div>
    </footer>
  </div>
</template>

<style scoped>
.game-screen {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background: linear-gradient(135deg, #f0f4f8 0%, #e2e8f0 100%);
}

.game-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem 2rem;
  background: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
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
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
  padding: 2rem;
  max-width: 1200px;
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
  padding: 2rem;
}

.date-text {
  font-size: 3rem;
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

.stats-card {
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

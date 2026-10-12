<template>
  <div class="game-screen">
    <BuyAssetModel v-if="showBuyAssetModal" v-model:selectedAssetType="selectedAssetType" v-model:showBuyAssetModal="showBuyAssetModal"/>

    <!-- Header with game info -->
    <header class="game-header">
      <div class="game-info"> 
        <div class="date-display">
          Today <span class="date-text">{{ formattedDate }}</span>
        </div>
      </div>
      <div class="controls" v-if="gameStore.isGameLeader">     
        <button @click="gameStore.setGameSpeed('Normal')" :class="{'btn btn-secondary': true, 'btn-active': gameSpeedDisplay == 'Normal'}" title="Normal">
          Normal
        </button>
        <button @click="gameStore.setGameSpeed('VeryFast')" :class="{'btn btn-secondary': true, 'btn-active': gameSpeedDisplay == 'VeryFast'}" title="Fast">
          Fast
        </button>
        <button @click="gameStore.setGameSpeed('UltraFast')" :class="{'btn btn-secondary': true, 'btn-active': gameSpeedDisplay == 'UltraFast'}" title="Ultra">
          Ultra
        </button>
        <button @click="gameStore.pauseGame(!isPaused)" class="btn" :class="{ 'btn-warning': !isPaused, 'btn-success': isPaused }">
          {{ isPaused ? 'Resume' : 'Pause' }}
        </button>
      </div>
      <div class="game-status" v-else>
        <span class="status-badge" :class="{ paused: isPaused, running: !isPaused && gameStateDisplay === 'Running' }">
          
        </span>
        <span class="speed-badge">{{ gameSpeedDisplay }}</span>
      </div>
    </header>

    <!-- Main Game Display -->
    <main class="game-main">
      
      <div class="game-card">
        <h2 class="card-title">Company stats</h2>
        <div class="stats-grid">
          <div class="stat-item">
            Production
            <span class="stat-value">{{ currentProduction }} kWh</span> 
          </div>
          <div class="stat-item">
            Consumption
            <span class="stat-value">{{ currentConsumption }} kWh</span> 
          </div>
          <div class="stat-item">
            Current cash
            <span class="stat-value">€ {{ cashInEuro }}</span> 
          </div>
          <div class="stat-item">
            Clients
            <span class="stat-value">{{ currentClients }}</span> 
          </div>
          <div class="stat-item">
            Carbon rate
            <span class="stat-value">{{ currentCarbon }}</span> 
          </div>
        </div>
        <br/>
        <h2 class="card-title">Your Assets</h2>
        <div class="assets-grid">
          <div class="asset-item">
            <div class="asset-icon solar">
              ☀️
            </div>
            <div class="asset-info">
              <span class="asset-name">Solar</span>
              <span class="asset-count">{{ solarAssetCount }}</span>
            </div>
          </div>
          <div class="asset-item">
            <div class="asset-icon wind">
             🌬️
            </div>
            <div class="asset-info">
              <span class="asset-name">Wind</span>
              <span class="asset-count">{{ windAssetCount }}</span>
            </div>
          </div>
        </div>
        
        <button @click="openBuyAssetModal" class="btn btn-primary buy-asset-btn">
          Buy Asset
        </button> 
      </div>
 
      <div class="weather-card"> 
        <h2>Today's weather</h2>       
        <div class="weather-grid">
          <div class="weather-item">
            <div class="weather-icon solar">
            ☀️
            </div>
            <div class="weather-value">
              <span class="value">{{ solarRadiation }}</span>
              <span class="unit">W/m2</span>
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
            <span class="stat-label" v-if="gameStore.players.length == 1">Player</span>
            <span class="stat-label" v-else>Players</span>
          </div>
          <div class="stat-item">
            <span class="stat-value">{{ totalAssetCount }}</span>
            <span class="stat-label" v-if="totalAssetCount == 1">Assets</span>
            <span class="stat-label" v-else>Assets</span>
          </div>
          <div class="stat-item">
            <span class="stat-value">{{ gameStore.players.reduce((sum, p) => sum + (p.totalProductionKWh || 0), 0) }}</span>
            <span class="stat-label">kWh production</span>
          </div>
        </div>
      </div> 
    </main> 
  </div>
</template>
<script setup lang="ts">
import { computed, ref } from 'vue';
import { useGameStore } from '@/stores/gameStore';
import BuyAssetModel from './BuyAssetDialog.vue';

const gameStore = useGameStore();

// State for buy asset modal
const showBuyAssetModal = ref(false);
const selectedAssetType = ref<'Solar' | 'Wind' | null>(null);

// Get current player's assets
const currentPlayer = computed(() => {
  return gameStore.players.find(p => p.playerId === gameStore.playerId);
});

// Get current player's assets
const playerAssets = computed(() => {
  return currentPlayer.value?.assets || [];
});
const cashInEuro = computed(() => {
  return currentPlayer.value?.cashInEuro || 0;
});
const currentClients = computed(() => {
  return currentPlayer.value?.clients || 0;
});
const currentCarbon = computed(() => {
  return currentPlayer.value?.totalCarbonEmitted || 0;
});
const currentProduction = computed(() => {
  return currentPlayer.value?.totalProductionKWh || 0;
});
const currentConsumption = computed(() => {
  return currentPlayer.value?.totalConsumptionKWh || 0;
});
// Count of each asset type
const solarAssetCount = computed(() => {
  return playerAssets.value.filter(a => a === 'Solar').length;
});

const windAssetCount = computed(() => {
  return playerAssets.value.filter(a => a === 'Wind').length;
}); 

 

const totalAssetCount = computed(() => {
 return gameStore.players.reduce((sum, p) => sum + (p.assets.length || 0), 0);
});

// Open buy asset modal
const openBuyAssetModal = () => {
  showBuyAssetModal.value = true;
  selectedAssetType.value = null;
}; 
// Format the current date
const formattedDate = computed(() => {
  const month = gameStore.gameState?.currentMonth ?? 1;
  const day = gameStore.gameState?.currentDay ?? 1;
  const hour = gameStore.gameState?.currentHour ?? 0;
  
  const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 
                     'July', 'August', 'September', 'October', 'November', 'December'];
  const monthName = monthNames[month - 1] || 'January';
  
  return `${pad(day,2)} ${monthName} at ${pad(hour,2)}:00`;
});
function pad(num:(number | string), size:number) {
    let outNum = num.toString();
    while (outNum.length < size) outNum = "0" + outNum;
    return outNum;
}
// Get solar radiation with formatting
const solarRadiation = computed(() => {
  const radiation = gameStore.gameState?.solarRadiation;
  if (radiation === undefined || radiation === null) return 'N/A';
  return pad(radiation.toFixed(1), 1);
});

// Get wind speed with formatting
const windSpeed = computed(() => {
  const speed = gameStore.gameState?.windSpeed;
  if (speed === undefined || speed === null) return 'N/A';
  return pad(speed.toFixed(1), 1);
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
  
.game-status {
  display: flex;
  gap: 1rem;
  align-items: center;
  align-self: center;
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
  min-width: 30rem;
  font-size: 1.25rem;
  font-weight: 700;
  color: #1a5f3f;
  padding-left: 0.325rem;
  font-variant-numeric: tabular-nums; 
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
  font-variant-numeric: tabular-nums; 
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
  font-variant-numeric: tabular-nums; 
}

.stat-label {
  font-size: 0.7rem;
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
  align-self: center;
}

 
/* Asset styles */
.assets-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.asset-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 8px;
}

.asset-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 10px;
  font-size: 1.25rem;
}

.asset-icon.solar {
  background: #fef3c7;
  color: #92400e;
}

.asset-icon.wind {
  background: #dbeafe;
  color: #1e40af;
}

.asset-info {
  display: flex;
  flex-direction: column;
}

.asset-name {
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
}

.asset-count {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1a5f3f;
}

.buy-asset-btn {
  width: 100%;
  margin-top: 1rem;
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
  
}
</style>

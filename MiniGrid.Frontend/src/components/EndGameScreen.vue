<template>
  <div class="end-game-screen">
    <div class="end-game-container">
      <h1 class="end-game-title">Game Over!</h1>
      <p class="end-game-subtitle">Final Rankings</p>

      <div class="ranking-table-container">
        <table class="ranking-table">
          <thead>
            <tr>
              <th class="rank-header">Rank</th>
              <th class="player-header">Energy company</th>
              <th class="carbon-header">Carbon Rate</th>
              <th class="production-header">Production</th>
              <th class="cash-header">Cash</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(player, index) in rankedPlayers" :key="player.playerId" class="player-row" :class="{ 'winner': index === 0, 'second': index === 1, 'third': index === 2 }">
              <td class="rank-cell">{{ index + 1 }}</td>
              <td class="player-cell">{{ player.name }}</td>
              <td class="carbon-cell">{{ player.totalCarbonEmitted }}</td>
              <td class="production-cell">{{ formatWh(player.totalProductionKWh) }}</td>
              <td class="cash-cell">{{ formatEuro(player.cashInEuro) }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="end-game-actions">
        <button @click="handleBackToLobby" class="btn btn-primary">
          Back to Lobby
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useGameStore } from '@/stores/gameStore';
import { formatWh, formatEuro } from './numberformaters.ts';

const gameStore = useGameStore();

// Compute ranked players
const rankedPlayers = computed(() => {
  const players = gameStore.players;
  
  // Sort by: lowest carbon rate (ascending), then highest production (descending), then highest cash (descending)
  return [...players].sort((a, b) => {
    // First: lowest carbon rate wins
    if (a.totalCarbonEmitted !== b.totalCarbonEmitted) {
      return a.totalCarbonEmitted - b.totalCarbonEmitted;
    }
    // Second: highest production wins
    if (a.totalProductionKWh !== b.totalProductionKWh) {
      return b.totalProductionKWh - a.totalProductionKWh;
    }
    // Third: highest cash wins
    return b.cashInEuro - a.cashInEuro;
  });
});

function handleBackToLobby() {
  gameStore.resetGameState();
}
</script>

<style scoped>
.end-game-screen {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 70vh;
  padding: 2rem;
  background: linear-gradient(135deg, #f0f4f8 0%, #e2e8f0 100%);
}

.end-game-container {
  background: white;
  padding: 3rem;
  border-radius: 20px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
  max-width: 800px;
  width: 100%;
  text-align: center;
}

.end-game-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: #1a5f3f;
  margin-bottom: 0.5rem;
}

.end-game-subtitle {
  font-size: 1.25rem;
  color: #6b7280;
  margin-bottom: 2rem;
}

.ranking-table-container {
  width: 100%;
  overflow-x: auto;
}

.ranking-table {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 2rem;
}

.ranking-table th,
.ranking-table td {
  padding: 1rem;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

.ranking-table th {
  font-size: 0.875rem;
  font-weight: 600;
  color: #6b7280;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  background: #f9fafb;
}

.ranking-table td {
  font-size: 1rem;
  color: #1f2937;
}

.player-row {
  transition: background-color 0.2s;
}

.player-row:hover {
  background: #f9fafb;
}

.player-row.winner {
  background: #d1fae5 !important;
}

.player-row.second {
  background: #dbeafe !important;
}

.player-row.third {
  background: #fef3c7 !important;
}

.rank-cell {
  font-weight: 700;
  color: #1a5f3f;
  text-align: center;
}

.player-cell {
  font-weight: 600;
  color: #1f2937;
}

.carbon-cell,
.production-cell,
.cash-cell {
  font-variant-numeric: tabular-nums;
}

.carbon-cell {
  color: #6b7280;
}

.production-cell {
  color: #059669;
  font-weight: 500;
}

.cash-cell {
  color: #d97706;
  font-weight: 500;
}

.end-game-actions {
  display: flex;
  justify-content: center;
  gap: 1rem;
}

.btn {
  padding: 0.75rem 2rem;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary {
  background: #1a5f3f;
  color: white;
}

.btn-primary:hover {
  background: #11402a;
}

@media (max-width: 600px) {
  .ranking-table th,
  .ranking-table td {
    padding: 0.75rem 0.5rem;
    font-size: 0.875rem;
  }
  
  .end-game-title {
    font-size: 2rem;
  }
  
  .end-game-subtitle {
    font-size: 1rem;
  }
  
  .end-game-container {
    padding: 2rem 1rem;
  }
}
</style>

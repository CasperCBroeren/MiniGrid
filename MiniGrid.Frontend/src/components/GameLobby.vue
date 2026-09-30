<template> 
      <!-- Connection Settings -->
      <div class="connection-settings" v-if="false">
        <label class="input-label">
          Server URL:
          <input v-model="serverUrl" type="text" class="input-field" placeholder="http://localhost:5000" />
        </label>
        <button @click="gameStore.connectToServer(serverUrl)" class="btn btn-secondary" :disabled="gameStore.isConnected">
          {{ gameStore.isConnected ? 'Connected' : 'Connect' }}
        </button>
      </div>

      <!-- Game Code Display (if in a game) -->
      <div v-if="hasGameCode" class="game-info-card">
        <div class="game-code-display">
          <span class="game-code-label">Game Code:</span>
          <span class="game-code">{{ gameStore.gameCode }}</span>
          <button @click="handleCopyGameCode" class="btn btn-icon" title="Copy to clipboard">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="9" y="9" width="13" height="13" rx="2" />
              <path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1" />
            </svg>
          </button>
        </div>
        <div class="player-badge" :class="{ leader: isGameLeader }">
          {{ isGameLeader ? 'Game Leader' : 'Player' }}: {{ gameStore.playerName || nameInput }}
        </div>
      </div>

      <!-- Tab Navigation -->
      <div v-if="!hasGameCode" class="tab-container">
        <button @click="activeTab = 'create'" class="tab-btn" :class="{ active: activeTab === 'create' }">
          Create Game
        </button>
        <button @click="activeTab = 'join'" class="tab-btn" :class="{ active: activeTab === 'join' }">
          Join Game
        </button>
      </div>

      <!-- Create Game Tab -->
      <div v-if="activeTab === 'create' && !hasGameCode" class="form-container">
        <form @submit.prevent="handleCreateGame" class="form">
          <label class="input-label">
            Your Name:
            <input v-model="nameInput" type="text" class="input-field" placeholder="Enter your name" required />
          </label>
          <button type="submit" class="btn btn-primary btn-full">
            Create New Game
          </button>
        </form>
      </div>

      <!-- Join Game Tab -->
      <div v-if="activeTab === 'join' && !hasGameCode" class="form-container">
        <form @submit.prevent="handleJoinGame" class="form">
          <label class="input-label">
            Your Name:
            <input v-model="nameInput" type="text" class="input-field" placeholder="Enter your name" required />
          </label>
          <label class="input-label">
            Game Code:
            <input v-model="gameCodeInput" type="text" class="input-field" placeholder="Enter game code" required />
          </label>
          <p v-if="joinError" class="error-message">{{ joinError }}</p>
          <button type="submit" class="btn btn-primary btn-full">
            Join Game
          </button>
        </form>
      </div>

      <!-- Player List (if in a game) -->
      <div v-if="hasGameCode" class="players-section">
        <h3 class="section-title">Players in Lobby</h3>
        <div class="players-list">
          <div v-for="player in playerList" :key="player.playerId" class="player-item">
            <span class="player-name">{{ player.name }}</span>
            <span v-if="player.isGameLeader" class="leader-badge">Leader</span>
          </div>
          <p v-if="playerList.length === 0" class="empty-message">No players yet</p>
        </div>
      </div>

      <!-- Game Controls (if game leader) -->
      <div v-if="hasGameCode && isGameLeader" class="game-controls">
        <h3 class="section-title">Game Controls</h3>
        <div class="controls-grid">
          <button @click="handleStartGame" class="btn btn-success" :disabled="!canStartGame || gameStore.gameStarted">
            {{ gameStore.gameStarted ? 'Game Running' : 'Start Game' }}
          </button>
          <button @click="handlePauseGame" class="btn btn-warning" :disabled="!gameStore.gameStarted">
            {{ gameStore.gameState?.isPaused ? 'Resume' : 'Pause' }}
          </button>
        </div>
        <div v-if="gameStore.gameStarted" class="speed-control">
          <label class="input-label">
            Game Speed:
            <select v-model="selectedSpeed" @change="handleSetGameSpeed" class="input-field" :disabled="!gameStore.gameStarted">
              <option v-for="speed in gameSpeeds" :key="speed.value" :value="speed.value">
                {{ speed.label }}
              </option>
            </select>
          </label>
        </div>
      </div>

      <!-- Reset Button -->
      <div v-if="hasGameCode" class="reset-section">
        <button @click="handleReset" class="btn btn-danger">
          Leave Game
        </button>
      </div> 
 
</template>
<script setup lang="ts">
    import { ref, onMounted, onUnmounted, computed } from 'vue';
    import { useGameStore } from '@/stores/gameStore';

    const gameStore = useGameStore();

    // Form inputs
    const nameInput = ref<string>('');
    const gameCodeInput = ref<string>('');
    const joinError = ref<string>('');

    // Tabs
    const activeTab = ref<'create' | 'join'>('create');

    // Game speed options
    const gameSpeeds = [
        { label: 'Slow', value: 1 },
        { label: 'Normal', value: 2 },
        { label: 'Fast', value: 3 },
    ];
    const selectedSpeed = ref<number>(2);
      

    // Player list
    const playerList = computed(() => gameStore.players);
    const isGameLeader = computed(() => gameStore.isGameLeader);
    const hasGameCode = computed(() => gameStore.hasGameCode);
    const canStartGame = computed(() => gameStore.canStartGame);

  

    // Methods
    async function handleCreateGame() {
        if (!nameInput.value.trim()) {
            return;
        }
        try {
            await gameStore.createGame(nameInput.value.trim());
            joinError.value = '';
        } catch (error) {
            joinError.value = error instanceof Error ? error.message : 'Failed to create game';
        }
    }

    async function handleJoinGame() {
        if (!nameInput.value.trim() || !gameCodeInput.value.trim()) {
            joinError.value = 'Please enter both name and game code';
            return;
        }
        try {
            const response = await gameStore.joinGame({
                gameCode: gameCodeInput.value.trim().toUpperCase(),
                playerName: nameInput.value.trim(),
            });
            if (!response?.success) {
                joinError.value = response?.error || 'Failed to join game';
            } else {
                joinError.value = '';
            }
        } catch (error) {
            joinError.value = error instanceof Error ? error.message : 'Failed to join game';
        }
    }

    async function handleStartGame() {
        try {
            await gameStore.startGame();
            console.log("GameStarted");
        } catch (error) {
            console.error('Failed to start game:', error);
        }
    }

    async function handlePauseGame() {
        try {
            await gameStore.pauseGame(!gameStore.gameState?.isPaused);
        } catch (error) {
            console.error('Failed to pause game:', error);
        }
    }

    async function handleSetGameSpeed() {
        try {
            await gameStore.setGameSpeed(selectedSpeed.value);
        } catch (error) {
            console.error('Failed to set game speed:', error);
        }
    }

    function handleCopyGameCode() {
        if (gameStore.gameCode) {
            navigator.clipboard.writeText(gameStore.gameCode);
        }
    }

    function handleReset() {
        gameStore.resetGameState();
        nameInput.value = '';
        gameCodeInput.value = '';
        joinError.value = '';
    }
</script>

<style scoped> 

.connection-settings {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.tab-container {
  display: flex;
  gap: 0.5rem;
  background: white;
  padding: 0.25rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.tab-btn {
  flex: 1;
  padding: 0.75rem 1rem;
  border: none;
  background: transparent;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 500;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s;
}

.tab-btn:hover {
  color: #1a5f3f;
}

.tab-btn.active {
  background: white;
  color: #1a5f3f;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

.form-container {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.input-label {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
}

.input-field {
  padding: 0.75rem 1rem;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 1rem;
  transition: border-color 0.2s, box-shadow 0.2s;
}

.input-field:focus {
  outline: none;
  border-color: #1a5f3f;
  box-shadow: 0 0 0 3px rgba(26, 95, 63, 0.1);
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-full {
  width: 100%;
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

.btn-icon {
  padding: 0.5rem;
  background: #f3f4f6;
}

.btn-icon:hover {
  background: #e5e7eb;
}

.game-info-card {
  background: white;
  padding: 1.5rem;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.game-code-display {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  background: #f0fdf4;
  border-radius: 8px;
}

.game-code-label {
  font-size: 0.875rem;
  font-weight: 500;
  color: #166534;
}

.game-code {
  font-size: 1.5rem;
  font-weight: 700;
  color: #166534;
  letter-spacing: 0.1em;
}

.player-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: #e5e7eb;
  border-radius: 20px;
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
}

.player-badge.leader {
  background: #d1fae5;
  color: #065f46;
}

.players-section {
  background: white;
  padding: 1.5rem;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.section-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 1rem;
}

.players-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.player-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  background: #f9fafb;
  border-radius: 8px;
}

.player-name {
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
}

.leader-badge {
  padding: 0.25rem 0.5rem;
  background: #d97706;
  color: white;
  border-radius: 4px;
  font-size: 0.75rem;
  font-weight: 600;
}

.empty-message {
  text-align: center;
  color: #9ca3af;
  font-size: 0.875rem;
  padding: 1rem;
}

.game-controls {
  background: white;
  padding: 1.5rem;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.controls-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
  gap: 1rem;
  margin-bottom: 1rem;
}

.speed-control {
  margin-top: 1rem;
}

.reset-section {
  display: flex;
  justify-content: center;
}

.error-message {
  color: #dc2626;
  font-size: 0.875rem;
  margin: 0;
}


</style>

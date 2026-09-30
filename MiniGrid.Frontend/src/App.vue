<script setup lang="ts">
    import { computed, onMounted, onUnmounted, ref } from 'vue';
    import { useGameStore } from '@/stores/gameStore';
    import GameLobby from '@/components/GameLobby.vue';
    import GameScreen from '@/components/GameScreen.vue';

    const serverUrl = ref<string>('https://localhost:5001');
    const gameStore = useGameStore();

    // Show GameLobby when game hasn't started or is ended
    // Show GameScreen when game is running
    const showGameLobby = computed(() => {
        const state = gameStore.gameState?.gameState;
        return !state || state === 'NotStarted' || state === 'Ended';
    });

    const showGameScreen = computed(() => {
        const state = gameStore.gameState?.gameState;
        return state === 'Playing' || state === 'Pauzed';
    });

    // Connection status
    const statusMessage = computed(() => {
        if (gameStore.connectionError) {
            return `Error: ${gameStore.connectionError}`;
        }
        if (gameStore.isConnected) {
            return 'Connected to server';
        }
        return 'Disconnected';
    });
    // Lifecycle
    onMounted(async () => {
        try {
            await gameStore.connectToServer(serverUrl.value);
        } catch (error) {
            console.error('Failed to connect on mount:', error);
        }
    });

    onUnmounted(() => {
        gameStore.disconnect();
    });
</script>

<template>
    <div class="container">
        <!-- Header -->
        <header class="header">
            <h1 class="title">MiniGrid</h1>
            <div class="status-bar" :class="{ connected: gameStore.isConnected, disconnected: !gameStore.isConnected }">
                {{ statusMessage }}
            </div>
        </header>

        <!-- Main Content -->
        <main class="main-content">
            <GameLobby v-if="showGameLobby" />
            <GameScreen v-if="showGameScreen" />
        </main>

        <!-- Footer -->
        <footer class="footer">
            <p>MiniGrid - A strategic energy management game</p>
        </footer>
    </div>
</template>

<style scoped>
    .container {
        display: flex;
        flex-direction: column;
        min-height: 100vh;
        background: linear-gradient(135deg, #f0f4f8 0%, #e2e8f0 100%);
    }

    .main-content {
        flex: 1;
        display: flex;
        flex-direction: column;
        gap: 1.5rem;
        padding: 2rem;
        max-width: 1200px;
        margin: 0 auto;
        width: 100%;
    }

    .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 1.5rem 2rem;
        background: white;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .title {
        font-size: 1.75rem;
        font-weight: 700;
        color: #1a5f3f;
        margin: 0;
    }

    .status-bar {
        padding: 0.5rem 1rem;
        border-radius: 20px;
        font-size: 0.875rem;
        font-weight: 500;
    }

        .status-bar.connected {
            background: #d1fae5;
            color: #065f46;
        }

        .status-bar.disconnected {
            background: #fee2e2;
            color: #991b1b;
        }

    .footer {
        text-align: center;
        padding: 1.5rem;
        color: #6b7280;
        font-size: 0.875rem;
    }
</style>

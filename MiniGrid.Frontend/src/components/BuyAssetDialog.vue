<template>
     <!-- Buy Asset Modal -->
    <div class="modal-overlay" @click="closeBuyAssetModal">
      <div class="modal-content" @click.stop>
        <h3>Buy Asset</h3>
        <p>Choose an asset type to purchase:</p>
        <div class="modal-buttons">
          <button @click="buyAsset('Solar')" class="btn btn-primary">
            Solar ☀️
          </button>
          <button @click="buyAsset('Wind')" class="btn btn-primary" >
            Wind 🌬️
          </button>
          <button @click="closeBuyAssetModal" class="btn btn-secondary">
            Cancel
          </button>
        </div>
               <!-- Error message display -->
        <div v-if="gameStore.buyAssetError" class="error-message">
          {{ gameStore.buyAssetError }}
        </div>
      </div>
    </div>
</template>
<script lang="ts" setup> 
import { useGameStore } from '@/stores/gameStore';

const selectedAssetType = defineModel('selectedAssetType');
const showBuyAssetModal = defineModel('showBuyAssetModal');
const gameStore = useGameStore();

// Close buy asset modal
const closeBuyAssetModal = () => {
  showBuyAssetModal.value = false;
  selectedAssetType.value = null;
};
// Buy asset function
const buyAsset = async (assetType: 'Solar' | 'Wind') => {
  selectedAssetType.value = assetType;
  const result = await gameStore.buyAsset(assetType);
  if (result) {
    showBuyAssetModal.value = false;
    selectedAssetType.value = null;
  }
};

</script>
<style lang="css" scoped>

/* Modal styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 20px 25px rgba(0, 0, 0, 0.1);
  min-width: 640px;
  min-height: 380px;
  text-align: center;
}

.modal-content h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1a5f3f;
  margin-bottom: 1rem;
}

.modal-content p {
  font-size: 0.875rem;
  color: #6b7280;
  margin-bottom: 1.5rem;
}

.modal-buttons {
  display: flex;
  flex-direction: column;  
  gap: 0.75rem;
}

.modal-buttons .btn {
  align-self: center;
  width: 80%;
}

.error-message {
  margin-top: 1rem;
  padding: 0.75rem 1rem;
  background: #fef2f2;
  color: #b91c1c;
  border-radius: 6px;
  font-size: 0.875rem;
  border: 1px solid #fecaca;
}

</style>
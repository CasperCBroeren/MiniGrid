<template>
    <!-- Buy Asset Modal -->
     
    <div class="modal-overlay" @click="closeBuyAssetModal">
      <div class="modal-content" @click.stop>
        <h3>Buy Energy Asset</h3>
        <p>Choose an asset to purchase for your energy portfolio:</p>
        
        <div class="assets-grid">
          <div 
            v-for="asset in assets" 
            :key="asset.id" 
            class="asset-card" 
            :class="{ 'selected': selectedAssetId === asset.id }"
            @click="selectAsset(asset.id)"
          >
            <!-- Asset Image -->
            <div class="asset-image-container">
              <img :src="path(asset.image)" :alt="asset.name" class="asset-image" />
            </div>
            
            <!-- Asset Title -->
            <h4 class="asset-title">{{ asset.name }}</h4>
            
            <!-- Asset Cost -->
            <div class="asset-detail">
              <span class="detail-label">Cost:</span>
              <span class="detail-value">{{ formatEuro(asset.price) }}</span>
            </div>
            
            <!-- Carbon Rate -->
            <div class="asset-detail">
              <span class="detail-label">Carbon Rate:</span>
              <span class="detail-value">{{ asset.carbonScore }} kg CO2/MWh</span>
            </div>
            
            <!-- Ramp Rate -->
            <div class="asset-detail">
              <span class="detail-label">Ramp Rate:</span>
              <span class="detail-value">{{ asset.rampRateInHours }} hour(s)</span>
            </div>
            
            <!-- Maximum kWh -->
            <div class="asset-detail">
              <span class="detail-label">Max Output:</span>
              <span class="detail-value">{{ asset.maxKiloWattHour }} kWh</span>
            </div>
            
            <!-- Fuel Cost per Day -->
            <div class="asset-detail">
              <span class="detail-label">Fuel Cost/Day:</span>
              <span class="detail-value">{{ formatEuro(asset.fuelCostPerDay) }}</span>
            </div>
            
            <!-- Description -->
            <p class="asset-description">{{ asset.description }}</p>
            
            <!-- Select Button -->
            <button 
              @click.stop="buyAsset(asset.id)" 
              class="btn btn-buy"
              :disabled="gameStore.buyAssetError !== null"
            >
              Buy {{ asset.name }}
            </button>
          </div>
        </div>
        
        <!-- Error message display -->
        <div v-if="gameStore.buyAssetError" class="error-message">
          {{ gameStore.buyAssetError }}
        </div>
        
        <!-- Cancel Button -->
        <div class="modal-actions">
          <button @click="closeBuyAssetModal" class="btn btn-secondary">
            Cancel
          </button>
        </div>
      </div>
    </div>
 
</template>

<script lang="ts" setup> 
import { ref } from 'vue';
import { useGameStore } from '@/stores/gameStore'; 
import { formatEuro } from '@/components/numberformaters';

const selectedAssetType = defineModel('selectedAssetType');
const showBuyAssetModal = defineModel('showBuyAssetModal');
const gameStore = useGameStore();

const selectedAssetId = ref<string | null>(null);
const assets = await gameStore.getAssetTypes();

function path(loc:string) { 
  return new URL(loc, import.meta.url).toString();
}

// Select an asset
const selectAsset = (assetId: string) => {
  selectedAssetId.value = assetId;
};

// Close buy asset modal
const closeBuyAssetModal = () => {
  showBuyAssetModal.value = false;
  selectedAssetType.value = null;
  selectedAssetId.value = null;
};

// Buy asset function
const buyAsset = async (assetId: string) => {
  selectedAssetType.value = assetId;
  selectedAssetId.value = assetId;
  const result = await gameStore.buyAsset(assetId);
  if (result) {
    showBuyAssetModal.value = false;
    selectedAssetType.value = null;
    selectedAssetId.value = null;
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
  min-width: 900px;
  max-width: 90vw;
  max-height: 90vh;
  overflow-y: auto;
  text-align: center;
}

.modal-content h3 {
  font-size: 1.5rem;
  font-weight: 600;
  color: #1a5f3f;
  margin-bottom: 0.5rem;
}

.modal-content p {
  font-size: 0.9rem;
  color: #6b7280;
  margin-bottom: 1.5rem;
}

/* Assets Grid */
.assets-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.5rem;
  margin-bottom: 1.5rem;
}

/* Asset Card */
.asset-card {
  background: #f8fafc;
  border: 2px solid #e2e8f0;
  border-radius: 12px;
  padding: 1.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
  text-align: center;
  width: 300px;
}

.asset-card:hover {
  border-color: #1a5f3f;
  box-shadow: 0 4px 12px rgba(26, 95, 63, 0.15);
  transform: translateY(-2px);
}

.asset-card.selected {
  border-color: #1a5f3f;
  background: #f0fdf4;
}

/* Asset Image */
.asset-image-container {
  width: 100%;
  height: 150px;
  overflow: hidden;
  border-radius: 8px;
  margin-bottom: 1rem;
  background: white;
}

.asset-image {
  width: 100%;
  height: 100%;
  object-fit: contain;
  background: linear-gradient(135deg, #e0f7ff 0%, #f0fff0 100%);
}

/* Asset Title */
.asset-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1a5f3f;
  margin-bottom: 1rem;
}

/* Asset Details */
.asset-detail {
  display: flex;
  justify-content: space-between;
  padding: 0.5rem 0;
  border-bottom: 1px solid #e2e8f0;
  font-size: 0.875rem;
}

.asset-detail:last-of-type {
  border-bottom: none;
}

.detail-label {
  color: #6b7280;
  font-weight: 500;
}

.detail-value {
  color: #1f2937;
  font-weight: 600;
}

/* Asset Description */
.asset-description {
  font-size: 0.8rem;
  color: #6b7280;
  margin: 1rem 0;
  font-style: italic;
  padding: 0.5rem;
  background: rgba(26, 95, 63, 0.05);
  border-radius: 6px;
  min-height: 75px;
}

/* Buy Button */
.btn-buy {
  background: linear-gradient(135deg, #1a5f3f 0%, #2d7a4f 100%);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 8px;
  font-size: 0.9rem;
  font-weight: 600;
  margin-top: 1rem;
  width: 100%;
  transition: all 0.3s ease;
}

.btn-buy:hover:not(:disabled) {
  background: linear-gradient(135deg, #2d7a4f 0%, #1a5f3f 100%);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(26, 95, 63, 0.3);
}

.btn-buy:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Modal Actions */
.modal-actions {
  display: flex;
  justify-content: center;
  gap: 1rem;
  margin-top: 1rem;
}

.modal-actions .btn {
  min-width: 120px;
}

/* Error message */
.error-message {
  margin: 1rem 0;
  padding: 0.75rem 1rem;
  background: #fef2f2;
  color: #b91c1c;
  border-radius: 6px;
  font-size: 0.875rem;
  border: 1px solid #fecaca;
}

</style>

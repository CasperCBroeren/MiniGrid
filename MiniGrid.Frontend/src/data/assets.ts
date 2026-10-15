import type { AssetConfig } from '@/types/game';

export const ASSET_CONFIGS: AssetConfig[] = [
  {
    id: 'Solar',
    title: 'Solar Park',
    cost: 100000,
    carbonRate: 0,
    rampRateInHours: 1,
    maxKWh: 11,
    fuelCostPerDay: 0,
    imagePath: new URL('./assets/solar_park.svg', import.meta.url).toString(),
    description: 'Clean renewable energy from sunlight. Zero emissions and low maintenance.'
  },
  {
    id: 'Wind',
    title: 'Wind Farm',
    cost: 300000,
    carbonRate: 0,
    rampRateInHours: 1,
    maxKWh: 9,
    fuelCostPerDay: 0,
    imagePath: new URL('./assets/wind_park.svg', import.meta.url).toString(),
    description: 'Harness the power of wind. Reliable and sustainable energy source.'
  }
];

export function getAssetConfig(assetId: string): AssetConfig | undefined {
  return ASSET_CONFIGS.find(config => config.id === assetId);
}

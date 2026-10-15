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
    imagePath: new URL('../assets/solar_park.png', import.meta.url).toString(),
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
    imagePath: new URL('../assets/wind_park.png', import.meta.url).toString(),
    description: 'Harness the power of wind all day, every season. Reliable and sustainable energy source.'
  },
  {
    id: 'Gas',
    title: 'Gas plant',
    cost: 200000,
    carbonRate: 0,
    rampRateInHours: 2,
    maxKWh: 6,
    fuelCostPerDay: 2,
    imagePath: new URL('../assets/gas_powerPlant.png', import.meta.url).toString(),
    description: 'Burn reliable and somewhat clean fuel'
  }
];

export function getAssetConfig(assetId: string): AssetConfig | undefined {
  return ASSET_CONFIGS.find(config => config.id === assetId);
}

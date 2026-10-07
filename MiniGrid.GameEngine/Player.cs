using MiniGrid.GameEngine.Assets;

namespace MiniGrid.GameEngine;

public class Player
{
    public List<EnergyAssets> Assets { get; private set; } = [];
    public int CashInEuro { get; set; }
    public int Clients { get; set; } =5000;
    public int TotalProductionKWh { get; internal set; }
    public int TotalCarbonEmitted { get; internal set; }
    public int Balance { get; internal set; }
    public int TotalConsumptionKWh { get; internal set; }

    public bool GameLeader { get; set; }

    public bool BuyAsset(EnergyAssets asset)
    {
        if (asset.Price > CashInEuro)
        {
            return false;
        }
        CashInEuro -= asset.Price;
        Assets.Add(asset);
        return true;
    }
}


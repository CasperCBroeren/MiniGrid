namespace MiniGrid.Server.Dto;
    public class BuyAssetRequest
    {
        public string GameCode { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public string AssetType { get; set; }
}

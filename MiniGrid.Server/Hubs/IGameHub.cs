namespace MiniGrid.Server.Hubs;
    public interface IGameHub
    {
        void BroadcastGameState(string gameCode);
    }

namespace CoveoBlitz
{
    public interface ISimpleBot
    {
        void Setup(GameState state);

        void Shutdown();

        string Move(GameState state);
    }
}

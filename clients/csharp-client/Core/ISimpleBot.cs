namespace CoveoBlitz
{
    public interface ISimpleBot
    {
        void Setup(GameState state);

        void Shutdown(GameState state);

        string Move(GameState state);
    }
}

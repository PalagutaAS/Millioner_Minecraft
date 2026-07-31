using Cysharp.Threading.Tasks;

public interface IGameState
{
    GameState StateId { get; }
    UniTask Enter();
    UniTask Exit();
}

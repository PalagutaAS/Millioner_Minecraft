using UnityEngine;
using VContainer;
using YG;

public class LeaderboardNameSetter : MonoBehaviour
{
    [SerializeField] private LeaderboardYG _leaderboardYg;
    private GameConfig _gameConfig;

    [Inject]
    private void Construct(GameConfig gameConfig)
    {
        _gameConfig = gameConfig;
        _leaderboardYg.nameLB = _gameConfig.NameLB;
    }
    
}

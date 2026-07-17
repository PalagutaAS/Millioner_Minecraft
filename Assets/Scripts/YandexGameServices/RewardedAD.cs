using System;

namespace YG
{
    public class RewardedAD
    {
        private readonly string _rewardID;
        
        public RewardedAD(GameConfig gameConfig)
        {
            _rewardID = gameConfig.RewardID;
        }

        public void RewardedAdvShow(Action action)
        {
            YG2.RewardedAdvShow(_rewardID, action);
        }
    }
}
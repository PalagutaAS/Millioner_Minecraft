using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HintButtonWithAlert : HintButton
    {
        [SerializeField] private Button _additionalButton;
        
        public override void SetInteractable(bool interactable)
        {
            _additionalButton.interactable = interactable;
            base.SetInteractable(interactable);
        }
    }
}
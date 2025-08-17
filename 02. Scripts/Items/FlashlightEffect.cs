using UnityEngine;

namespace _02._Scripts.Items
{
    public class FlashlightEffect : IItemEffect
    {
        public void Use(out bool canUse)
        {
            PlayerController.Instance.FlashlightController.ToggleFlashlight();
            canUse = true;
        }
    }
}
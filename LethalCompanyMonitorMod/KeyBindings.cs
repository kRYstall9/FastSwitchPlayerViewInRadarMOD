using LethalCompanyInputUtils.Api;
using LethalCompanyInputUtils.BindingPathEnums;
using UnityEngine.InputSystem;

namespace LethalCompanyMonitorMod
{
    public class KeyBindings : LcInputActions
    {
        [InputAction(KeyboardControl.LeftArrow, Name = "Previous Player Cam")]
        public InputAction PreviousPlayerCamKey { get; set; }
        [InputAction(KeyboardControl.RightArrow, Name = "Next Player Cam")]
        public InputAction NextPlayerCamKey { get; set; }

    }
}

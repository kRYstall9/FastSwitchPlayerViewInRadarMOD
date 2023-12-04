using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace LethalCompanyMonitorMod
{
    public class Keybindings
    {
        //Allowed keys. At most 10 players per lobby
        public static Dictionary<int, Key> allowedKeys = new Dictionary<int, Key>()
        {
            {0, Key.Digit0 },
            {1, Key.Digit1 },
            {2, Key.Digit2 },
            {3, Key.Digit3 },
            {4, Key.Digit4 },
            {5, Key.Digit5 },
            {6, Key.Digit6 },
            {7, Key.Digit7 },
            {8, Key.Digit8 },
            {9, Key.Digit9 },
        };
    }
}

using TMPro;
using UnityEngine;

namespace SpaceShooter
{
    namespace SS_Manager
    {
        public class SS_Level_UIManager : Manager.UIManager_Level
        {
            [SerializeField] private TextMeshProUGUI countCurrentKillContainer;

            public void ShowCurrentKill(int kill, int needed)
            {
                string text = kill.ToString() + "/" + needed.ToString();
                countCurrentKillContainer.SetText(text);
            }
        }

    }
}


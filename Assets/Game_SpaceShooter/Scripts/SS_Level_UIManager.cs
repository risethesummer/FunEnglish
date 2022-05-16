using TMPro;
using UnityEngine;

namespace SpaceShooter
{
    namespace SS_Manager
    {
        public class SS_Level_UIManager : Manager.UIManager_Level
        {
            [SerializeField] private TextMeshProUGUI countCurrentKillContainer;

            [SerializeField] private TextMeshProUGUI remainingEnemy;

            public void ShowCurrentKill(int kill, int needed)
            {
                string text = kill.ToString() + "/" + needed.ToString();
                countCurrentKillContainer.SetText(text);
            }

            public void ShowRemainingEnemies(int remaining)
            {
                remainingEnemy.SetText(remaining.ToString());
            }
        }

    }
}


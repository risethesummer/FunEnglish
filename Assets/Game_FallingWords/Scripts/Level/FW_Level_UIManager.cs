using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace FallingWords
{
    namespace FW_Manager
    {
        public class FW_Level_UIManager : Manager.UIManager_Level
        {
            [SerializeField] private Image[] starsCurrentShow;

            [SerializeField] private TextMeshProUGUI countWrongText;

            public void ShowCurrentStars(int current)
            {
                for (int i = current; i < starsCurrentShow.Length; i++) //Grey out the out range star
                {
                    starsCurrentShow[i].color = new Color(1, 1, 1, 0.5f);
                }
            }

            public void ResetStart()
            {
                for (int i = 0; i < starsCurrentShow.Length; i++)
                {
                    starsCurrentShow[i].color = Color.yellow;
                }
            }

            public void CountWrong(int remains)
            {
                if (remains >= 0)
                    countWrongText.SetText(remains.ToString());
            }
        }
    }
}

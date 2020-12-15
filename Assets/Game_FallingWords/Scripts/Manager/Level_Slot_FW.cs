using UnityEngine;
using UnityEngine.UI;

namespace FallingWords
{
    namespace FW_Manager
    {
        public class Level_Slot_FW : MonoBehaviour
        {
            public int index { set; get; }

            [SerializeField] private Image[] stars;

            [SerializeField] private Button play;

            public event System.Action<int> OnClick;

            private void Awake()
            {
                play.onClick.AddListener(() => OnClick?.Invoke(index));
            }

            public void SetStar(int number)
            {
                for (int i = 0; i < number; i++)
                {
                    stars[i].color = Color.yellow;
                }
            }
        }
    }
}


using UnityEngine;
using UnityEngine.UI;


    namespace Manager
    {
        public class Level_Slot : MonoBehaviour
        {
            public int Index { set; get; }

            [SerializeField] private Image[] stars;

            [SerializeField] private Button play;

            public event System.Action<int> OnClick;

            private void Awake()
            {
                play.onClick.AddListener(() => OnClick?.Invoke(Index));
            }

            public void SetStar(int number)
            {
                for (int i = 0; i < number; i++)
                {
                    stars[i].color = Color.yellow;
                }
                for (int i = number; i < 3; i++)
                {
                    stars[i].color = Color.gray;
                }
            }
        }
    }


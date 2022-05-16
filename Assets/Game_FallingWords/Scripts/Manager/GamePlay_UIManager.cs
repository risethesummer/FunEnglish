using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace Manager
{
    public class GamePlay_UIManager : UI_BaseClass
    {
        [SerializeField] private GameObject background;

        [SerializeField] private GamePlay_Manager manager;

        [SerializeField] private Level_Slot[] levelSlots;

        [SerializeField] private Button backButton;

        public event System.Action OnBack;


        private void Awake()
        {
            for (int i = 0; i < levelSlots.Length; i++)
            {
                levelSlots[i].Index = i;

                levelSlots[i].OnClick += (order) =>
                {
                    if (canInteract)
                    {
                        canInteract = false;
                        StartCoroutine(manager.LoadLevel(order));
                        canInteract = true;
                    }
                };
            }

            backButton.onClick.AddListener(() =>
            {
                OnBack?.Invoke();
                Hide();
            });
        }

        public void ModifyStar(int index, int stars)
        {
            if (index < levelSlots.Length)
                levelSlots[index].SetStar(stars);
        }

        public void AppearWithBackground()
        {
            background.SetActive(true);
            background.transform.DOLocalMoveX(0, APPEAR_DELAY * 2);
            base.Appear();
        }

        public void HideWithBackground()
        {
            canInteract = false;
            background.SetActive(true);
            background.transform.DOLocalMoveX(-3000, HIDE_DELAY * 2).OnComplete(() => background.SetActive(false));
            base.Hide();
        }
    }
}


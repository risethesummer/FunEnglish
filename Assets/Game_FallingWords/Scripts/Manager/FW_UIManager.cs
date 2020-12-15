using UnityEngine;
using UnityEngine.UI;

namespace FallingWords
{
    namespace FW_Manager
    {
        public class FW_UIManager : UI_BaseClass
        {
            [SerializeField] private Animator backgroundAnim;

            [SerializeField] private FallingWords_GameManager manager;

            [SerializeField] private Level_Slot_FW[] levelSlots;

            [SerializeField] private Button backButton;
            public event System.Action OnBack;


            private void Awake()
            {
                for (int i = 0; i < levelSlots.Length; i++)
                {
                    levelSlots[i].index = i;

                    levelSlots[i].OnClick += (order) =>
                    {
                        if (canInteract)
                        {
                            canInteract = false;
                            StartCoroutine(manager.LoadLevel(order, true));
                            canInteract = true;
                        }
                    };
                }

                backButton.onClick.AddListener(() =>
                {
                    Hide();
                    OnBack?.Invoke();
                });
            }

            public void ModifyStar(int index, int stars)
            {
                if (index < levelSlots.Length)
                    levelSlots[index].SetStar(stars);
            }

            public void AppearWithBackground()
            {
                canInteract = true;
                anim.Play("Appear");
                backgroundAnim.Play("Appear");
            }

            public void HideWithBackground()
            {
                canInteract = false;
                anim.Play("Hide");
                backgroundAnim.Play("Hide");
            }
        }
    }
}


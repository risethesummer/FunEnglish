using UnityEngine.UI;
using UnityEngine;

namespace Manager
{
    namespace UI
    {
        public class UI_OptionsPlay : UI_BaseClass
        {
            [SerializeField] private Button topicsButton;

            [SerializeField] private GamePlay_UIManager topicUIManager;

            [SerializeField] private Button synAntButton;
            [SerializeField] private GamePlay_UIManager synAntUIManager;

            //[SerializeField] private Button picturesButton;
            //[SerializeField] private UI_BaseClass picturesUIManager;

            [SerializeField] private Button backButton;
            public event System.Action OnBack;

            private void Awake()
            {
                topicsButton.onClick.AddListener(() =>
                {
                    if (canInteract)
                    {
                        Hide();
                        topicUIManager.Appear();
                    }
                });
                topicUIManager.OnBack += Appear;

                synAntButton.onClick.AddListener(() =>
                {
                    if (canInteract)
                    {
                        Hide();
                        synAntUIManager.Appear();
                    }
                });
                synAntUIManager.OnBack += Appear;

                //picturesButton.onClick.AddListener(() =>
                //{
                //    if (canInteract)
                //    {

                //    }
                //});

                backButton.onClick.AddListener(() =>
                {
                    if (canInteract)
                    {
                        Hide();
                        OnBack?.Invoke();
                    }
                });
            }

        }
    }
}


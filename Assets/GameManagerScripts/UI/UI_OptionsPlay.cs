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
                        topicUIManager.gameObject.SetActive(true);
                        Hide();
                        topicUIManager.Appear();
                    }
                });
                topicUIManager.OnBack += () =>
                {
                    this.gameObject.SetActive(true);
                    Appear();
                };                    

                synAntButton.onClick.AddListener(() =>
                {
                    if (canInteract)
                    {
                        synAntUIManager.gameObject.SetActive(true);
                        Hide();
                        synAntUIManager.Appear();
                    }
                });
                synAntUIManager.OnBack += () =>
                {
                    this.gameObject.SetActive(true);
                    Appear();
                };

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
                        OnBack?.Invoke();
                        Hide();
                    }
                });
            }

        }
    }
}


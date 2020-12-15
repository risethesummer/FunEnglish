using UnityEngine.UI;
using UnityEngine;

namespace Manager
{
    namespace UI
    {
        public class UI_Menu_Start : UI_BaseClass
        {
            [SerializeField] private Button playButton;
            public event System.Action OnPlayButtonClick; //Call back to the parent UI when clicked

            [SerializeField] private Button settingButton;
            public event System.Action OnSettingButtonClick; //Call back to the parent UI when clicked

            private void Awake()
            {
                playButton.onClick.AddListener(() =>
                {
                    if (canInteract)
                    {
                        Hide();
                        OnPlayButtonClick?.Invoke();
                    }
                });

                settingButton.onClick.AddListener(() =>
                {
                    if (canInteract)
                    {
                        Hide();
                        OnSettingButtonClick?.Invoke();
                    }
                });
            }

        }
    }
}

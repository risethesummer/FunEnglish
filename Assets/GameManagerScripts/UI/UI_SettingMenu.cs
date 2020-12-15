using UnityEngine.UI;
using UnityEngine;

namespace Manager
{
    namespace UI
    {
        public class UI_SettingMenu : UI_BaseClass
        {
            [SerializeField] private Slider sliderBackground;

            [SerializeField] private Slider sliderAction;

            [SerializeField] private Button backButton;

            public event System.Action OnBack;

            private void Awake()
            {
                backButton.onClick.AddListener(() =>
                {
                    Hide();
                    OnBack?.Invoke();
                });

                sliderBackground.onValueChanged.AddListener((vol) => GameManager.Init.SetBackgroundVolumn(vol));
                sliderAction.onValueChanged.AddListener((vol) => GameManager.Init.SetActionVolumn(vol));
            }
        }

    }
}

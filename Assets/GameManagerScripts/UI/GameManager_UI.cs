using UnityEngine;

namespace Manager
{
    namespace UI
    {
        public class GameManager_UI : MonoBehaviour
        {
            [SerializeField] private UI_Menu_Start startMenu;

            [SerializeField] private UI_OptionsPlay optionsMenu;

            [SerializeField] private UI_SettingMenu settingMenu;

            private void Awake()
            {
                startMenu.OnSettingButtonClick += () => {
                    settingMenu.gameObject.SetActive(true);
                    settingMenu.Appear();
                }; //Chose open the setting

                startMenu.OnPlayButtonClick += () => {
                    optionsMenu.gameObject.SetActive(true);
                    optionsMenu.Appear();
                };

                optionsMenu.OnBack += () =>
                {
                    startMenu.gameObject.SetActive(true);
                    startMenu.Appear();
                };

                settingMenu.OnBack += () =>
                {
                    startMenu.gameObject.SetActive(true);
                    startMenu.Appear();
                };
            }

            private void Start()
            {
                startMenu.gameObject.SetActive(true);
                startMenu.Appear();
            }
        }
    }
}

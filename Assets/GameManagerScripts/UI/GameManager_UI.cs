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
                startMenu.OnSettingButtonClick += () => settingMenu.Appear(); //Chose open the setting

                startMenu.OnPlayButtonClick += () => optionsMenu.Appear();

                optionsMenu.OnBack += () => startMenu.Appear();

                settingMenu.OnBack += () => startMenu.Appear();
            }

            private void Start()
            {
                startMenu.Appear();
            }

            public void StartMenu()
            {
                startMenu.Appear();
            }
        }
    }
}

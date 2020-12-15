using FallingWords.FW_Manager;
using UnityEngine.SceneManagement;
using UnityEngine;


namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Init;

        private const string topicManageScene = "ManagerScene_FW";
        private const string synAntManageScene = "ManagerScene_SA";
        private const string picturesManageScene = "ManagerScene_PT";

        [SerializeField] private UI.GameManager_UI uiManager;

        [SerializeField] AudioSource backgroundSource;

        [SerializeField] AudioClip switchUI;

        public static float volumn = 1f;


        private void Awake()
        {
            if (this)
                Init = this;
        }

        public void SetBackgroundVolumn(float newVol)
        {
            backgroundSource.volume = newVol;
        }

        public void SetActionVolumn(float newVol)
        {
            volumn = newVol;
        }
        
        public void SwitchMenuSound()
        {
            AudioSource.PlayClipAtPoint(switchUI, Vector3.zero, volumn);
        }
    }

}

public class Word_Check
{
    public string word { private set; get; }

    public bool right { private set; get; }

    public Word_Check(string word, bool right)
    {
        this.word = word;
        this.right = right;
    }
}
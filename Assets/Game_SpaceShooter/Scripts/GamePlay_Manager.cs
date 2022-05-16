using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Manager
{
    public abstract class GamePlay_Manager : MonoBehaviour
    {
        [SerializeField] protected GamePlay_UIManager uiManager;

        protected readonly List<int> stars = new List<int>(); //Store stars

        [SerializeField] protected TextAsset levelWithDifficult;

        public void SaveGame()
        {
            StringBuilder stringStars = new StringBuilder();

            for (int i = 0; i < stars.Count; i++)
            {
                if (i == stars.Count - 1)
                    stringStars.Append(stars[i].ToString());
                else
                    stringStars.Append(stars[i].ToString() + ",");
            }

            Debug.Log(stringStars);
            PlayerPrefs.SetString(GetPrefsString, stringStars.ToString());
        }

        public abstract string GetPrefsString { get; }

        public void LoadGame()
        {
            if (PlayerPrefs.HasKey(GetPrefsString))
            {
                string stars = PlayerPrefs.GetString(GetPrefsString);
                Debug.Log(stars);
                if (!string.IsNullOrEmpty(stars))
                    foreach (var star in stars.Split(','))
                        this.stars.Add(int.Parse(star));
                for (int i = 0; i < this.stars.Count; i++)
                    uiManager.ModifyStar(i, this.stars[i]);
            }
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Save");
            SaveGame();
        }

        public abstract IEnumerator LoadLevel(int level);

    }

}

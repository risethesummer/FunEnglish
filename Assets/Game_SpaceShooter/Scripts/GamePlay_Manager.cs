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

            PlayerPrefs.SetString("stars", stringStars.ToString());
        }

        public void LoadGame()
        {
            if (PlayerPrefs.HasKey("stars"))
            {
                string stars = PlayerPrefs.GetString("stars");

                if (!string.IsNullOrEmpty(stars))
                    foreach (var star in stars.Split(','))
                    {
                        this.stars.Add(int.Parse(star));
                    }
            }
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        public abstract IEnumerator LoadLevel(int level);

    }

}

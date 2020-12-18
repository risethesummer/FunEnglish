using UnityEngine;
using Manager;

namespace SpaceShooter
{
    namespace SS_Manager
    {
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine.SceneManagement;

        public class SS_GameManager : GamePlay_Manager
        {
            private const string sceneName = "SpaceShooter";

            public struct Level
            {
                public int enemyAmount, neededAmount, turnAmount, wrongInOne;

                public WordOption option;

                public Level(int eAmount, int nAmount, int tAmount, int w, WordOption o)
                {
                    this.enemyAmount = eAmount;
                    this.neededAmount = nAmount;
                    this.turnAmount = tAmount;
                    this.wrongInOne = w;
                    this.option = o;
                }
            }

            private readonly List<Level> levels = new List<Level>();

            public void StartLevel(int level)
            {
                var current = levels[level];

                var levelManager = GameObject.Find("LevelManager").GetComponent<SS_Level_Manager>();

                levelManager.OnEndLevel += HandleEndgame;

                StartCoroutine(levelManager.StartLevel(level, current.enemyAmount, current.neededAmount, current.turnAmount, current.wrongInOne, current.option));
            }

            public override IEnumerator LoadLevel(int level)
            {
                if (level <= stars.Count && level < levels.Count)
                {  

                    //If the scene is not loaded
                    if (!SceneManager.GetSceneByName(sceneName).isLoaded)
                    {
                        yield return new WaitWhile(() => SceneManager.sceneCount >= 2);

                        var process = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                        while (!process.isDone)
                            yield return null;

                        uiManager.HideWithBackground();
                    }

                    StartLevel(level);
                }
            }

            public IEnumerator UnloadLevel()
            {
                yield return new WaitForSeconds(1f); //Wait ui to drop down

                var progress = SceneManager.UnloadSceneAsync(sceneName);

                while (!progress.isDone)
                {
                    yield return null;
                }

                SceneManager.SetActiveScene(this.gameObject.scene);
            }


            public void HandleEndgame(bool win, int level, int star, bool nextLevel)
            {
                uiManager.AppearWithBackground();

                if (win)
                {
                    if (level < stars.Count)
                        stars[level] = star;
                    else
                        stars.Add(star);

                    uiManager.ModifyStar(level, star); //Slot  = level
                }

                if (nextLevel)
                    StartCoroutine(LoadLevel(level + 1));
                else
                    StartCoroutine(UnloadLevel());
            }
        }
    }

}



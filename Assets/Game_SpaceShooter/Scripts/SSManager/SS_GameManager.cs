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
            private SS_Level_Manager levelManager;

            public struct Level
            {
                public int enemyAmount, neededAmount, turnAmount, health;

                public WordOption option;

                //0 is syn //1 is ant
                public Level(int eAmount, int nAmount, int tAmount, int health, WordOption o)
                {
                    this.enemyAmount = eAmount;
                    this.neededAmount = nAmount;
                    this.turnAmount = tAmount;
                    this.health = health;
                    this.option = o;
                }
            }

            private void Awake()
            {
                foreach (var line in levelWithDifficult.text.Split('\n'))
                {
                    var parts = line.Split(',');

                    WordOption w;
                    if (int.Parse(parts[4]) == 0)
                        w = WordOption.Synnonym;
                    else
                        w = WordOption.Antonym;

                    levels.Add(new Level(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]), w));
                }
                LoadGame();
            }

            private readonly List<Level> levels = new List<Level>();

            public override string GetPrefsString => "SS_STARS";

            public void StartLevel(int level)
            {
                try
                {
                    var current = levels[level];
                    if (!levelManager)
                    {
                        levelManager = GameObject.Find("LevelManager").GetComponent<SS_Level_Manager>();
                        levelManager.OnEndLevel += HandleEndgame;
                    }
                    levelManager.StartLevel(level, current.enemyAmount, current.neededAmount, current.turnAmount, current.health, current.option);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }

            public override IEnumerator LoadLevel(int level)
            {
                if (level <= stars.Count && level < levels.Count)
                {

                    //If the scene is not loaded

                    yield return new WaitWhile(() => SceneManager.sceneCount >= 2);

                    var process = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    while (!process.isDone)
                        yield return null;

                    var levelScene = SceneManager.GetSceneByName(sceneName);//??? error

                    SceneManager.SetActiveScene(levelScene);

                    uiManager.HideWithBackground();

                    Screen.orientation = ScreenOrientation.Landscape; //Right rotate

                    StartLevel(level);
                }

            }

            public IEnumerator UnloadLevel()
            {
                //yield return new WaitForSeconds(1f); //Wait ui to drop down
                var progress = SceneManager.UnloadSceneAsync(sceneName);
                while (!progress.isDone)
                    yield return null;
                SceneManager.SetActiveScene(this.gameObject.scene);
            }

            public void HandleEndgame(bool win, int level, int star, bool nextLevel)
            {
                if (win)
                {
                    if (level < stars.Count)
                        stars[level] = star;
                    else
                        stars.Add(star);
                    uiManager.ModifyStar(level, star); //Slot  = level
                }

                if (!nextLevel)
                {
                    try
                    {
                        levelManager = null;
                        StartCoroutine(UnloadLevel());
                        uiManager.gameObject.SetActive(true);
                        uiManager.AppearWithBackground();
                        Screen.orientation = ScreenOrientation.Portrait;
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
                else
                {
                    StartLevel(level + 1);
                }

            }
        }
    }

}



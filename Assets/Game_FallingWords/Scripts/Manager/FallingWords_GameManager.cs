using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FallingWords
{
    using Manager;
    namespace FW_Manager
    {
        public class FallingWords_GameManager : GamePlay_Manager
        {
            private const string headSceneName = "FW_Level";

            [System.Serializable]
            struct Scene_Boxes_Words
            {
                public int scene { get; set; } //2, 3, 4 boxes
                public int wordInTopic { get; set; }

                public int missedTimes;

                public Scene_Boxes_Words(int scene, int word, int missedTime)
                {
                    this.scene = scene;
                    this.wordInTopic = word;
                    this.missedTimes = missedTime;
                }
            }

            private readonly List<Scene_Boxes_Words> levels = new List<Scene_Boxes_Words>();

            private void Awake()
            {
                foreach (var line in levelWithDifficult.text.Split('\n'))
                {
                    var parts = line.Split(',');
                    levels.Add(new Scene_Boxes_Words(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])));
                }
            }

            public void StartLevel(int level, int maxWords, int missed)
            {
                var levelManager = GameObject.Find("LevelManager").GetComponent<Level.FallingWords_Level_Manager>();

                levelManager.OnEndLevel += HandleEndGame;

                StartCoroutine(levelManager.StartLevel(level, maxWords, missed));
            }

            public override IEnumerator LoadLevel(int level)
            {

                if (level <= stars.Count && level < levels.Count)
                {
                    var currentLev = levels[level];

                    yield return new WaitWhile(() => SceneManager.sceneCount >= 2); //Exist 2 scenes

                    var progress = SceneManager.LoadSceneAsync(headSceneName + currentLev.scene.ToString(), LoadSceneMode.Additive);
                    while (!progress.isDone)
                    {
                        yield return null;
                    }

                    var levelScene = SceneManager.GetSceneByName(headSceneName + currentLev.scene.ToString());//??? error
                    SceneManager.SetActiveScene(levelScene);

                    //Hide the ui, because this scene just has a UI
                    uiManager.HideWithBackground();

                    Screen.orientation = ScreenOrientation.Landscape; //Right rotate

                    StartLevel(level, currentLev.wordInTopic, currentLev.missedTimes);
                }
            }

            public IEnumerator UnloadLevel(int level)
            {
                yield return new WaitForSeconds(1f); //Wait ui to drop down

                var currentLev = levels[level];

                var progress = SceneManager.UnloadSceneAsync(headSceneName + currentLev.scene.ToString());

                while (!progress.isDone)
                {
                    yield return null;
                }

                SceneManager.SetActiveScene(this.gameObject.scene);
            }

            public void HandleEndGame(bool win, int level, int star, bool nextLevel)
            {
                
                uiManager.AppearWithBackground();

                StartCoroutine(UnloadLevel(level));

                if (win)
                {
                    if (level < stars.Count)
                        stars[level] = star;
                    else
                        stars.Add(star);

                    uiManager.ModifyStar(level, star); //Slot  = level
                }

                if (!nextLevel)
                    Screen.orientation = ScreenOrientation.Portrait;
                else
                    StartCoroutine(LoadLevel(level + 1));

            }
        }

    }

    

}

public interface IDestroyable
{
    void DoDestroy();
}

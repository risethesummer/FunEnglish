using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

namespace FallingWords
{
    namespace FW_Manager
    {
        namespace Level
        {
            public class UIManager_Level_FW : UI_BaseClass
            {
                [SerializeField] private FallingWords_Level_Manager manager;

                [SerializeField] private Image[] stars;

                [SerializeField] private Image[] starsCurrentShow;

                [SerializeField] TextMeshProUGUI wrongWords;

                [SerializeField] TextMeshProUGUI winLoseText;

                [SerializeField] private Button backToMenu;
                [SerializeField] private Button replay;
                [SerializeField] private Button nextLevel;

                [SerializeField] private TextMeshProUGUI countWrongText;

                private const string oxfordHeadLink = "https://www.oxfordlearnersdictionaries.com/definition/english/";

                private void Awake()
                {
                    replay.onClick.AddListener(() =>
                    {
                        if (canInteract)
                        {
                            canInteract = false;
                            manager.HandleReplay();
                        }
                    });

                   

                    backToMenu.onClick.AddListener(() =>
                    {
                        if (canInteract)
                        {
                            canInteract = false;
                            manager.HandleBackToMenu(false);
                        }
                    }); 
                    

                    nextLevel.onClick.AddListener(() =>
                    {
                        if (canInteract)
                        {
                            canInteract = false;
                            manager.HandleBackToMenu(true);
                        }
                    });
                }

                public void CountWrong(int remains)
                {
                    if (remains >= 0)
                        countWrongText.SetText(remains.ToString());
                }

                public void SetupEnding(bool win, int stars, Queue<Word_Check> wrongWords)
                {
                    SetStar(stars);
                    SetWrongWords(wrongWords);
                    SetWin(win);
                    Appear();
                }

                public void ShowCurrentStars(int current)
                {
                    for (int i = current; i < stars.Length; i++) //Grey out the out range star
                    {
                        starsCurrentShow[i].color = new Color(1, 1, 1, 0.5f);
                    }
                }

                public void ResetStart()
                {
                    for (int i = 0; i < stars.Length; i++)
                    {
                        starsCurrentShow[i].color = Color.yellow;
                    }
                }

                public void SetStar(int stars)
                {
                    for (int i = 0; i < stars; i++)
                    {
                        this.stars[i].color = Color.yellow;
                    }
                }

                public void SetWrongWords(Queue<Word_Check> words)
                {
                    StringBuilder result = new StringBuilder();
                    StringBuilder word = new StringBuilder();

                    while (words.Count > 0)
                    {
                        var tempW = words.Dequeue();
                        
                        word.Append("<link=");
                        word.Append(oxfordHeadLink);
                        word.Append(tempW.word);
                        word.Append("/><u>");

                        if (tempW.right)
                            word.Append("<color=green>");
                        else
                            word.Append("<color=red>");

                        word.Append(tempW.word);
                        word.Append("</color></u></link>   ");

                        result.Append(word.ToString());
                        word.Clear();
                    }

                    wrongWords.SetText(result.ToString());
                }

                public void SetWin(bool win)
                {
                    if (win)
                    {
                        winLoseText.SetText("Winner honda X");
                        nextLevel.gameObject.SetActive(true);
                    }
                    else
                    {
                        winLoseText.SetText("Lmfao loser");
                        nextLevel.gameObject.SetActive(false);
                    }
                }

            }

        }

    }
}


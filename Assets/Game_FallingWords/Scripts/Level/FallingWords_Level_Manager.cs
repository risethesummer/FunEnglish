using UnityEngine.UI;
using FallingWords.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FallingWords
{
    namespace FW_Manager
    {
        namespace Level
        {
            public class FallingWords_Level_Manager : Manager.LevelManager_BaseClass
            {
                protected int missedTime = 3;

                protected int countWrong = 0;

                protected int subtractStar;

                protected int currentStars = 3;


                [SerializeField] private int maxWordsInTopic;

                [SerializeField] private Box_Words[] boxes; //Manage boxes

                [SerializeField] private FallingWord prefabWord;

                private readonly Queue<Word> wordsToOut = new Queue<Word>();

                [SerializeField] protected Queue<IDestroyable> destroyers = new Queue<IDestroyable>(); //Use in replay


                private Vector3 initialPos;

                [SerializeField] private int yPos;

                [SerializeField] protected FW_Level_UIManager uiManager;  

                [SerializeField] private Train train;

                public event System.Action<bool, int, int, bool> OnEndLevel;


                [SerializeField] private Button leftMove;
                [SerializeField] private Button rightMove;
                [SerializeField] private Button downMove;

                [SerializeField] private AudioClip switchSound;

                public event System.Action OnLeftMove;
                public event System.Action OnRightMove;
                public event System.Action OnDownMove;

                private void Awake()
                {
                    waitCor = new WaitForSeconds(1);

                    initialPos = new Vector3(boxes[0].FindNewXPos(), yPos);

                    for (int i = 0; i < boxes.Length; i++) //Set index, because can't change its property
                        boxes[i].whichBox = i;

                    leftMove.onClick.AddListener(() => OnLeftMove?.Invoke());
                    rightMove.onClick.AddListener(() => OnRightMove?.Invoke());
                    downMove.onClick.AddListener(() => OnDownMove?.Invoke());
                }


                //public void CheatCode()
                //{
                //    if (Input.anyKeyDown)
                //    {
                //        if (Input.GetKeyDown(cheatCode[index]))
                //        {
                //            index++;

                //        }
                //        else
                //            index = 0; //If missed key
                //    }
                //    if (index == cheatCode.Length)
                //    {
                //        missedTime += 50;
                //        countWrong = 0;
                //        index = 0;
                //        uiManager.CountWrong(missedTime - countWrong);
                //    }
                //}


                public IEnumerator StartLevel(int level, int maxWordsInTopic, int missed)
                {
                    this.maxWordsInTopic = maxWordsInTopic;
                    this.currentLevel = level;
                    this.missedTime = missed;

                    countWrong = 0;
                    currentStars = 3;

                    uiManager.CountWrong(missedTime);

                    destroyers.Enqueue(train.GetComponent<IDestroyable>());

                    subtractStar = (missed / 3) > 0 ? missed / 3 : 1; //count how many missed time will subtract 1 star

                    RandomChooseTopic();

                    yield return new WaitForSeconds(3);

                    InitialNewWord();
                }

                public void InitialNewWord()
                {
                    //If 
                    if (wordsToOut.Count == 0)
                    {
                        HandleEndingLevel();
                    }
                    else
                    {
                        var word = wordsToOut.Dequeue();

                        var falling = Instantiate(prefabWord, initialPos, Quaternion.identity);


                        //Add and remove after destroying
                        OnLeftMove += () =>
                        {
                            falling.InputMoveWordToLeft();
                            if (!source.isPlaying)
                                source.PlayOneShot(switchSound, Manager.GameManager.volumn);
                        };

                        OnRightMove += () =>
                        {
                            falling.InputMoveWordToRight();

                            if (!source.isPlaying)
                                source.PlayOneShot(switchSound, Manager.GameManager.volumn);
                        };

                        OnDownMove += () =>
                        {
                            falling.InputToMoveQuick();
                            if (!source.isPlaying)
                                source.PlayOneShot(switchSound, Manager.GameManager.volumn);
                        };


                        destroyers.Enqueue(falling.GetComponent<IDestroyable>());

                        List<float> pos = new List<float>(); //Find pos first
                        foreach (var box in boxes)
                            pos.Add(box.FindNewXPos());

                        falling.SetUp(word, pos.ToArray());

                        falling.OnTouch += (index, match, w) =>
                        {
                            boxes[index].currentWords++;

                            if (!match)
                            {
                                source.PlayOneShot(wrongSound, Manager.GameManager.volumn);
                                countWrong++;
                                uiManager.CountWrong(missedTime - countWrong);

                                if (countWrong % subtractStar == 0)
                                {
                                    currentStars--;
                                    if (currentStars < 0)
                                        currentStars = 0; //Can't be less than 0
                                    uiManager.ShowCurrentStars(currentStars);
                                }
                            }
                            else
                                source.PlayOneShot(rightSound, Manager.GameManager.volumn);

                            OnLeftMove = null;
                            OnRightMove = null;
                            OnDownMove = null;

                            reviewWords.Enqueue(new Word_Check(w, match));

                            StartCoroutine(WaitSpawningCoroutine());
                        };
                    }
                }

                public void HandleEndingLevel()
                {
                    bool win = countWrong <= missedTime;

                    train.Move();

                    if (win)
                        source.PlayOneShot(winSound, Manager.GameManager.volumn);
                    else
                        source.PlayOneShot(loseSound, Manager.GameManager.volumn);

                    uiManager.gameObject.SetActive(true);
                    uiManager.SetupEnding(win, currentStars, reviewWords);
                }

                public override void HandleReplay()
                {
                    while (destroyers.Count > 0) //Clear words and train
                        destroyers.Dequeue().DoDestroy();
                    reviewWords.Clear();
                    uiManager.ResetStart();
                    wordsToOut.Clear(); //restore words
                    uiManager.Hide();
                    StartCoroutine(StartLevel(currentLevel, maxWordsInTopic, missedTime));
                }

                public override void HandleBackToMenu(bool nextLevel)
                {
                    bool win = countWrong <= missedTime;

                    OnEndLevel?.Invoke(win, currentLevel, currentStars, nextLevel);
                    uiManager.Hide();
                }

                public void QuickLosing()
                {
                    countWrong = missedTime + 1;
                    currentStars = 0;
                    StopAllCoroutines();
                }

                IEnumerator WaitSpawningCoroutine()
                {
                    yield return waitCor;
                    InitialNewWord();
                }

                public void SetTopicsToBoxes(IList<string> topics)
                {
                    for (int i = 0; i < topics.Count; i++)
                    {
                        boxes[i].SetupBox(topics[i]);
                    }
                }

                //Choose random before starting the game
                public void RandomChooseWord(IList<string> topics)
                {
                    IDictionary<string, List<Word>> stored = TopicsManager.GetWords(currentLevel / 3); //Seperate 2 boxes => 0. 3 boxes => 1. 4 => 2

                    List<bool>[] markChoose = new List<bool>[topics.Count]; //Use to check if choose a chosen word

                    for (int i = 0; i < topics.Count; i++)
                    {
                        markChoose[i] = new List<bool>(); //Check false to all
                        for (int j = 0; j < stored[topics[i]].Count; j++)
                        {
                            markChoose[i].Add(false);
                        }
                    }

                    List<int> countWordsInTopic = new List<int>();

                    for (int i = 0; i < topics.Count; i++)
                    {
                        countWordsInTopic.Add(0); //Mark them 0
                    }

                    while (wordsToOut.Count < boxes.Length * maxWordsInTopic) //When filling the queue
                    {
                        int randomTopic = Random.Range(0, topics.Count);

                        if (countWordsInTopic[randomTopic] >= maxWordsInTopic)
                            continue; //If the topic is full

                        int randomWord;
                        do //Choose random in current topic
                        {
                            randomWord = Random.Range(0, stored[topics[randomTopic]].Count);
                        }
                        while (markChoose[randomTopic][randomWord]); //If choose a chosen word

                        markChoose[randomTopic][randomWord] = true;

                        countWordsInTopic[randomTopic]++;

                        wordsToOut.Enqueue(stored[topics[randomTopic]][randomWord]);
                    }
                }
                public void RandomChooseTopic()
                {
                    List<string> chosenTopics = new List<string>();

                    List<int> numbers = new List<int>();

                    while (numbers.Count < boxes.Length) //Choose number of topics equal to boxes array
                    {
                        int random;
                        do
                        {
                            random = Random.Range(0, FW_Manager.TopicsManager.topicsAmount);
                        }
                        while (numbers.Contains(random));

                        chosenTopics.Add(FW_Manager.TopicsManager.topics[random]);

                        numbers.Add(random);
                    }

                    RandomChooseWord(chosenTopics);
                    SetTopicsToBoxes(chosenTopics);
                }
            }

        }

    }
}

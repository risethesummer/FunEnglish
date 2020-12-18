using UnityEngine;

namespace SpaceShooter
{
    using Objects;
    using System.Collections;
    using System.Collections.Generic;

    namespace SS_Manager
    {
        public class SS_Level_Manager : Manager.LevelManager_BaseClass
        {
            private int enemyAmount;

            private int turnAmount; //Count how many turns

            private int wrongInOne; //Count wrong times in an object

            [SerializeField] WordOption option;

            private int currentDead = 0;

            //Pool object
            [SerializeField] private Enemy enemyPf;
            [SerializeField] private Queue<Enemy> enemies = new Queue<Enemy>();
                 
            private readonly List<string> keys = new List<string>();

            //private readonly Dictionary<string, Queue<Word_SymAnt>> wordsList = new Dictionary<string, Queue<Word_SymAnt>>();

            [SerializeField]  private PlayerBattleManager player;

            public event System.Action<bool, int, int, bool> OnEndLevel;

            private void Start()
            {
                StartCoroutine(StartLevel(0, 3, 3, 3, WordOption.Synnonym));    
            }

            public IEnumerator StartLevel(int level, int enemyAmount, int turnAmount, int wrongInOne, WordOption option)
            {
                this.enemyAmount = enemyAmount;
                this.turnAmount = turnAmount;
                this.wrongInOne = wrongInOne;
                this.option = option;
                currentLevel = level;
                currentDead = 0;
                currentStars = 3;

                if (currentLevel == 0 && guide)
                {
                    guide.SetActive(true);
                    yield return new WaitWhile(() => guide.activeSelf);
                }

                player.SetBattle(ChooseWordsRandom(), enemyAmount);

                player.OnDead += () => HandleEnding(false);

                InitializeEnemies();

                yield return new WaitForSeconds(3);

                ActiveNewEnemies();
            }

            //Initilize all
            public void InitializeEnemies()
            {
                //Initial for a turn with turn amount
                for (int i = 0; i < enemyAmount; i++)
                {
                    //Set key to enemy
                    //Use action to call back and deque a word_sym from words list
                    //Till word list is empty or the enemy is dead => remove from the dictionary
                    //Start from current turn (how many enemies was dead)

                    //send player current keys by an array
                    //Player pick random in array to initilize bullets
                    //If that element in dictionary is empty => enemey destroy

                    //send player current words
                    var enemy = Instantiate(this.enemyPf, new Vector2(-10f, -10f), Quaternion.identity);

                    destroyers.Enqueue(enemy.GetComponent<IDestroyable>());

                    enemy.Setup(keys[i], wrongInOne, player);

                    enemy.OnDead += (sA) =>
                    {
                        currentDead++;

                        if (currentDead % (enemyAmount / turnAmount) == 0)
                            ActiveNewEnemies();
                        else
                            player.RemoveAOption(sA);
                        
                    };

                    enemy.OnWrong += (match, word) =>
                    {
                        if (!match)
                        {
                            currentStars--;
                            if (currentStars < 0)
                                currentStars = 0; //Can't be less than 0

                            uiManager.ShowCurrentStars(currentStars);
                        }

                        reviewWords.Enqueue(new Word_Check(word, match));
                    };

                    enemies.Enqueue(enemy);
                }
            }

            public void ActiveNewEnemies()
            {
                int amount = enemyAmount / turnAmount;

                string[] newKeys = new string[amount];

                for (int i = 0; i < amount; i++)
                {
                    if (enemies.Count > 0)
                    {
                        var enemy = enemies.Dequeue();
                        enemy.gameObject.SetActive(true);
                        newKeys[i] = enemy.contain;
                    }
                    else
                    {
                        HandleEnding(true);
                        return;
                    }
                }
                player.SetKeys(newKeys);
            }

            public override void HandleReplay()
            {
                base.HandleReplay();

                keys.Clear();

                StartCoroutine(StartLevel(currentLevel, enemyAmount, turnAmount, wrongInOne, option));
            }

            public void HandleEnding(bool win)
            {
                if (win)
                    AudioSource.PlayClipAtPoint(winSound, Vector3.zero, Manager.GameManager.volumn);
                else
                    AudioSource.PlayClipAtPoint(loseSound, Vector3.zero, Manager.GameManager.volumn);

                while (destroyers.Count > 0) //Clear words and train
                    destroyers.Dequeue().DoDestroy();

                enemies.Clear();

                uiManager.SetupEnding(win, currentStars, reviewWords);
            }

            public override void HandleBackToMenu(bool nextLevel)
            {
                base.HandleBackToMenu(nextLevel);

                bool win = currentDead >= enemyAmount;

                OnEndLevel?.Invoke(win, currentLevel, currentStars, nextLevel);
            }

            public void QuickLosing()
            {
                currentStars = 0;
                currentDead = 0;
                StopAllCoroutines();
            }

            public Dictionary<string, Queue<Word_SymAnt>> ChooseWordsRandom()
            {
                Dictionary<string, Queue<Word_SymAnt>> wordsList = new Dictionary<string, Queue<Word_SymAnt>>();

                var managerWords = SynonymAntonymManager.Init;

                int count = option == WordOption.Synnonym ? managerWords.CountSyn : managerWords.CountAnt;

                bool[] checks = new bool[count];
                int[] indexes = new int[enemyAmount];

                int temp;
                //Choose word for enemies
                for (int i = 0; i < enemyAmount; i++)
                {
                    do
                    {
                        temp = Random.Range(0, count);
                    }
                    while (checks[temp]);
                    checks[temp] = true;
                    indexes[i] = temp;
                }

                if (option == WordOption.Synnonym)
                {
                    foreach (var words in managerWords.GetSynonyms(indexes))
                    {
                        string key = words[0].synAnt;

                        if (!wordsList.ContainsKey(key))
                        {
                            //Add to queue, and get it in turn
                            wordsList.Add(key, new Queue<Word_SymAnt>());
                            keys.Add(key);
                        }

                        foreach (var word in words)
                            wordsList[key].Enqueue(word);
                    }
                }
                else
                {
                    foreach (var words in managerWords.GetAntonyms(indexes))
                    {
                        string key = words[0].synAnt;

                        if (!wordsList.ContainsKey(key))
                        {
                            //Add to queue, and get it in turn
                            wordsList.Add(key, new Queue<Word_SymAnt>());
                            keys.Add(key);
                        }

                        foreach (var word in words)
                            wordsList[key].Enqueue(word);
                    }
                }
                return wordsList;
            }
        }

        [System.Serializable]
        public enum WordOption
        {
            Antonym, Synnonym
        }

    }
}


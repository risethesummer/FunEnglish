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

            private int health; //Count wrong times in an object

            [SerializeField] WordOption option;

            private int currentDead = 0;

            private int amountNeeded;

            private int killed;

            //Pool object
            [SerializeField] private Enemy[] enemyPf;
            [SerializeField] private Queue<Enemy> enemies = new Queue<Enemy>();
            [SerializeField] private Queue<Enemy> currentEnemies = new Queue<Enemy>();
            private readonly List<string> keys = new List<string>();

            //private readonly Dictionary<string, Queue<Word_SymAnt>> wordsList = new Dictionary<string, Queue<Word_SymAnt>>();

            [SerializeField]  private PlayerBattleManager player;

            public event System.Action<bool, int, int, bool> OnEndLevel;

            [SerializeField] protected SS_Level_UIManager uiManager;

            [SerializeField] private AudioClip fireSound;

            [SerializeField] private AudioClip damageSound;
            [SerializeField] private AudioClip destroySound;

            [SerializeField] private TextAnimation optionText;

            //private bool isEnd;
            private bool isEnd;

            public void AddWrongWord(string word, bool match)
            {
                reviewWords.Enqueue(new Word_Check(word, match));
            }

            public IEnumerator StartLevel(int level, int enemyAmount, int amountNeeded,int turnAmount, int health, WordOption option)
            {
                this.enemyAmount = enemyAmount;
                this.turnAmount = turnAmount;
                this.health = health;
                this.amountNeeded = amountNeeded;
                this.option = option;

                currentLevel = level;
                currentDead = 0;
                killed = 0;
                isEnd = false;

                uiManager.ShowCurrentKill(killed, amountNeeded);

                string text = option == WordOption.Synnonym ? "synonym" : "antonym";
                optionText.gameObject.SetActive(true);
                optionText.Text(text);

                player.SetBattle(ChooseWordsRandom(), enemyAmount);

                player.OnDead += () => HandleEnding();

                player.OnOutOfBullet += OutOffBullet;

                player.OnFireSound += () => source.PlayOneShot(fireSound, Manager.GameManager.volumn / 2f);

                player.OnDamageSound += () => source.PlayOneShot(damageSound, Manager.GameManager.volumn);

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
                    int index = Random.Range(0, enemyPf.Length);
                    var enemy = Instantiate(this.enemyPf[index], new Vector2(-10f, -10f), Quaternion.identity);

                    enemy.Setup(keys[i], health);

                    enemy.OnFireSound += () => source.PlayOneShot(fireSound, Manager.GameManager.volumn / 4f);

                    enemy.OnDead += (sA, playerKill) =>
                    {
                        source.PlayOneShot(destroySound, Manager.GameManager.volumn);
                        currentDead++;
                        if (playerKill)
                        {
                            killed++;
                            uiManager.ShowCurrentKill(killed, amountNeeded);
                        }
                        ActiveNewEnemies(sA);
                    };

                    //Take wrong words
                    enemy.OnWrong += (match, word) =>
                    {
                        if (match)
                            source.PlayOneShot(rightSound, Manager.GameManager.volumn);
                        else
                            source.PlayOneShot(wrongSound, Manager.GameManager.volumn);

                        reviewWords.Enqueue(new Word_Check(word, match));
                    };


                    enemies.Enqueue(enemy);
                }
            }

            public void OutOffBullet(string enemyWord)
            {
                foreach (var e in currentEnemies)
                {
                    if (e.contain == enemyWord)
                    {
                        if (e.isActive)
                        {
                            e.DoDestroy();
                            break;
                        }
                    }
                }
            }

            public void ActiveNewEnemies(string last = "")
            {
                if (enemies.Count <= 0 && currentEnemies.Count == enemyAmount)
                {
                    HandleEnding();
                    return;
                }

                player.RemoveAOption(last);

                int amount = enemyAmount / turnAmount;

                if (currentDead % amount == 0)
                {
                    string[] newKeys = new string[amount];

                    for (int i = 0; i < amount; i++)
                    {
                        if (enemies.Count > 0)
                        {
                            var enemy = enemies.Dequeue();

                            currentEnemies.Enqueue(enemy);

                            enemy.gameObject.SetActive(true);

                            newKeys[i] = enemy.contain;
                        }
                    }
                    player.SetKeys(newKeys);
                }
            }

            public override void HandleReplay()
            {
                ClearAllEnemies();
                player.EndBattle();
                keys.Clear();
                reviewWords.Clear();
                uiManager.Hide();
                StartCoroutine(StartLevel(currentLevel, enemyAmount, amountNeeded, turnAmount, health, option));
            }

            public void ClearAllEnemies()
            {
                while (enemies.Count > 0)
                    Destroy(enemies.Dequeue().gameObject);
                while (currentEnemies.Count > 0)
                    Destroy(currentEnemies.Dequeue().gameObject);
            }

            public int GetStars(bool win)
            {
                int star;
                if (win)
                    star = 3 - (amountNeeded - killed);
                else
                    star = 0;

                return star;
            }
            public void HandleEnding()
            {
                if (!isEnd)
                {
                    isEnd = true;

                    bool win = killed >= amountNeeded;

                    if (win)
                        source.PlayOneShot(winSound, Manager.GameManager.volumn);
                    else
                        source.PlayOneShot(loseSound, Manager.GameManager.volumn);

                    ClearAllEnemies();

                    uiManager.SetupEnding(win, GetStars(win), reviewWords);
                 }
            }

            public override void HandleBackToMenu(bool nextLevel)
            {
                bool win = killed >= amountNeeded;

                OnEndLevel?.Invoke(win, currentLevel, GetStars(win), nextLevel);

                uiManager.Hide();
            }

            public void QuickLosing()
            {
                killed = 0;
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


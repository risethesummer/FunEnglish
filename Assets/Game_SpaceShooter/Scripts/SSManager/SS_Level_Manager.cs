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
            private readonly Queue<Enemy> enemies = new Queue<Enemy>();
            private readonly List<Enemy> currentEnemies = new List<Enemy>();
            private readonly List<string> keys = new List<string>();

            [SerializeField]  private PlayerBattleManager player;

            public event System.Action<bool, int, int, bool> OnEndLevel;

            [SerializeField] protected SS_Level_UIManager uiManager;

            [SerializeField] private AudioClip fireSound;

            [SerializeField] private AudioClip damageSound;
            [SerializeField] private AudioClip destroySound;

            [SerializeField] private TextAnimation optionText;

            //private bool isEnd;
            private bool isEnd;

            private void Awake()
            {
                player.OnDead += () => HandleEnding();
                player.OnOutOfBullet += OutOffBullet;
                player.OnFireSound += () => source.PlayOneShot(fireSound, Manager.GameManager.volumn / 2f);
                player.OnDamageSound += () => source.PlayOneShot(damageSound, Manager.GameManager.volumn);
            }

            public void AddWrongWord(string word, bool match)
            {
                reviewWords.Enqueue(new Word_Check(word, match));
            }

            public void StartLevel(int level, int enemyAmount, int amountNeeded, int turnAmount, int health, WordOption option)
            {
                try
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
                    uiManager.ShowRemainingEnemies(enemyAmount);
                    string text = option == WordOption.Synnonym ? "synonym" : "antonym";
                    optionText.gameObject.SetActive(true);
                    optionText.Text(text);
                    player.SetBattle(ChooseWordsRandom(), enemyAmount);
                    InitializeEnemies();
                    ActiveNewEnemies();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                }
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
                            if (killed >= amountNeeded)
                                HandleEnding();
                            uiManager.ShowCurrentKill(killed, amountNeeded);
                        }
                        uiManager.ShowRemainingEnemies(enemies.Count + currentEnemies.Count);
                        ActiveNewEnemies(sA);
                    };

                    //Take wrong words
                    enemy.OnWrong += (match, word) =>
                    {
                        source.PlayOneShot(match ? rightSound : wrongSound, Manager.GameManager.volumn);
                        reviewWords.Enqueue(new Word_Check(word, match));
                    };

                    enemies.Enqueue(enemy);
                }
            }

            public void OutOffBullet(string enemyWord)
            {
                for (int i = 0; i < currentEnemies.Count; i++)
                {
                    Enemy e = currentEnemies[i];
                    if (e.contain == enemyWord && e.IsActive)
                    {
                        currentEnemies.RemoveAt(i);
                        e.DoDestroy();
                        break;
                    }
                }
            }

            public void ActiveNewEnemies(string last = "")
            {
                //If there remaining enemies is not enough to pass the level
                if (enemies.Count + currentEnemies.Count < amountNeeded - killed)
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
                          
                            try
                            {
                                var enemy = enemies.Dequeue();
                                currentEnemies.Add(enemy);
                                enemy.gameObject.SetActive(true);
                                newKeys[i] = enemy.contain;

                            }
                            catch (System.Exception e)
                            {
                                Debug.Log(e.Message);
                            }
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
                StartLevel(currentLevel, enemyAmount, amountNeeded, turnAmount, health, option);
            }

            public void ClearAllEnemies()
            {
                while (enemies.Count > 0)
                    Destroy(enemies.Dequeue().gameObject);
                foreach (var enemy in currentEnemies)
                    Destroy(enemy.gameObject);
                currentEnemies.Clear();
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
                    source.PlayOneShot(win ? winSound : loseSound, Manager.GameManager.volumn);
                    ClearAllEnemies();
                    player.EndBattle();
                    keys.Clear();
                    uiManager.gameObject.SetActive(true);
                    uiManager.SetupEnding(win, GetStars(win), reviewWords);
                    reviewWords.Clear();
                }
            }

            public override void HandleBackToMenu(bool nextLevel)
            {
                bool win = killed >= amountNeeded;
                uiManager.Hide();
                OnEndLevel?.Invoke(win, currentLevel, GetStars(win), nextLevel);
            }

            public void QuickLosing()
            {
                killed = 0;
                currentDead = 0;
                StopAllCoroutines();
            }

            public Dictionary<string, Queue<Word_SymAnt>> ChooseWordsRandom()
            {
                try
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

                    IEnumerable<Word_SymAnt[]> symAnts = option == WordOption.Synnonym ?
                        managerWords.GetSynonyms(indexes) : managerWords.GetAntonyms(indexes);

                    foreach (var words in symAnts)
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
                    return wordsList;
                }
                catch (System.Exception e)
                {
                    return null;
                }
            }
        }

        [System.Serializable]
        public enum WordOption
        {
            Antonym, Synnonym
        }

    }
}


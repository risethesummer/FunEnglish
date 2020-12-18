using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SpaceShooter
{
    namespace Objects
    {
        public class PlayerBattleManager : MonoBehaviour
        {
            //Health
            [SerializeField]
            private int maxHealth;
            private int currentHealth;

            //Cooldown 
            [SerializeField]
            private float timeDelay;
            private WaitForSeconds waitDelay;
            private bool canFire = true;

            //Pool objects
            private const int amountToInit = 20;
            [SerializeField]
            private PlayerBullet prefab;
            private readonly List<PlayerBullet> bullets = new List<PlayerBullet>();

            public event System.Action OnDead;


            //Count input touches
            private int countTouches;

            private Dictionary<string, Queue<Word_SymAnt>> wordsList = new Dictionary<string, Queue<Word_SymAnt>>();

            private readonly List<Word_Check> currentSynAntCanChoose = new List<Word_Check>();
            private Word_SymAnt currentChose;
            [SerializeField] private TextMeshProUGUI wordContain;

            [SerializeField] private UnityEngine.UI.Slider healthBar;

            private void Awake()
            {
                waitDelay = new WaitForSeconds(timeDelay);
                currentHealth = maxHealth;

                for (int i = 0; i < amountToInit; i++)
                {
                    var temp = Instantiate(prefab, new Vector3(-10, 10), Quaternion.identity);
                    bullets.Add(temp);
                }
            }

            public void SetBattle(Dictionary<string, Queue<Word_SymAnt>> words, int health)
            {
                this.wordsList = words;
                this.currentHealth = health;
                this.maxHealth = health;
                healthBar.value = 1;
                GetComponent<PlayerMovementManager>().canMove = true;
            }

            public void SetKeys(string[] keys)
            {
                currentSynAntCanChoose.Clear();
                foreach (var key in keys)
                    currentSynAntCanChoose.Add(new Word_Check(key, false));
                ChooseForFire();
            }

            private void Update()
            {
                countTouches = Input.touchCount;

                if (countTouches > 0)
                {
                    var touch = Input.GetTouch(countTouches - 1);
                    if (touch.phase == TouchPhase.Began)
                        Fire();
                }
            }

            public void ChooseForFire()
            {
                bool isFound = false;
                for (int i = 0; i < currentSynAntCanChoose.Count; i++)
                {
                    if (currentSynAntCanChoose[i].right && wordsList[currentSynAntCanChoose[i].word].Count > 0)
                    {
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                    return;

                int temp;
                Word_Check tempWord;
                do
                {
                    temp = Random.Range(0, currentSynAntCanChoose.Count); //Choose random to initial a bullet
                    tempWord = currentSynAntCanChoose[temp];

                } while (!tempWord.right || wordsList[tempWord.word].Count <= 0); //While choose an topic is empty

                currentChose = wordsList[tempWord.word].Dequeue();

                wordContain.SetText(currentChose.word);
            }

            //Remove to can't choose it anymore in the function above
            public void RemoveAOption(string sA)
            {
                for (int i = 0; i < currentSynAntCanChoose.Count; i++)
                {
                    if (currentSynAntCanChoose[i].word == sA)
                        currentSynAntCanChoose[i] = new Word_Check(sA, false);
                }
            }

            public void Fire()
            {
                if (canFire)
                {
                    canFire = false;

                    foreach (var bullet in bullets)
                    {
                        if (!bullet.isActive && !currentChose.IsNull())
                        {

                            bullet.SetPosition(this.transform.position + Vector3.up);

                            bullet.DoActive(Vector2.up, currentChose);

                            bullet.gameObject.SetActive(true);

                            currentChose = new Word_SymAnt("", "");

                            ChooseForFire();
                            break;
                        }
                    }

                    StartCoroutine(CoolDownFire());
                }
            }

            IEnumerator CoolDownFire()
            {
                yield return waitDelay;

                canFire = true;
            }

            public void TakenDamage(int damamge = 1)
            {
                currentHealth -= damamge;

                if (currentHealth <= 0)
                    HandleDying();
                else
                {
                    healthBar.value = (float)currentHealth / (float)maxHealth;
                }
            }

            public void HandleDying()
            {
                canFire = false;
                GetComponent<PlayerMovementManager>().canMove = false;
                OnDead?.Invoke();
            }
        }
    }
}

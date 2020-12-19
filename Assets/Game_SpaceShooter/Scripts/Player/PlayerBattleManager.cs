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
            [SerializeField] private int currentHealth;

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
            public event System.Action<string> OnOutOfBullet;
            public event System.Action OnFireSound;
            public event System.Action OnDamageSound;
            //Count input touches
            [SerializeField] private UnityEngine.UI.Button fireButton;

            private Dictionary<string, Queue<Word_SymAnt>> wordsList = new Dictionary<string, Queue<Word_SymAnt>>();

            private readonly List<Word_Check> currentSynAntCanChoose = new List<Word_Check>();
            private Word_SymAnt currentChose;
            [SerializeField] private TextMeshProUGUI wordContain;

            [SerializeField] private UnityEngine.UI.Image healthBar;

            private PlayerMovementManager movement;

            public void EndBattle()
            {
                movement.canMove = false;
                canFire = false;
                OnDead = null;
                OnOutOfBullet = null;
                OnFireSound = null;
                OnDamageSound = null;
            }

            private void Awake()
            {
                movement = GetComponent<PlayerMovementManager>();

                fireButton.onClick.AddListener(Fire);

                waitDelay = new WaitForSeconds(timeDelay);

                
            }

            public void SetBattle(Dictionary<string, Queue<Word_SymAnt>> words, int health)
            {
                for (int i = 0; i < amountToInit; i++)
                {
                    var temp = Instantiate(prefab, new Vector3(-10, 10), Quaternion.identity);
                    bullets.Add(temp);
                }

                this.wordsList = words;

                this.currentHealth = health;

                this.maxHealth = health;

                healthBar.fillAmount = 1;

                movement.canMove = true;

                canFire = true;
            }

            public void SetKeys(string[] keys)
            {
                currentSynAntCanChoose.Clear();
                foreach (var key in keys)
                    currentSynAntCanChoose.Add(new Word_Check(key, true));
                ChooseForFire();
            }


            public void ChooseForFire()
            {
                bool check = false;
                foreach (var choice in currentSynAntCanChoose)
                {
                    if (choice.right)
                    {
                        check = true;
                        break;
                    }
                }
                if (!check)
                {
                    currentChose = new Word_SymAnt("", "");

                    wordContain.SetText("");

                    return;
                }

                int temp;
                Word_Check tempWord;
                do
                {
                    temp = Random.Range(0, currentSynAntCanChoose.Count); //Choose random to initial a bullet
                    tempWord = currentSynAntCanChoose[temp];

                } while (!tempWord.right); //While choose an topic is empty

                if (wordsList[tempWord.word].Count > 0)
                {
                    currentChose = wordsList[tempWord.word].Dequeue();
                    wordContain.SetText(currentChose.word);
                }
                else
                {
                    tempWord.right = false;

                    currentChose = new Word_SymAnt("", "");

                    wordContain.SetText("");

                    OnOutOfBullet?.Invoke(tempWord.word);

                    //Rechoose
                    ChooseForFire();
                }
            }

            //Remove to can't choose it anymore in the function above
            public void RemoveAOption(string sA)
            {
                for (int i = 0; i < currentSynAntCanChoose.Count; i++)
                {
                    if (currentSynAntCanChoose[i].word == sA)
                    {
                        currentSynAntCanChoose[i].right = false;
                    }
                }
            }

            public void Fire()
            {
                if (canFire && !currentChose.IsNull())
                {
                    canFire = false;

                    foreach (var bullet in bullets)
                    {
                        if (!bullet.isActive)
                        {
                            OnFireSound?.Invoke();

                            bullet.SetPosition(this.transform.position + Vector3.up);

                            bullet.DoActive(Vector2.up, currentChose);

                            bullet.gameObject.SetActive(true);

                            currentChose = new Word_SymAnt("", "");

                            wordContain.SetText("");

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
                OnDamageSound?.Invoke();
                currentHealth -= damamge;

                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    HandleDying();
                }
                
                healthBar.fillAmount = (float)currentHealth / (float)maxHealth;
            }

            public void HandleDying()
            {
                canFire = false;
                movement.canMove = false;
                OnDead?.Invoke();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SpaceShooter
{
    namespace Objects
    {
        public class Enemy : MonoBehaviour, IDamagable, IDestroyable
        {
            [SerializeField] private float minX = -4;
            [SerializeField] private float maxX = 4;
            [SerializeField] private float minY = 0;
            [SerializeField] private float maxY = 12;

            public event System.Action OnFireSound;
            public string contain { get; private set; }

            private int health;

            private int maxHealth;

            [SerializeField] private UnityEngine.UI.Image healthBar;

            public event System.Action<string, bool> OnDead;

            public event System.Action<bool, string> OnWrong;

            public bool isActive = false;

            [SerializeField] private AnimationManager animator;

            [SerializeField] private int speed = 10;

            const int amount = 20;

            [SerializeField] private EnemyBullet bulletPf;
            private readonly List<EnemyBullet> bullets = new List<EnemyBullet>();

            private bool canFire = true;

            private WaitForSeconds cooldown;

            private WaitForSeconds cooldownMove;

            [SerializeField] TextMeshProUGUI textContain;

            private Vector3 targetPos;

            private void OnEnable()
            {
                isActive = true;
                this.transform.position = new Vector3(Random.Range(minX, maxX), maxY);
            }

            public void Setup(string contain, int maxHealth)
            {
                this.contain = contain;
                this.health = maxHealth;
                this.maxHealth = maxHealth;
                ChangeHealth();
                textContain.SetText(contain);
            }

            public void ChangeHealth()
            {
                healthBar.fillAmount = (float)health / maxHealth;
            }
            private void Awake()
            {
                gameObject.SetActive(false);

                cooldown = new WaitForSeconds(1.5f);

                cooldownMove = new WaitForSeconds(0.5f);

                for (int i = 0; i < amount; i++)
                {
                    var newB = Instantiate(bulletPf, new Vector3(-10, -10), Quaternion.identity);
                    bullets.Add(newB);
                }
                
            }

            private void Start()
            {
                targetPos = this.transform.position;
                StartCoroutine(Move());
            }

            private void Update()
            {
                if (canFire)
                    Fire();
            }

            IEnumerator Move()
            {
                if (this.transform.position != targetPos)
                {
                    transform.position = Vector2.MoveTowards(this.transform.position, targetPos, 4f * Time.deltaTime);
                    yield return null;
                    StartCoroutine(Move());
                }
                else
                {
                    targetPos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
                    yield return cooldownMove;
                    StartCoroutine(Move());
                }
            }

            public void Fire()
            {
                canFire = false;

                OnFireSound?.Invoke();

                foreach (var b in bullets)
                {
                    if (!b.isActive)
                    {
                        b.SetPosition(this.transform.position + Vector3.down);

                        b.DoActive(Vector2.down);

                        b.gameObject.SetActive(true);

                        break;
                    }
                }

                StartCoroutine(Cooldown());
            }

            IEnumerator Cooldown()
            {
                yield return cooldown;
                canFire = true;
            }


            public void TakenDamage(Word_SymAnt word_SymAnt)
            {
                if (isActive)
                {
                    bool check = word_SymAnt.synAnt == contain;

                    OnWrong?.Invoke(check, word_SymAnt.word);

                    if (check)
                    {
                        health--;
                        if (health <= 0)
                            StartCoroutine(DoDestroyWithAnim(true));
                        else
                            ChangeHealth();
                    }
                }
            }

            public void Move(Vector3 direction, int speed = 0)
            {
                int realSpeed = speed == 0 ? this.speed : speed;
                transform.Translate((direction - this.transform.position) * realSpeed);
            }

            private IEnumerator DoDestroyWithAnim(bool playerKillOrNot)
            {
                isActive = false;
                OnDead?.Invoke(contain, playerKillOrNot);
                yield return new WaitForSeconds(animator.DestroyAnim());
                this.gameObject.SetActive(false);
            }

            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (collision && isActive)
                {
                    if (collision.TryGetComponent<PlayerBattleManager>(out var player))
                    {
                        isActive = false;
                        player.TakenDamage(1);
                        StartCoroutine(DoDestroyWithAnim(false));
                    }
                }
            }

            public void DoDestroy()
            {
                if (this.gameObject.activeInHierarchy)
                    StartCoroutine(DoDestroyWithAnim(false));
            }

            private void OnDestroy()
            {
                foreach (var bullet in bullets)
                {
                    if (bullet != null)
                        Destroy(bullet.gameObject);
                }
            }
        }
    }
}


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
            private PlayerBattleManager target;

            const int minX = -4;
            const int maxX = 4;
            const int minY = 0;
            const int maxY = 12;

            public string contain { get; private set; }

            private int maxWrong;

            private int countWrong = 0;

            public event System.Action<string> OnDead;

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
                this.transform.position = new Vector3(4, 12);
            }

            private void Awake()
            {
                gameObject.SetActive(false);

                cooldown = new WaitForSeconds(1.5f);

                cooldownMove = new WaitForSeconds(1f);

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
                //Move();
            }

            IEnumerator Move()
            {
                if (this.transform.position != targetPos)
                {
                    transform.position = Vector2.MoveTowards(this.transform.position, targetPos, 1f * Time.deltaTime);
                    yield return null;
                    StartCoroutine(Move());
                }
                else
                {
                    targetPos = new Vector2(Random.Range((float)minX, maxX), Random.Range((float)minY, maxY));
                    yield return cooldownMove;
                    StartCoroutine(Move());
                }
            }

            public void Fire()
            {
                canFire = false;

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

            public void Setup(string contain, int maxWrong, PlayerBattleManager target)
            {
                this.contain = contain;
                this.maxWrong = maxWrong;
                this.target = target;
                textContain.SetText(contain);
            }

            public void TakenDamage(Word_SymAnt word_SymAnt)
            {
                bool check = word_SymAnt.synAnt == contain;

                OnWrong?.Invoke(check, word_SymAnt.word);

                if (check)
                    StartCoroutine(DoDestroyWithAnim());
                else
                {
                    countWrong++;
                    if (countWrong >= maxWrong)
                         Move(target.transform.position, 20);
                }
            }

            public void Move(Vector3 direction, int speed = 0)
            {
                int realSpeed = speed == 0 ? this.speed : speed;
                transform.Translate((direction - this.transform.position) * realSpeed);
            }

            private IEnumerator DoDestroyWithAnim()
            {
                yield return new WaitForSeconds(animator.DestroyAnim());
                OnDead?.Invoke(contain);
                Destroy(this.gameObject);
            }

            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (collision)
                {
                    if (collision.TryGetComponent<PlayerBattleManager>(out var player))
                    {
                        player.TakenDamage(3);
                        StartCoroutine(DoDestroyWithAnim());
                    }
                }
            }

            public void DoDestroy()
            {
                Destroy(this.gameObject);
            }
        }
    }
}


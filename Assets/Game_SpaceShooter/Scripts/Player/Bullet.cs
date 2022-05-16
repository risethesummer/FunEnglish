using UnityEngine;

namespace SpaceShooter
{
    namespace Objects
    {
        public class Bullet : MonoBehaviour, IDestroyBullet
        {
            [SerializeField]
            private float speed;

            public bool isActive = false;

            private Vector2 direction = Vector2.zero;

            private void Awake()
            {
                isActive = false;
                gameObject.SetActive(false);
            }

            private void Update()
            {
                if (isActive)
                    transform.Translate(direction * speed * Time.deltaTime);
            }

            public void DoActive(Vector2 direction)
            {
                isActive = true;

                this.direction = direction;
            }

            public void SetPosition(Vector2 newPos)
            {
                this.transform.position = newPos;
            }


            public void DoDestroy()
            {
                isActive = false;

                gameObject.SetActive(false);

                SetPosition(new Vector2(-10, -10));
            }
        }
    }

    public interface IDamagable
    {
        void TakenDamage(Word_SymAnt sA);
    }

    public interface IDestroyBullet
    {
        void DoDestroy();
    }
}



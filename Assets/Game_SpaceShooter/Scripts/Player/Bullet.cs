using UnityEngine;

namespace SpaceShooter
{
    namespace Objects
    {
        public class Bullet : MonoBehaviour
        {
            [SerializeField]
            private float speed;

            private bool isActive = false;

            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (collision && isActive)
                {
                    if (collision.TryGetComponent<IDamagable>(out var damagable))
                    {
                        damagable.TakenDamage();
                        DoDestroy();
                    }
                }
            }

            public void DoDestroy()
            {
                isActive = false;
                gameObject.SetActive(false);
            }
        }
    }

    public interface IDamagable
    {
        void TakenDamage();
    }
}



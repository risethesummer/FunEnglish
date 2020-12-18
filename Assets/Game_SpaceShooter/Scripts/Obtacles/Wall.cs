using UnityEngine;

namespace SpaceShooter
{
    namespace Objects
    {
        public class Wall : MonoBehaviour
        {
            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (collision)
                {
                    if (collision.TryGetComponent<IDestroyBullet>(out var des))
                        des.DoDestroy();
                }
            }
        }
    }
}


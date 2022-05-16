using UnityEngine;

namespace SpaceShooter
{
    namespace Objects
    {
        public class Wall : MonoBehaviour
        {
            [SerializeField] private SS_Manager.SS_Level_Manager manager;

            private void OnTriggerEnter2D(Collider2D collision)
            {

                if (collision)
                {
                    if (collision.TryGetComponent<PlayerBullet>(out var des))
                    {
                        string word = des.DoDestroyWithWord();
                        manager.AddWrongWord(word, false);
                    }
                    else if (collision.TryGetComponent<Bullet>(out var b))
                        b.DoDestroy();
                }
            }
        }
    }
}


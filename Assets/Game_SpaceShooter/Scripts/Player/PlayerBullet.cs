using UnityEngine;

namespace SpaceShooter
{
    namespace Objects
    {
        public class PlayerBullet : Bullet
        {
            private Word_SymAnt word;

            public void DoActive(Vector2 direction, Word_SymAnt word)
            {
                base.DoActive(direction);
                this.word = word;
            }

            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (collision)
                {
                    if (collision.TryGetComponent<IDamagable>(out var e))
                    {
                        e.TakenDamage(word);
                        base.DoDestroy();
                    }
                }
            }

            public string DoDestroyWithWord()
            {
                base.DoDestroy();
                return word.word;
            }
        }
    }
}

using UnityEngine;


namespace SpaceShooter
{
    namespace Objects
    {
        public class EnemyBullet : Bullet
        {
            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (collision && isActive)
                {
                    if (collision.TryGetComponent<PlayerBattleManager>(out var player))
                    {
                        player.TakenDamage();
                        DoDestroy();
                    }
                }
            }
        }
    }
}


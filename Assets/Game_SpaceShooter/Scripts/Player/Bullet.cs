using UnityEngine;

namespace SpaceShooter
{
    namespace Objects
    {
        public class Bullet : MonoBehaviour
        {
            [SerializeField]
            private float speed;

            public bool isActive = false;

            private Vector2 direction = Vector2.zero;

            private Rigidbody2D rigid;

            private void Awake()
            {
                isActive = false;
                gameObject.SetActive(false);
                rigid = GetComponent<Rigidbody2D>();
            }

            public void DoActive(Vector2 direction)
            {
                isActive = true;

                rigid.AddForce(direction * speed);
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

}



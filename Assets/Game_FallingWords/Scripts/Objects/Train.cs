using System.Collections.Generic;
using UnityEngine;

namespace FallingWords
{
    namespace Objects
    {
        public class Train : MonoBehaviour, IDestroyable
        {
            [SerializeField] int speed;

            [SerializeField] private Rigidbody2D head;

            private Rigidbody2D[] rigids;

            private readonly List<Vector2> position = new List<Vector2>();

            const int e = 100;

            private void Awake()
            {
                rigids = GetComponentsInChildren<Rigidbody2D>();
                foreach (var rigid in rigids)
                {
                    position.Add(rigid.gameObject.transform.position);
                }
            }

            public void Move()
            {
                head.AddForce(speed * e * Vector2.right);
            }

            public void DoDestroy()
            {
                for (int i = 0; i < rigids.Length; i++)
                {
                    rigids[i].velocity = Vector2.zero;
                    rigids[i].transform.position = position[i];
                }
            }
        }

    }
}

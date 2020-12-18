using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    namespace Objects
    {
        public class PlayerMovementManager : MonoBehaviour
        {
            public bool canMove { get; set; } = false;
            [SerializeField] 
            private int speed;

            [SerializeField]
            private Joystick controller;

            private Rigidbody2D rigidBody;

           // private AnimationManager animationManager;

            private void Awake()
            {
                rigidBody = GetComponent<Rigidbody2D>();
                //animationManager = GetComponent<AnimationManager>();
            }

            private void Update()
            {
                if (canMove)
                {
                    Vector2 direction = controller.Direction;
                    Move(direction);
                }
            }

            public void Move(Vector2 direction)
            {
                rigidBody.AddForce(direction * speed);
                //animationManager.MoveAnim(direction.y);
            }
        }
    }
}


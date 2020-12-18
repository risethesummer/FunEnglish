using UnityEngine;

namespace SpaceShooter
{
    namespace Objects
    {
        public class AnimationManager : MonoBehaviour
        {
            private Animator animator;
            [SerializeField] float deadLength;

            //[SerializeField]
            //private float timeBeforeDestroying;

            private void Awake()
            {
                animator = GetComponent<Animator>();
            }

            //Just anim to the left or right
            public void MoveAnim(float hor)
            {
                animator.SetFloat("Horizontal", hor);
            }

            public float DestroyAnim()
            {
                animator.Play("Dead");
                return deadLength;
            }
        }
    }
}

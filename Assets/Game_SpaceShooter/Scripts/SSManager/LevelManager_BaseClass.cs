using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public abstract class LevelManager_BaseClass : MonoBehaviour
    {
        [SerializeField] protected int currentLevel;

        [SerializeField]
        protected AudioSource source;
        [SerializeField]
        protected AudioClip rightSound;
        [SerializeField]
        protected AudioClip wrongSound;
        [SerializeField] protected AudioClip winSound;

        [SerializeField] protected AudioClip loseSound;


        protected readonly Queue<Word_Check> reviewWords = new Queue<Word_Check>();

        protected WaitForSeconds waitCor;

        public abstract void HandleReplay();

        public abstract void HandleBackToMenu(bool nextLevel);
    }

}

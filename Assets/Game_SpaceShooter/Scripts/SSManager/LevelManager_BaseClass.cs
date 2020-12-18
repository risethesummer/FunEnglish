using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class LevelManager_BaseClass : MonoBehaviour
    {
        [SerializeField] protected int currentLevel;

        protected int countWrong = 0;

        protected int subtractStar;

        protected int currentStars = 3;


        [SerializeField] protected AudioClip winSound;

        [SerializeField] protected AudioClip loseSound;

        [SerializeField] protected GameObject guide;

        [SerializeField] protected Queue<IDestroyable> destroyers = new Queue<IDestroyable>(); //Use in replay

        [SerializeField] protected UIManager_Level uiManager;

        protected readonly Queue<Word_Check> reviewWords = new Queue<Word_Check>();

        protected WaitForSeconds waitCor;

        public virtual void HandleReplay()
        {
            print("replay");
            while (destroyers.Count > 0) //Clear words and train
                destroyers.Dequeue().DoDestroy();

            uiManager.ResetStart();
            reviewWords.Clear();
            uiManager.Hide();
        }

        public virtual void HandleBackToMenu(bool nextLevel)
        {
            uiManager.Hide(); //UI of current level
        }

    }

}

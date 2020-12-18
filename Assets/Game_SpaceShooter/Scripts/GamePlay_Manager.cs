using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public abstract class GamePlay_Manager : MonoBehaviour
    {
        [SerializeField] protected GamePlay_UIManager uiManager;

        protected readonly List<int> stars = new List<int>(); //Store stars

        [SerializeField] protected TextAsset levelWithDifficult;

        public abstract IEnumerator LoadLevel(int level);


    }

}

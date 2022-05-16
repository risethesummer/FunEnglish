using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    namespace SS_Manager
    {
        public class SynonymAntonymManager : MonoBehaviour
        {
            public static SynonymAntonymManager Init;
        
            private readonly Dictionary<string, List<Word_SymAnt>> synonyms = new Dictionary<string, List<Word_SymAnt>>();
            private readonly List<string> synonymKeys = new List<string>();

            private readonly Dictionary<string, List<Word_SymAnt>> antonyms = new Dictionary<string, List<Word_SymAnt>>();
            private readonly List<string> antonymKeys = new List<string>();

            [SerializeField] private TextAsset synData;

            [SerializeField] private TextAsset antData;

            private void Awake()
            {
                if (this)
                {
                    GetDatatbase();
                    Init = this;
                }
            }

            public void GetDatatbase()
            {
                foreach (var line in synData.text.Split('\n'))
                {
                    var parts = line.Split(' ');

                    string head = parts[0];

                    if (!synonyms.ContainsKey(head))
                    {
                        synonyms.Add(head, new List<Word_SymAnt>());
                        synonymKeys.Add(head);

                    }

                    var temp = synonyms[head];
                    for (int i = 1; i < parts.Length; i++)
                    {
                        temp.Add(new Word_SymAnt(head, parts[i]));
                    }
                }

                foreach (var line in antData.text.Split('\r'))
                {
                    var parts = line.Split(' ');

                    string head = parts[0];

                    if (!antonyms.ContainsKey(head))
                    {
                        antonyms.Add(head, new List<Word_SymAnt>());
                        antonymKeys.Add(head);
                    }

                    var temp = antonyms[head];
                    for (int i = 1; i < parts.Length; i++)
                    {
                        temp.Add(new Word_SymAnt(head, parts[i]));
                    }
                }
            }

            public int CountSyn => synonyms.Count;
            public int CountAnt => antonyms.Count;

            public IEnumerable<Word_SymAnt[]> GetSynonyms(int[] index)
            {
                for (int i = 0; i < index.Length; i++)
                    yield return synonyms[synonymKeys[index[i]]].ToArray();
            }

            public IEnumerable<Word_SymAnt[]> GetAntonyms(int[] index)
            {
                for (int i = 0; i < index.Length; i++)
                    yield return antonyms[antonymKeys[index[i]]].ToArray();
            }
        }
    }

    [System.Serializable]
    public struct Word_SymAnt
    {
        public string synAnt;
        public string word;

        public Word_SymAnt(string sA, string word)
        {
            synAnt = sA;
            this.word = word;
        }

        public bool IsNull()
        {
            return string.IsNullOrEmpty(synAnt) || string.IsNullOrEmpty(word);
        }
    }
}

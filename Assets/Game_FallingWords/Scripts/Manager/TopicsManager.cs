using System.Collections.Generic;
using UnityEngine;

namespace FallingWords
{
    namespace FW_Manager
    {
        public class TopicsManager : MonoBehaviour
        {
            public static readonly int levelsAmount = 3;

            public static readonly int topicsAmount = 18;

            public static readonly string[] levelStringRepresent = new string[] { "easy", "medium", "hard" };

            [SerializeField] private TextAsset[] assets;

            private static List<Dictionary<string, List<Word>>> levels = new List<Dictionary<string, List<Word>>>(); //3 levels

            public static List<string> topics = new List<string>();

            private void Awake()
            {
                for (int i = 0; i < levelsAmount; i++)
                    levels.Add(new Dictionary<string, List<Word>>());
                SetupWords();
            }

            public void ShowWords()
            {
                for (int i = 0; i < levels.Count; i++)
                {
                    print(levels[i].Count);
                }
            }

            public void SetupWords()
            {
                foreach (var asset in assets)
                {
                    topics.Add(asset.name);

                    foreach (var line in asset.text.Split('\n'))
                    {
                        var part = line.Split(',');

                        int index = FindMatchedType(part[2]); //Find level

                        if (!levels[index].ContainsKey(asset.name)) //If dont have key in dictionary
                            levels[index].Add(asset.name, new List<Word>()); //Add a new pair

                        levels[index][asset.name].Add(new Word(asset.name, part[0], part[1]));
                    }
                }
            }

            private int FindMatchedType(string level)
            {
                for (int i = 0; i < levelStringRepresent.Length; i++)
                {
                    if (levelStringRepresent[i] == level)
                        return i;
                }
                return -1;
            }

            public static IDictionary<string, List<Word>> GetWords(int index)
            {
                if (index < levels.Count)
                    return levels[index];
                print("Out range");
                return null;
            }
        }


        public class Word
        {
            public string topic { get; set; }
            public string word { get; set; }
            public string form { get; set; }
            public Word(string topic, string word, string form)
            {
                this.topic = topic;
                this.word = word;
                this.form = form;
            }
        }
    }
}

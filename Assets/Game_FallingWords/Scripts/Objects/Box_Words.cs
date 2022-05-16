using UnityEngine;
using TMPro;

namespace FallingWords
{
    namespace Objects
    {
        public class Box_Words : MonoBehaviour, ITouch
        {
            public string topic { get; set; }

            public int whichBox { get; set; }


            [SerializeField] private TextMeshProUGUI topicContainer;

            public int currentWords { get; set; } =  0;

            public void SetupBox(string topic)
            {
                this.topic = topic;

                this.topicContainer.SetText(topic);

            }

            public float FindNewXPos()
            {
                return this.transform.position.x;
            }
        }

        public interface ITouch
        {
            string topic { get; set; }
            int whichBox { get; set; }

        }
    }

}

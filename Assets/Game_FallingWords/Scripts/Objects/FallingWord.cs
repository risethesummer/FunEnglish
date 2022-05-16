using FallingWords.FW_Manager;
using TMPro;
using UnityEngine;

namespace FallingWords
{
    namespace Objects
    {
        public class FallingWord : MonoBehaviour, ITouch, IDestroyable
        {
            public string topic { get; set; }

            [SerializeField] private TextMeshProUGUI wordContainter;

            [SerializeField] private TextMeshProUGUI formContainer;

            [SerializeField] private bool active = false;

            private float[] posToMove;

            private int currentWordPos = 0;

            public event System.Action<int, bool, string> OnTouch;

            //[SerializeField] private int speedDown = 10;

            [SerializeField] private int speedHor;

            public int whichBox { get; set; }

            private void Update()
            {
                if (active)
                    Move();
            }

            public void SetUp(Word word, float[] positions)
            {
                wordContainter.SetText(word.word);

                formContainer.SetText(ConvertForm(word.form));

                this.topic = word.topic;

                posToMove = positions;

                active = true;
            }

            public string ConvertForm(string form)
            {
                if (form == "adjective")
                    return "Adj";
                System.Text.StringBuilder res = new System.Text.StringBuilder();
                res.Append(char.ToUpper(form[0]));
                for (int i = 1; i < form.Length; i++)
                    res.Append(form[i]);

                return res.ToString();
            }

            public void Move()
            {
                if (transform.position.x != posToMove[currentWordPos])
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(posToMove[currentWordPos], transform.position.y), speedHor * Time.deltaTime);
                }
            }

            public void InputMoveWordToRight()
            {
                //if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
              //  {
                    if (currentWordPos < posToMove.Length - 1)
                        currentWordPos++;
              //  }
            }

            public void InputMoveWordToLeft()
            {

                    if (currentWordPos > 0)
                        currentWordPos--;
            }

            public void InputToMoveQuick()
            {
                var rigid = GetComponent<Rigidbody2D>();

                if (rigid.velocity.y > -20f)
                    rigid.AddForce(new Vector2(0, -500f), ForceMode2D.Force);
            }

            public void Stop()
            {
                active = false;
                //rigid.gravityScale = 0;
            }

            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (active)
                {
                    if (collision.TryGetComponent<ITouch>(out var box))
                    {

                        this.whichBox = box.whichBox;

                        //After handle will assign current topic
                        var isMatched = box.topic.Equals(this.topic);

                        HandleMatching(isMatched);

                        OnTouch?.Invoke(whichBox, isMatched, this.wordContainter.text); //Use the previous box to invoke

                        this.topic = box.topic;

                        Stop();

                        if (collision.TryGetComponent<Rigidbody2D>(out var rigid))
                        {
                            var newPos =  new Vector3 (rigid.transform.position.x, this.transform.position.y);
                            this.transform.position = newPos;
                            gameObject.AddComponent<FixedJoint2D>().connectedBody = rigid;
                        }
                    }
                }
            }

            public void HandleMatching(bool matched)
            {
                if (matched)
                    wordContainter.color = Color.green;
                else
                    wordContainter.color = Color.red;
            }

            public void DoDestroy()
            {
                active = false;
                Destroy(this.gameObject);
            }
        }

    }
}

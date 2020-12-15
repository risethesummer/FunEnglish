using FallingWords.FW_Manager;
using TMPro;
using UnityEngine;

namespace FallingWords
{
    namespace Objects
    {
        public class FallingWord : MonoBehaviour, ITouch, IDestroy
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

            [SerializeField] private AudioClip rightSound;

            [SerializeField] private AudioClip wrongSound;

            [SerializeField] private AudioClip switchSound;

            public int whichBox { get; set; }

            private void Update()
            {
                if (active)
                {
                    InputMoveWordToLeft();
                    InputMoveWordToRight();
                    InputToMoveQuick();
                    Move();
                }
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
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (currentWordPos < posToMove.Length - 1)
                    {
                        currentWordPos++;
                        AudioSource.PlayClipAtPoint(switchSound, Vector3.zero, Manager.GameManager.volumn);
                    }
                }
            }

            public void InputMoveWordToLeft()
            {

                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (currentWordPos > 0)
                    {
                        currentWordPos--;
                        AudioSource.PlayClipAtPoint(switchSound, Vector3.zero, Manager.GameManager.volumn);

                    }
                }
            }

            public void InputToMoveQuick()
            {
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -1000f), ForceMode2D.Force);
                    AudioSource.PlayClipAtPoint(switchSound, Vector3.zero, Manager.GameManager.volumn);
                }
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
                            gameObject.AddComponent<FixedJoint2D>().connectedBody = rigid;
                        }
                    }
                }
            }

            public void HandleMatching(bool matched)
            {
                if (matched)
                {
                    wordContainter.color = Color.green;
                    AudioSource.PlayClipAtPoint(rightSound, Vector3.zero, Manager.GameManager.volumn);
                }
                else
                {
                    wordContainter.color = Color.red;
                    AudioSource.PlayClipAtPoint(wrongSound, Vector3.zero, Manager.GameManager.volumn);
                }
            }

            public void DoDestroy()
            {
                active = false;
                Destroy(this.gameObject);
            }
        }

    }
}

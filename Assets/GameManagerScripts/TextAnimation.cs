using System.Collections;
using System.Text;
using UnityEngine;

public class TextAnimation : MonoBehaviour
{
    private string text;

    [SerializeField] TMPro.TextMeshProUGUI contain;

    [SerializeField] Animator animator;

    [SerializeField] private float time;
    private WaitForSeconds wait;

    private void Awake()
    {
        wait = new WaitForSeconds(time);
    }
    public void Text(string text)
    {
        contain.SetText("");
        this.text = text;
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        StringBuilder temp = new StringBuilder();

        foreach (var word in text.ToCharArray())
        {
            temp.Append(word);
            contain.SetText(temp.ToString());
            yield return wait;
        }

        animator.Play("Hide");

        yield return new WaitForSeconds(0.5f);

        this.gameObject.SetActive(false);
    }
}

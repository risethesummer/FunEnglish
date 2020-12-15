using System.Collections;
using UnityEngine;

public class UI_BaseClass : MonoBehaviour //Base class for every UI
{
    protected bool canInteract = false;

    [SerializeField] protected Animator anim;

    public virtual void Hide()
    {
        canInteract = false;
        anim.Play("Hide");
    }

    public virtual void Appear()
    {
        canInteract = true;
        StartCoroutine(WaitBeforeAppearing());
    }

    IEnumerator WaitBeforeAppearing()
    {
        yield return new WaitForSeconds(0.8f);
        anim.Play("Appear");
    }
}

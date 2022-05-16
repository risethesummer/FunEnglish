using System.Collections;
using UnityEngine;
using DG.Tweening;

public class UI_BaseClass : MonoBehaviour //Base class for every UI
{
    protected bool canInteract = false;

    public static readonly float APPEAR_DELAY = 0.5f;
    public static readonly float HIDE_DELAY = 0.5f;

    public virtual void Hide()
    {
        canInteract = false;
        transform.localScale = new Vector3(1, 1);
        transform.DOScale(0, HIDE_DELAY).OnComplete(() => gameObject.SetActive(false));
    }

    public virtual void Appear()
    {
        transform.localScale = new Vector3(0, 0);
        transform.DOScale(1, APPEAR_DELAY).SetDelay(HIDE_DELAY).OnComplete(() => canInteract = true);
    }
}

using System.Collections;
using UnityEngine;

public class ClickOpenLink : MonoBehaviour
{
    [SerializeField] private string link;
    public void OpenLink()
    {
        Application.OpenURL(link);
    }
}

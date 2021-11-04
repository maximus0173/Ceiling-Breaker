using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectFirstButton : MonoBehaviour
{

    private void OnEnable()
    {
        StartCoroutine(SelectButtonDelayed());
    }

    IEnumerator SelectButtonDelayed()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        Button button = GetComponentInChildren<Button>();
        if (button != null)
        {
            button.Select();
        }
    }

    private void OnDisable()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

}

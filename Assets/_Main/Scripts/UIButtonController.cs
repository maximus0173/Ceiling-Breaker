using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonController : MonoBehaviour
{

    [SerializeField]
    protected Button button;

    [SerializeField]
    protected bool disableOnWebGL = false;

    private void Awake()
    {
        this.button = GetComponent<Button>();
    }

    private void Start()
    {
#if UNITY_WEBGL
        if (this.disableOnWebGL)
        {
            this.button.interactable = false;
        }
#endif
    }

}

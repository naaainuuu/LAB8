using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UIController : MonoBehaviour
{
    public static UIController instance;

    private void Awake()
    {
        instance = this;
    }

    public TMP_Text overHeatedMessage;
}

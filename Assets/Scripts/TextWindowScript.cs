using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextWindowScript : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] InputManager.PossibleKeys Key;

    [SerializeField] private TextMeshProUGUI TMP;

    [SerializeField] private bool JustText_true_OrIncludeKey_false;
    [SerializeField] string text;

    void Start()
    {
        if(!JustText_true_OrIncludeKey_false)
        {
            text = "Press " + inputManager.GetInputsFor(Key)[0].ToString() + " " + text;
        }
        TMP.text = text;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textToChange;

    public void SetText(string text)
    {
        this.textToChange.text = text;
    }
}

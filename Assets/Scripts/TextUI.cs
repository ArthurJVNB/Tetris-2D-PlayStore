using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetText(string text)
    {
        this.text.text = text;
    }
}

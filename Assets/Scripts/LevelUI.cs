using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void UpdateLevel(int value)
    {
        text.text = value.ToString();
    }
}

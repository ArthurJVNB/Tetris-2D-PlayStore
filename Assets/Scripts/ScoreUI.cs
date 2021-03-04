using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void UpdateScore(int value)
    {
        text.text = value.ToString();
    }
}

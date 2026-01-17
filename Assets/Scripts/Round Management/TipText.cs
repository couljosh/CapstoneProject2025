using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TipText : MonoBehaviour
{
    public List<string> TipTexts;

    void Start()
    {
        int rand = Random.Range(0, TipTexts.Count);

        gameObject.GetComponent<TextMeshProUGUI>().text = ("TIP: " + TipTexts[rand]);
    }
}

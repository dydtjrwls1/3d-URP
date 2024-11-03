using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoViewer : MonoBehaviour
{
    public TextMeshProUGUI CurrentAmmoGUI { get; private set; }

    public TextMeshProUGUI TotalAmmoUI { get; private set; }

    public TextMeshProUGUI GrenadeUI { get; private set; }

    public Image Icon { get; private set; }

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        Icon = child.GetComponent<Image>();

        child = transform.GetChild(1);
        TotalAmmoUI = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(2);
        CurrentAmmoGUI = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(4);
        GrenadeUI = child.GetComponent<TextMeshProUGUI>();
    }
}

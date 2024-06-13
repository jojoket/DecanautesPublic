using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIButton : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image img;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color hoverColor;

    public void Start() {
        if (text != null) text.color = normalColor;
        if (img != null) img.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (text != null) text.color = hoverColor;
        if (img != null) img.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (text != null) text.color = normalColor;
        if (img != null) img.color = normalColor;
    }
}
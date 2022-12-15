using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Отображение атрибута карты в тексте
/// </summary>
public class StatDisplay : MonoBehaviour
{
    [SerializeField]
    private Attribute attribute;
    [SerializeField]
    private Text statText;
    [SerializeField]
    private float updateTime = 0.8f;

    private int displayValue = 0;

    private void Awake()
    {
        attribute.OnValueChanged += UpdateDisplay;
    }

    private void OnDestroy()
    {
        attribute.OnValueChanged -= UpdateDisplay;
    }

    private void UpdateDisplay(Attribute attr)
    {
        DOTween.To(() => displayValue,
                   x => { displayValue = x; statText.text = x.ToString(); },
                   attr.Value,
                   updateTime);
    }
}

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

    private void Awake() => attribute.OnValueChanged += UpdateDisplay;

    private void OnDestroy() => attribute.OnValueChanged -= UpdateDisplay;

    private void UpdateDisplay(Attribute attr) => statText.text = attr.Value.ToString();
}

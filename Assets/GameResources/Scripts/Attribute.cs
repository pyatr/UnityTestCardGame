using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AttributeType
{
    Health = 0,
    Mana = 1,
    Attack = 2
}

public class Attribute : MonoBehaviour
{
    private const float ATTR_CHANGE_TIME = 0.3f;

    public Action<Attribute> OnValueChanged = delegate { };

    public AttributeType AttributeType => attributeType;

    public int Value
    {
        get => attributeValue;
        set => DOTween.To(() => attributeValue, x => { attributeValue = x; OnValueChanged.Invoke(this); }, value, ATTR_CHANGE_TIME);
    }

    [SerializeField]
    private AttributeType attributeType;

    [SerializeField, Range(1, 100)]
    private int minValue, maxValue;

    private int attributeValue = 1;

    public void Randomize()
    {
        Value = Random.Range(minValue, maxValue + 1);
    }
}
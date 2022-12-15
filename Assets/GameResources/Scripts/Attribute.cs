using System;
using System.Collections;
using System.Collections.Generic;
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
    public Action<Attribute> OnValueChanged = delegate { };

    public AttributeType AttributeType => attributeType;

    public int Value
    {
        get => attributeValue;
        set
        {
            attributeValue = value;
            OnValueChanged.Invoke(this);
        }
    }

    [SerializeField]
    private AttributeType attributeType;

    [SerializeField, Range(1, 100)]
    private int minValue, maxValue;

    private int attributeValue = 0;

    public void Randomize()
    {
        Value = Random.Range(minValue, maxValue + 1);
    }
}
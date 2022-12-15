using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action<Card, Attribute> OnAttributeChanged = delegate { };

    public RectTransform RectTransform { get; private set; }

    public Vector2Int ImageDimensions => imageDimensions;

    public IReadOnlyDictionary<AttributeType, Attribute> Attributes => attributes;

    public int Health
    {
        get => Attributes[AttributeType.Health].Value;
        set => Attributes[AttributeType.Health].Value = value;
    }

    public int Mana
    {
        get => Attributes[AttributeType.Mana].Value;
        set => Attributes[AttributeType.Mana].Value = value;
    }

    public int Attack
    {
        get => Attributes[AttributeType.Attack].Value;
        set => Attributes[AttributeType.Attack].Value = value;
    }

    [HideInInspector]
    public CardDeck ParentDeck;

    [SerializeField]
    private UIOutline outline;
    [SerializeField]
    private Text cardName, cardDescription;
    [SerializeField]
    private Image cardImage;

    [SerializeField]
    private List<Attribute> attributeObjects;

    [SerializeField]
    private Vector2Int imageDimensions = new Vector2Int(200, 300);

    [SerializeField]
    private float outlineChargeTime = 0.7f;

    private Dictionary<AttributeType, Attribute> attributes = new();

    private float outlineWidthCached;

    private void Awake()
    {
        RectTransform = transform as RectTransform;
        outlineWidthCached = outline.OutlineWidth;
        attributeObjects.ForEach(at => attributes.Add(at.AttributeType, at));
    }

    private void Start()
    {
        attributeObjects.ForEach(a => { a.OnValueChanged += InvokeAttributeChangeEvent; a.Randomize(); });
    }

    private void InvokeAttributeChangeEvent(Attribute a)
    {
        OnAttributeChanged.Invoke(this, a);
    }

    private void OnDestroy()
    {
        attributeObjects.ForEach(a => { a.OnValueChanged -= InvokeAttributeChangeEvent; });
    }

    public void SetImageFromTexture(Texture2D texture)
    {
        cardImage.sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.one / 2);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        outline.OutlineWidth = 0f;
        outline.enabled = true;
        DOTween.To(() => outline.OutlineWidth = 0f,
                   x => outline.OutlineWidth = x,
                   outlineWidthCached,
                   outlineChargeTime);
    }

    public void OnDrag(PointerEventData eventData)
    {
#if UNITY_ANDROID || UNITY_IOS
        RectTransform.position = Input.touches[0].position;
#else
        RectTransform.position = Input.mousePosition;
#endif
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DOTween.To(() => outline.OutlineWidth = outlineWidthCached,
                   x => outline.OutlineWidth = x,
                   0f,
                   outlineChargeTime).onComplete += () => outline.enabled = false;
    }
}

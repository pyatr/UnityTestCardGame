using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action<Card, Attribute> OnAttributeChanged = delegate { };

    public RectTransform RectTransform { get; private set; }

    public Vector2Int ImageDimensions => imageDimensions;


    public int Health
    {
        get => health.Value;
        set
        {
            health.Value = value;
            OnAttributeChanged.Invoke(this, health);
        }
    }

    public int Mana
    {
        get => mana.Value;
        set
        {
            mana.Value = value;
            OnAttributeChanged.Invoke(this, mana);
        }
    }

    public int Attack
    {
        get => attack.Value;
        set
        {
            attack.Value = value;
            OnAttributeChanged.Invoke(this, attack);
        }
    }

    [SerializeField]
    private Attribute health, mana, attack;
    [SerializeField]
    private UIOutline outline;
    [SerializeField]
    private Text cardName, cardDescription;
    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private Vector2Int imageDimensions = new Vector2Int(200, 300);

    [SerializeField]
    private float outlineChargeTime = 0.7f;

    private float outlineWidthCached;

    private void Awake()
    {
        RectTransform = transform as RectTransform;
        outlineWidthCached = outline.OutlineWidth;
    }

    private void Start()
    {
        health.Randomize();
        mana.Randomize();
        attack.Randomize();
    }

    public void SetImageFromTexture(Texture2D texture)
    {
        cardImage.sprite = Sprite.Create(texture, new Rect(Vector2.zero, imageDimensions), Vector2.one / 2);
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

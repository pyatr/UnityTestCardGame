using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
///  онтроллер карточной колоды
/// </summary>
public class CardDeck : MonoBehaviour
{
    private const int MIN_RANDOM_CHANGE = -2;
    private const int MAX_RANDOM_CHANGE = 9;

    private const float MAX_IMAGE_WAIT_TIME = 10f;

    [SerializeField]
    private RectTransform deckPoint, cardArea, cardSpawnPoint;

    [SerializeField]
    private AnimationCurve placementCurve, movementCurve;

    [SerializeField]
    private Card cardPrefab;

    [SerializeField, Range(0, 100)]
    private int minCardAmount = 0;
    [SerializeField, Range(0, 100)]
    private int maxCardAmount = 0;

    [SerializeField]
    private float cardMoveSpeed = 2f, cardMoveStartDelay = 0.5f, rotationPerCard = 20f, rotationOffset = 8f, randomStatChangeDelay = 0.3f, cardGap = 24f;

    private List<Card> activeCards = new List<Card>();

    private List<Sequence> movingCards = new List<Sequence>();

    private WaitForSeconds randomStatChangeTimer = null;

    private ImageDownloader imageDownloader;

    private int imagesToDownload = 0;

    #region Unity methods
    private void Awake()
    {
        imageDownloader = FindObjectOfType<ImageDownloader>();
        randomStatChangeTimer = new WaitForSeconds(randomStatChangeDelay);
    }

    private void Start()
    {
        GenerateCards();
    }

    private void OnValidate()
    {
        if (minCardAmount > maxCardAmount)
        {
            minCardAmount = maxCardAmount - 1;
        }
    }
    #endregion

    #region Public methods
    public void DamageRandomCard()
    {
        if (activeCards.Count > 0)
        {
            activeCards.GetRandomElement().Health -= Random.Range(1, 3);
        }
    }

    public void StartRandomChangeCoroutine()
    {
        StartCoroutine(RandomChangeCoroutine());
    }

    public void DestroyCard(Card card)
    {
        if (activeCards.Contains(card))
        {
            activeCards.Remove(card);
            Destroy(card.gameObject);
            ArrangeCards();
        }
    }
    #endregion

    #region Private methods
    [ContextMenu("Generate cards")]
    private void GenerateCards()
    {
        int generatedCards = Random.Range(minCardAmount, maxCardAmount + 1);
        imagesToDownload = generatedCards;
        for (int i = 0; i < generatedCards; i++)
            GenerateCard();
        Invoke(nameof(ArrangeCards), MAX_IMAGE_WAIT_TIME);
    }

    private void GenerateCard()
    {
        Card newCard = Instantiate(cardPrefab);
        newCard.transform.SetParent(deckPoint, true);
        newCard.RectTransform.anchoredPosition = cardSpawnPoint.anchoredPosition;
        newCard.OnAttributeChanged += OnCardAttributeChanged;
        newCard.ParentDeck = this;
        imageDownloader.GetRandomImage(newCard.ImageDimensions, (texture) => { newCard.SetImageFromTexture(texture); imagesToDownload--; CheckIfImagesDownloaded(ArrangeCards); });
        activeCards.Add(newCard);
    }

    private void CheckIfImagesDownloaded(Action onAllDownloaded)
    {
        if (imagesToDownload == 0)
        {
            onAllDownloaded?.Invoke();
        }
    }

    private void ArrangeCards()
    {
        //Ќачинаем двигать если есть карты и если нет движущихс€ карт
        if (activeCards.Count > 0)
        {
            movingCards.ForEach(seq => seq.Kill());
            movingCards.Clear();
            Vector2 cardSize = activeCards[0].RectTransform.sizeDelta;
            //1 если количество карт четное, 0 если нечетное
            int evenMod = (1 - activeCards.Count % 2);
            //–ассто€ние от одной карты до следующей
            float cardDistanceX = Mathf.Min(cardArea.sizeDelta.x / activeCards.Count, cardSize.x) + cardGap;
            //Ќасто€щий размер области карты - либо нужна€ область, либо максимально больша€ выделенна€
            float realAreaSizeX = activeCards.Count * (cardDistanceX - evenMod);
            //Ќачальна€ точка расположени€ карт
            float offsetX = (cardDistanceX - realAreaSizeX) / 2;
            float evaluation, offsetFromCard;
            for (int i = 0; i < activeCards.Count; i++)
            {
                offsetFromCard = i * cardDistanceX;
                evaluation = placementCurve.Evaluate((offsetFromCard + cardDistanceX / 2) / realAreaSizeX);
                Vector3 newCardPos = new Vector3(cardArea.anchoredPosition.x + offsetX + offsetFromCard, evaluation * cardSize.y + cardArea.anchoredPosition.y);
                Vector3 newCardRot = new Vector3(0, 0, rotationOffset - rotationPerCard * (i - activeCards.Count / 2) - rotationPerCard * evenMod / 2);
                Sequence moveSeq = DOTween.Sequence();
                movingCards.Add(moveSeq);
                moveSeq.Append(activeCards[i].RectTransform.DOAnchorPos(newCardPos, cardMoveSpeed).SetEase(movementCurve));
                moveSeq.Join(activeCards[i].RectTransform.DORotate(newCardRot, cardMoveSpeed).SetEase(movementCurve));
                moveSeq.onComplete += () => movingCards.Remove(moveSeq);
                moveSeq.Play();
            }
        }
    }

    private void OnCardAttributeChanged(Card card, Attribute attr)
    {
        switch (attr.AttributeType)
        {
            case AttributeType.Health:
                if (card.Health <= 0)
                {
                    DestroyCard(card);
                }
                break;
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator RandomChangeCoroutine()
    {
        while (isActiveAndEnabled && activeCards.Count > 0)
        {
            for (int i = 0; i < activeCards.Count; i++)
            {
                int a = Random.Range(0, 3);
                int change = Random.Range(-MIN_RANDOM_CHANGE, MAX_RANDOM_CHANGE + 1);
                switch (a)
                {
                    case 0:
                        activeCards[i].Health = change;
                        break;
                    case 1:
                        activeCards[i].Mana = change;
                        break;
                    case 2:
                        activeCards[i].Attack = change;
                        break;
                }
                yield return randomStatChangeTimer;
            }
        }
    }
    #endregion
}

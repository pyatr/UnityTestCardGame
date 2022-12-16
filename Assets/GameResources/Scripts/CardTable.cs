using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTable : MonoBehaviour
{
    [SerializeField]
    private RectTransform cardPosition;
    [SerializeField]
    private float cardSnapDuration = 0.7f;

    private List<Card> hoveringCards = new List<Card>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Card card))
        {
            hoveringCards.Add(card);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Card card) && hoveringCards.Contains(card))
        {
            hoveringCards.Remove(card);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            for (int i = 0; i < hoveringCards.Count; i++)
            {
                if (hoveringCards[i].ParentDeck)
                {
                    hoveringCards[i].ParentDeck.RemoveCard(hoveringCards[i]);
                    hoveringCards[i].ParentDeck = null;
                    hoveringCards[i].RectTransform.SetParent(transform);
                    Sequence moveSeq = DOTween.Sequence();
                    moveSeq.Append(hoveringCards[i].RectTransform.DOAnchorPos(cardPosition.anchoredPosition, cardSnapDuration));
                    moveSeq.Join(hoveringCards[i].RectTransform.DORotate(Vector3.zero, cardSnapDuration));
                    moveSeq.Play();
                    hoveringCards[i].GetComponent<BoxCollider2D>().enabled = false;
                }
            }
        }
    }
}

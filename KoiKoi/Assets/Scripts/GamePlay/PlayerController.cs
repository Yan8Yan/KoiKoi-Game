using UnityEngine;
using System.Collections.Generic;
using KoiKoiProject;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public HandController3D handController;

    [Header("Captured Cards")]
    public List<Card> kasu = new List<Card>();
    public List<Card> tanzaku = new List<Card>();
    public List<Card> tane = new List<Card>();
    public List<Card> hikari = new List<Card>();

    [SerializeField] CardCaptureManager cardCaptureManager;

    public int roundScore;
    public int matchScore;

    public void AddCapturedCard(Card card)
    {
        switch (card.cardType)
        {
            case Card.CardType.kasu:
                kasu.Add(card);

                break;

            case Card.CardType.tanzaku:
                tanzaku.Add(card);
                break;

            case Card.CardType.tane:
                tane.Add(card);
                break;

            case Card.CardType.hikari:
                hikari.Add(card);
                break;
        }

        Debug.Log($"{gameObject.name} captured {card.cardType}");
    }

    public void CheckForYaku()
    {
        int points = YakuChecker.CheckYaku(this);

        if (points > 0)
        {
            Debug.Log(gameObject.name + " has Yaku! Points: " + points);
            UIManager.Instance.ShowKoiKoi();
        }

        roundScore += points;

        Debug.Log($"У {gameObject.name} очков в раунде: {roundScore}");
    }

    public void ResetCapturedCards()
    {
        kasu.Clear();
        tanzaku.Clear();
        tane.Clear();
        hikari.Clear();

        cardCaptureManager.ResetCapturedCardTransder();
    }

    public void ResetRoundScore()
    {
        roundScore = 0;
    }

    public void ResetMatchScore()
    {
        matchScore = 0;
    }

}
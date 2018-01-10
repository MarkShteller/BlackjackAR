using Blackjack.Common;
using Blackjack.GameManager;
using Blackjack.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour {

    private Manager logicManager;
    public Player player;


    public GameObject GameHolder;
    public GameObject ChooseBetHolder;
    public GameObject CardPrefab;
    public GameObject PlayerHandHolder;
    public GameObject DealerHandHolder;
    public GameObject WinParticles;

    private List<GameObject> cardRefs;
    private GameObject downfacedCard;

    private int cardCount = 0;
    private int cardCountDealer = 0;

    private int betAmount = 10;

    private int playerBalance;
    public int PlayerBalance
    {
        get
        {
            return playerBalance;
        }

        set
        {
            playerBalance = value;
        }
    }
    [HideInInspector]
    public static GameManager Instance;

    

    // Use this for initialization
    void Start ()
    {
        Instance = this;
        this.player = new Player();
        logicManager = new Manager(player);
        cardRefs = new List<GameObject>();

        PlayerBalance = 500;
        

        downfacedCard = null;

        logicManager.DealFirstTwoCards();
        Debug.Log("Players hand:\n" + player.ShoutHand());

        
    }

    private IEnumerator PlaceBegginingCards()
    {
        List<Card> hand = player.ShowHand();
        for (int i = 0; i < hand.Count; i++)
        {
            PlaceCard(hand[i], PlayerHandHolder, true, false);
            yield return new WaitForSeconds(1);
        }

        List<Card> hand2 = logicManager.getDealerCards();
        for (int i = 0; i < hand.Count; i++)
        {
            PlaceCard(hand2[i], DealerHandHolder, false, i == 1);
            yield return new WaitForSeconds(1);
        }
    }


    public void LoadMainStage(string betTag)
    {
        GUIManager.Instance.UpdateBalance();
        GUIManager.Instance.ShowActionButtons();

        string str = betTag.Substring(4);
        betAmount = int.Parse(str);

        ChooseBetHolder.SetActive(false);
        GameHolder.SetActive(true);

        StartCoroutine(PlaceBegginingCards());
    }

    private void PlaceCard(Card logicCard, GameObject pivot, bool isPlayerCard, bool isFacedDown)
    {
        GameObject cardHolder = Instantiate(new GameObject(), pivot.transform);
        GameObject card = Instantiate(CardPrefab, cardHolder.transform);

        if (isPlayerCard)
        {
            cardHolder.transform.localPosition = new Vector3(cardCount * 0.2f, 0.01f + cardCount * 0.01f, 0);
        }
        else
        {
            cardHolder.transform.localPosition = new Vector3(cardCountDealer * 0.2f, 0.01f + cardCountDealer * 0.01f, 0);
            cardHolder.transform.eulerAngles = new Vector3(0, 180, 0);
        }

        string textureName = "PNG Cards/" + (logicCard.Value + "_of_" + logicCard.Suit).ToLower();
        Debug.Log("texture name: " + textureName);
        Texture tex = Resources.Load<Texture>(textureName);
        card.GetComponent<Renderer>().material.mainTexture = tex;

        if (isFacedDown)
        {
            cardHolder.transform.eulerAngles = new Vector3(0, cardHolder.transform.eulerAngles.y, 180);
            downfacedCard = cardHolder;
        }

        cardRefs.Add(card);

        card.GetComponent<Animator>().enabled = true;

        if (isPlayerCard)
            cardCount++;
        else
            cardCountDealer++;
    }

    public void FlipDownfacedCard()
    {
        downfacedCard.transform.eulerAngles = Vector3.zero;
    }


    public void Restart()
    {
        //discard from old
        foreach (var card in cardRefs)
        {
            Destroy(card);
        }
        cardCount = 0;
        cardCountDealer = 0;
        downfacedCard = null;
        WinParticles.SetActive(false);

        //set new
        playerBalance -= betAmount;
        GUIManager.Instance.UpdateBalance();

        logicManager.StartNewDeal();
        /*List<Card> hand = player.ShowHand();
        for (int i = 0; i < hand.Count; i++)
        {
            PlaceCard(hand[i], PlayerHandHolder, true, false);
        }

        List<Card> hand2 = logicManager.getDealerCards();
        for (int i = 0; i < hand.Count; i++)
        {
            PlaceCard(hand2[i], DealerHandHolder, false, i == 1);
        }*/

        StartCoroutine(PlaceBegginingCards());
    }

    public void Hit()
    {
        Debug.Log("HIT");
        player.HitMe(logicManager.DealCard());

        PlaceCard(player.LastCard(), PlayerHandHolder, true, false);

        if (player.GetScore() > 21)
        {
            //Player lost
            GUIManager.Instance.ShowPlayerLostMessage();
        }
    }

    public void Stand()
    {
        Debug.Log("stand");

        FlipDownfacedCard();

        while(logicManager.GetDealerScore() <= 16)
        {
            logicManager.GiveDealerACard();
            PlaceCard(logicManager.DealerLastCard, DealerHandHolder, false, false);
        }

        if (logicManager.GetDealerScore() > 21)
        {
            WinParticles.SetActive(true);
            GUIManager.Instance.ShowPlayerWonMessage();
            playerBalance += betAmount * 2;
            GUIManager.Instance.UpdateBalance();
        }
        else
        {
            if (player.GetScore() > logicManager.GetDealerScore())
            {
                WinParticles.SetActive(true);
                GUIManager.Instance.ShowPlayerWonMessage();
                playerBalance += betAmount * 2;
                GUIManager.Instance.UpdateBalance();
            }
            else if (player.GetScore() == logicManager.GetDealerScore())
            {
                GUIManager.Instance.ShowTie();
                playerBalance += betAmount;
                GUIManager.Instance.UpdateBalance();
            }
            else
            {
                GUIManager.Instance.ShowPlayerLostMessage();
            }
        }
    }

    public void Double()
    {
        Debug.Log("double");
        Hit();
        Stand();
    }

    public void Surrender()
    {
        Restart();
    }

}

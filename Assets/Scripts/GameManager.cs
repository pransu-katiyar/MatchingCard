using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text turnsText;
    public TMP_Text matchesText;

    [Header("Cards Setup")]
    public List<Card> allCards;          
    public List<Sprite> cardSymbols;     

    private Card firstCard;
    private Card secondCard;

    public bool IsBusy { get; private set; } = false;

    private int turns = 0;
    private int matches = 0;

    [SerializeField] private AudioController audioController;
    [SerializeField] private GameOverScreen gameOverScreen;

    void Start()
    {
        if (PlayerPrefs.HasKey("Turns"))
        {
            LoadProgress();
        }
        else
        {
            SetupCards();
        }
        StartCoroutine(ShowAllCardsBriefly(allCards));

    }

    private void SetupCards()
    {
        List<Sprite> symbolsPool = new List<Sprite>();

        foreach (var symbol in cardSymbols)
        {
            symbolsPool.Add(symbol);
            symbolsPool.Add(symbol);
        }

        if (!PlayerPrefs.HasKey("CardMapping"))
        {
            for (int i = 0; i < symbolsPool.Count; i++)
            {
                Sprite temp = symbolsPool[i];
                int randomIndex = Random.Range(i, symbolsPool.Count);
                symbolsPool[i] = symbolsPool[randomIndex];
                symbolsPool[randomIndex] = temp;
            }

            SaveCardMapping(symbolsPool);
        }

        string savedMapping = PlayerPrefs.GetString("CardMapping");
        string[] mappingNames = savedMapping.Split(',');
        for (int i = 0; i < allCards.Count; i++)
        {
            allCards[i].cardId = i;
            Sprite sp = cardSymbols.Find(s => s.name == mappingNames[i]);
            if (sp != null)
                allCards[i].frontImage.sprite = sp;

            allCards[i].Hide();
            allCards[i].gameObject.SetActive(true);
        }
        if (PlayerPrefs.HasKey("Turns"))
        {
            turns = PlayerPrefs.GetInt("Turns");
            matches = PlayerPrefs.GetInt("Matches");
            turnsText.text = "Turns: " + turns;
            matchesText.text = "Matches: " + matches;

        }
    }

    private void SaveCardMapping(List<Sprite> symbolsPool)
    {
        string mapping = "";
        foreach (var sp in symbolsPool)
        {
            mapping += sp.name + ",";
        }
        PlayerPrefs.SetString("CardMapping", mapping);
        PlayerPrefs.Save();
    }

    public void CardRevealed(Card card)
    {
        audioController.PlayFlip();

        if (firstCard == null)
            firstCard = card;
        else if (secondCard == null)
        {
            secondCard = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        IsBusy = true;
        turns++;
        turnsText.text = "Turns: " + turns;
        SaveProgress();

        yield return new WaitForSeconds(0.8f);

        if (firstCard.frontImage.sprite == secondCard.frontImage.sprite)
        {
            matches++;
            matchesText.text = "Matches: " + matches;
            audioController.PlayMatch();

            firstCard.gameObject.SetActive(false);
            secondCard.gameObject.SetActive(false);

            SaveProgress();
            CheckGameOver();
        }
        else
        {
            audioController.PlayMismatch();
            firstCard.Hide();
            secondCard.Hide();
        }

        firstCard = null;
        secondCard = null;
        IsBusy = false;
    }

    private void CheckGameOver()
    {
        if (matches == cardSymbols.Count)
        {
            audioController.PlayGameOver();
            gameOverScreen.ShowGameOver();

            PlayerPrefs.DeleteKey("Turns");
            PlayerPrefs.DeleteKey("Matches");
            PlayerPrefs.DeleteKey("MatchedCards");
            PlayerPrefs.DeleteKey("CardMapping");
        }
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("Turns", turns);
        PlayerPrefs.SetInt("Matches", matches);

        string matchedCards = "";
        for (int i = 0; i < allCards.Count; i++)
        {
            if (!allCards[i].gameObject.activeSelf)
                matchedCards += i + ",";
        }
        PlayerPrefs.SetString("MatchedCards", matchedCards);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        turns = PlayerPrefs.GetInt("Turns", 0);      
        matches = PlayerPrefs.GetInt("Matches", 0);

        turnsText.text = "Turns: " + turns;
        matchesText.text = "Matches: " + matches;

        string mapping = PlayerPrefs.GetString("CardMapping");
        if (!string.IsNullOrEmpty(mapping))
        {
            string[] symbolNames = mapping.Split(',');
            int index = 0;
            foreach (var card in allCards)
            {
                if (index >= symbolNames.Length) break;

                Sprite sp = cardSymbols.Find(s => s.name == symbolNames[index]);
                if (sp != null)
                    card.frontImage.sprite = sp;

                card.Hide();
                card.gameObject.SetActive(true);
                index++;
            }
        }

        string matchedCards = PlayerPrefs.GetString("MatchedCards");
        if (!string.IsNullOrEmpty(matchedCards))
        {
            string[] indices = matchedCards.Split(',');
            foreach (var s in indices)
            {
                if (int.TryParse(s, out int idx))
                {
                    if (idx >= 0 && idx < allCards.Count)
                        allCards[idx].gameObject.SetActive(false);
                }
            }
        }
    }

    public IEnumerator ShowAllCardsBriefly(List<Card> cards)
    {
        foreach (var card in cards)
        {
            if (card.gameObject.activeSelf)
                card.Flip();
        }

        yield return new WaitForSeconds(2f);

        foreach (var card in cards)
        {
            if (card.gameObject.activeSelf)
                card.ShowBack();
        }
    }

    public void RestartGame(bool isFreshGame = true)
    {
        if (isFreshGame)
        {
            PlayerPrefs.DeleteKey("Turns");
            PlayerPrefs.DeleteKey("Matches");
            PlayerPrefs.DeleteKey("MatchedCards");
            PlayerPrefs.DeleteKey("CardMapping");

            turns = 0;
            matches = 0;
            turnsText.text = "Turns: 0";
            matchesText.text = "Matches: 0";

            SetupCards();
        }
        else
        {
            LoadProgress();
        }

        foreach (var card in allCards)
        {
            if (card.gameObject.activeSelf)
                card.ShowBack();
        }

        StartCoroutine(ShowAllCardsBriefly(allCards));
    }
}

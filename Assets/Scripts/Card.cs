using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public int cardId;               
    public Image frontImage;         
    public Image backImage;          

    private bool isFlipped = false;
    private bool isMatched = false; 
    private bool isAnimating = false; 

    private GameManager gameManager;
    [SerializeField] private Button cardButton;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        ShowBack();
        cardButton.onClick.AddListener(OnCardClicked);
    }

    public void OnCardClicked()
    {
        if (isFlipped || isMatched || isAnimating || gameManager.IsBusy) return;

        Flip();
        gameManager.CardRevealed(this);
    }

    public void Flip()
    {
        isFlipped = true;
        isAnimating = true;

        transform.DORotate(new Vector3(0, 90, 0), 0.25f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            backImage.gameObject.SetActive(false);
            frontImage.gameObject.SetActive(true);

            transform.DORotate(Vector3.zero, 0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                isAnimating = false;
            });
        });

        FindObjectOfType<AudioController>().PlayFlip();
    }

    
    public void Hide()
    {
        isFlipped = false;
        isAnimating = true;

        transform.DORotate(new Vector3(0, 90, 0), 0.25f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            frontImage.gameObject.SetActive(false);
            backImage.gameObject.SetActive(true);

            transform.DORotate(Vector3.zero, 0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                isAnimating = false;
            });
        });
    }

    public void ShowBack()
    {
        isFlipped = false;
        isAnimating = false;
        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
        transform.localRotation = Quaternion.identity;
    }

    public void Remove()
    {
        isMatched = true;
        gameObject.SetActive(false);
    }
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverPanel;
    public Button replayButton;

    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        Vector3 startPos = new Vector3(0, 400, 0); 
        Vector3 midPos = new Vector3(startPos.x, -400f, startPos.z); 
        Vector3 endPos = new Vector3(startPos.x, 0f, startPos.z);  

        Sequence jumpSequence = DOTween.Sequence();

        jumpSequence.Append(replayButton.transform.DOLocalMove(midPos, 0.6f).SetEase(Ease.InQuad));

        jumpSequence.Append(replayButton.transform.DOLocalJump(endPos, 400f, 1, 1f).SetEase(Ease.OutQuad));

        replayButton.onClick.RemoveAllListeners();
        replayButton.onClick.AddListener(OnReplayClicked);
    }

    private void OnReplayClicked()
    {
        gameOverPanel.SetActive(false);

        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        replayButton.gameObject.transform.localPosition = new Vector3(0, 400, 0); 
    }
}

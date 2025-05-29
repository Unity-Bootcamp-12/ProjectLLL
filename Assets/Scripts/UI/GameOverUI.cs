using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private FadeInPanel _gameWinPanel;
    [SerializeField] private FadeInPanel _gameLosePanel;

    private void Start()
    {
        _gameWinPanel.gameObject.SetActive(false);
        _gameLosePanel.gameObject.SetActive(false);
    }

    public void ShowGameWinPanel()
    {
        _gameLosePanel.gameObject.SetActive(false);
        _gameWinPanel.FadeIn();
    }

    public void ShowGameLosePanel()
    {
        _gameWinPanel.gameObject.SetActive(false);
        _gameLosePanel.FadeIn();
    }
}

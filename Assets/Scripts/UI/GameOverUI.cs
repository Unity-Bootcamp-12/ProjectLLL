using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject _gameWinPanel;
    [SerializeField] private GameObject _gameLosePanel;

    private void Start()
    {
        _gameWinPanel.SetActive(false);
        _gameLosePanel.SetActive(false);
    }

    public void ShowGameWinPanel()
    {
        _gameWinPanel.SetActive(true);
        _gameLosePanel.SetActive(false);
    }

    public void ShowGameLosePanel()
    {
        _gameWinPanel.SetActive(false);
        _gameLosePanel.SetActive(true);
    }
}

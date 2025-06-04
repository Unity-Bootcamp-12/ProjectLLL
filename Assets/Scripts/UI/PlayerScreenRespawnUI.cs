using TMPro;
using UnityEngine;

public class PlayerScreenRespawnUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private void Start()
    {
        _text = GetComponentInChildren<TMP_Text>();
    }

    public void EnableRespawnPanel(float respawnTime)
    {
        gameObject.SetActive(true);
        _text.text = Mathf.CeilToInt(respawnTime).ToString();
    }

    public void UpdateRespawnTimer(float remainingSeconds)
    {
        _text.text = Mathf.CeilToInt(remainingSeconds).ToString();
    }

    public void DisableRespawnPanel()
    {
        gameObject.SetActive(false);
    }
}

using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;
    [SerializeField] private Button _clientJoinButton;
    [SerializeField] private TMP_InputField _ipInputField;

    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _hostPanel;
    [SerializeField] private GameObject _clientPanel;

    private void Start()
    {
        _mainPanel.SetActive(true);
        _hostPanel.SetActive(false);
        _clientPanel.SetActive(false);

        _hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = "0.0.0.0";
            NetworkManager.Singleton.StartHost();
            _mainPanel.SetActive(false);
            _hostPanel.SetActive(true);
        });

        _clientButton.onClick.AddListener(() =>
        {
            _mainPanel.SetActive(false);
            _clientPanel.SetActive(true);
        });

        _clientJoinButton.onClick.AddListener(() =>
        {
            string ip = _ipInputField.text;
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ip;

            NetworkManager.Singleton.StartClient();
            gameObject.SetActive(false);
        });
    }
}

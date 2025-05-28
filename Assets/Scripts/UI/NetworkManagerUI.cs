using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] Button _hostButton;
    [SerializeField] Button _clientButton;
    [SerializeField] TMP_InputField _ipInputField;

    private void Start()
    {
        _hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = "0.0.0.0";
            NetworkManager.Singleton.StartHost();
            gameObject.SetActive(false);
        });

        _clientButton.onClick.AddListener(() =>
        {
            string ip = _ipInputField.text;
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ip;

            NetworkManager.Singleton.StartClient();
            gameObject.SetActive(false);
        });
    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement _playerMovement;

	private void Awake()
    {
		_playerMovement = GetComponent<PlayerMovement>();
    }
}

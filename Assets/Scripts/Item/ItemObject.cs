using Unity.Netcode;
using UnityEngine;

public class ItemObject : NetworkBehaviour
{
    [SerializeField] private ItemScriptableObject _item;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void Init()
    { 
        NetworkObject.Spawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerController>(out var player))
            {
                player.AddItem(_item);
                NetworkObject.Despawn();
            }
        }
    }

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
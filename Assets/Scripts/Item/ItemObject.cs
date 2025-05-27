using Unity.Netcode;
using UnityEngine;

public class ItemObject : NetworkBehaviour
{
    [SerializeField] private ItemScriptableObject _item;

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
                if (!player.IsOwner)
                {
                    return;
                }

                player.AddItem(new ItemData(_item, GetComponent<IUsableItem>()));
                RemoveItemRpc();
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void RemoveItemRpc()
    {
        NetworkObject.Despawn();
    }

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class NetworkParticle : NetworkBehaviour
{
    [SerializeField] private float _destroyTime = 3.0f;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(DestroyCoroutine());
        }
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(_destroyTime);
        NetworkObject.Despawn();
        Destroy(gameObject);
    }
}

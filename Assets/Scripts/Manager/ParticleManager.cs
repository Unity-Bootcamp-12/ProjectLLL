using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum ParticleType
{
    PlayerResurrection,
    TowerDestruction,
    PlayerHeal,
    PlayerBlink,
    PlayerPickItem,
    PlayerHit,
}

public class ParticleManager : NetworkBehaviour
{
    public static ParticleManager Instance { get; private set; }

    [SerializeField] private List<GameObject> _particleList;

    private Dictionary<ParticleType, GameObject> _particleDic = new Dictionary<ParticleType, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            for (int i = 0; i < _particleList.Count; i++)
            {
                _particleDic.Add((ParticleType)i, _particleList[i]);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayParticleServerRpc(ParticleType type, Vector3 position)
    {
        if (!_particleDic.ContainsKey(type)) return;

        GameObject particle = Instantiate(_particleDic[type], position, Quaternion.identity);
        NetworkObject networkObject = particle.GetComponent<NetworkObject>();
        networkObject.Spawn();

        Animator animator = particle.GetComponent<Animator>();
        if (animator != null) StartCoroutine(DestroyAfterAnimation(networkObject, animator));
        else StartCoroutine(DestroyAfterTime(networkObject, 3.0f));
    }

    private IEnumerator DestroyAfterAnimation(NetworkObject networkObject, Animator animator)
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        networkObject.Despawn();
        Destroy(networkObject.gameObject);
    }

    private IEnumerator DestroyAfterTime(NetworkObject networkObject, float time)
    {
        yield return new WaitForSeconds(time);
        networkObject.Despawn();
        Destroy(networkObject.gameObject);
    }
}

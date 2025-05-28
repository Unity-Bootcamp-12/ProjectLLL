using System.Collections.Generic;
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

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    [SerializeField] private List<GameObject> _particleList;
    [SerializeField] private float _defualtDestroyTime = 3.0f;

    private Dictionary<ParticleType, GameObject> particleSystemDic = new Dictionary<ParticleType, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < _particleList.Count; i++)
        {
            particleSystemDic.Add((ParticleType)i, _particleList[i]);
        }
    }

    public void ParticlePlay(ParticleType type, Vector3 position)
    {
        GameObject particleObj = Instantiate(particleSystemDic[type]);
        particleObj.transform.position = position;
        particleObj.SetActive(true);

        Animator animator = particleObj.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(0);
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            Destroy(particleObj, stateInfo.length);
        }
        else
        {
            Destroy(particleObj, _defualtDestroyTime);
        }
    }
}

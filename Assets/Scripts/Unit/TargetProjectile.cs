using Unity.Netcode;
using UnityEngine;

public class TargetProjectile : NetworkBehaviour
{
    [SerializeField] private bool _isPlayer = false;

    private Transform _target;
    private float _speed = 0.0f;
    private float _damage = 0.0f;

    public void Init(Transform target, float speed, float damage)
    {
        _target = target;
        _speed = speed;
        _damage = damage;

        NetworkObject.Spawn();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (_target == null)
        {
            NetworkObject.Despawn();
            return;
        }

        Vector3 direction = (_target.position - transform.position).normalized;
        transform.position += direction * _speed * Time.deltaTime;

        transform.LookAt(_target);

        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance < 0.2f)
        {
            if (_target.TryGetComponent<UnitController>(out var unit))
            {
                unit.ReceiveDamage(_damage);
                if (_isPlayer == true)
                {
                    ParticleManager.Instance.PlayParticleServerRpc(ParticleType.PlayerHit, _target.position);
                }
            }

            NetworkObject.Despawn();
        }
    }
}

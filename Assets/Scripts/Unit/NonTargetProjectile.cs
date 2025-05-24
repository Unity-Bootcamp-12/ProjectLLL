using Unity.Netcode;
using UnityEngine;

public class NonTargetProjectile : NetworkBehaviour
{
    private float _speed = 0.0f;
    private float _damage = 0.0f;
    private float _lifeTime = 5f;
    private UnitTeamType _teamType;

    private float _timer = 0f;

    public void Init(Vector3 destination, float speed, float damage, float lifeTime, UnitTeamType teamType)
    {
        Vector3 direction = (destination - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        _speed = speed;
        _damage = damage;
        _lifeTime = lifeTime;
        _teamType = teamType;

        NetworkObject.Spawn();
    }

    private void Update()
    {
        if (!IsServer) 
        {
            return; 
        }

        _timer += Time.deltaTime;
        if (_timer > _lifeTime)
        {
            NetworkObject.Despawn();
        }

        transform.position += _speed * Time.deltaTime * transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Unit"))
        {
            if (other.gameObject.TryGetComponent<UnitController>(out var unit))
            {
                if (unit.TeamType != _teamType)
                {
                    unit.ReceiveDamage(_damage);
                    NetworkObject.Despawn();
                }
            }
        }
    }
}

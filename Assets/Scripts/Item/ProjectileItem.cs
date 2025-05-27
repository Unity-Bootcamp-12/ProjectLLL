using Unity.Netcode;
using UnityEngine;

public class ProjectileItem : MonoBehaviour, IUsableItem
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private LayerMask _groundLayerMask;

    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private float _damage = 60.0f;
    [SerializeField] private float _lifeTime = 3.0f;

    public void Use(PlayerController player)
    {
        Logger.Info("USE PROJECTILE");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit groundHit, 100f, _groundLayerMask))
        {
            Vector3 spawnPosition = groundHit.point;
            spawnPosition.y = player.transform.position.y;

            player.FireNonTargetProjectileRpc(spawnPosition,
                _speed, _damage, _lifeTime);
        }
    }
}

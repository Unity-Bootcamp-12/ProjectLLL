using Unity.Netcode;
using UnityEngine;

public class BlinkItem : MonoBehaviour, IUsableItem
{
    [SerializeField] private float _moveDistance = 6.0f;
    [SerializeField] private LayerMask _groundLayerMask;

    public void Use(PlayerController player)
    {
        Logger.Info("USE BLINK");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit groundHit, 100f, _groundLayerMask))
        {
            Vector3 playerPosition = player.transform.position;
            Vector3 direction = (groundHit.point - playerPosition).normalized;

            Vector3 targetPosition = playerPosition + direction * _moveDistance;

            player.BlinkToRpc(targetPosition);
        }
    }
}

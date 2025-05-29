using UnityEngine;

public class HealItem : MonoBehaviour, IUsableItem
{
    [SerializeField] private float _healAmount = 30.0f;

    public void Use(PlayerController player)
    {
        Logger.Info("USE HEAL");

        player.ReceiveHeal(_healAmount);
        ParticleManager.Instance.PlayParticleServerRpc(ParticleType.PlayerHeal, player.transform.position);
    }
}

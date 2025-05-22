using UnityEngine;

public enum ItemType
{
    AttackPower,
    AttackSpeed,
    AttackRange,
    MoveSpeed,
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class ItemScriptableObject : ScriptableObject
{
    [Header("아이템의 고유 ID")]
    public int ItemID => _itemID;
    [SerializeField] private int _itemID;

    [Header("아이템의 타입")]
    public ItemType ItemType => _itemType;
    [SerializeField] private ItemType _itemType;

    [Header("아이템의 이미지")]
    public Sprite ItemSprite => _itemSprite;
    [SerializeField] private Sprite _itemSprite;

    [Header("아이템의 스탯 증가량")]
    [Tooltip("AttackPower : 공격력, AttackSpeed : 공격 속도, AttackRange : 공격 범위, MoveSpeed : 이동 속도")]
    public float UpgradeValue => _upgradeValue;
    [SerializeField] private float _upgradeValue;

    public virtual void Use(PlayerController player)
    {
        var statControl = player.GetComponent<HPController>();
        if(statControl == null)
        {
            Logger.Error("[ItemSO] HPController를 찾을 수 없음. on " + player.name);
            return;
        }

        switch(ItemType)
        {
            case ItemType.AttackPower:
                statControl.SetAttackPowerRpc(UpgradeValue);
                break;
            case ItemType.AttackSpeed:
                statControl.SetAttackSpeedRpc(UpgradeValue);
                break;
            case ItemType.AttackRange:
                statControl.SetAttackRangeRpc(UpgradeValue);
                break;
            case ItemType.MoveSpeed:
                statControl.SetMoveSpeedRpc(UpgradeValue);
                break;
        }
        Logger.Info($"[ItemSO] {name}({ItemType} 사용 : {UpgradeValue} 적용함.");
    }
}

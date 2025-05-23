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
    [Header("아이템의 타입")]
    public ItemType ItemType => _itemType;
    [SerializeField] private ItemType _itemType;

    [Header("아이템의 이미지")]
    public Sprite ItemSprite => _itemSprite;
    [SerializeField] private Sprite _itemSprite;

    [Header("아이템의 스탯 증가량")]
    public UnitStatus UpgradeStatus => _upgradeStatus;
    [SerializeField] private UnitStatus _upgradeStatus;
}
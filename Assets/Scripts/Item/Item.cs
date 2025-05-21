using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum ItemType
{
    None = 0b0,
    AttackPower = 0b1,
    AttackSpeed = 0b10,
    AttackRange = 0b100,
    MoveSpeed = 0b1000,
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class Item : ScriptableObject
{
    [Header("아이템의 고유 ID")]
    public int ItemID => _itemID;
    [SerializeField] private int _itemID;

    [Header("아이템의 타입")]
    public ItemType ItemType => _itemType;
    [SerializeField] private ItemType _itemType;

    [Header("씬에서 오브젝트로 보여질 아이템의 프리팹")]
    public GameObject ItemPrefab => _itemPrefab;
    [SerializeField] private GameObject _itemPrefab;
}

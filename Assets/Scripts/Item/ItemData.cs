using UnityEngine;

public class ItemData
{
    public ItemScriptableObject ItemSO;
    private IUsableItem _itemEffect;

    public ItemData(ItemScriptableObject itemSO, IUsableItem itemEffect)
    {
        ItemSO = itemSO;
        _itemEffect = itemEffect;
    }

    public void Use(PlayerController player)
    {
        _itemEffect.Use(player);
    }
}

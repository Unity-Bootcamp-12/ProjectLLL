using UnityEngine;

public class ItemConsumable : MonoBehaviour
{
    [Header("해당 오브젝트에 할당되는 아이템")]
    public ItemScriptableObject Item => _item;
    [SerializeField] private ItemScriptableObject _item;

    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = Item.ItemSprite;
    }
}

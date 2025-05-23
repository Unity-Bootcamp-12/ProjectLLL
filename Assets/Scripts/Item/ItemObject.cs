using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemScriptableObject _item;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer.sprite = _item.ItemSprite;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerController>(out var player))
            {
                if (!player.IsItemFull())
                {
                    player.AddItem(_item);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
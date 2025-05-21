using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [Header("해당 오브젝트에 할당되는 아이템")]
    public Item Item => _item;
    [SerializeField] private Item _item;

    [Header("해당 아이템 정보를 가져올 때, 아이템의 크기가 다르기 때문에 높이 지정")]
    public float IndicatorHeight => _indicatorHeight;
    [SerializeField] private float _indicatorHeight;
}

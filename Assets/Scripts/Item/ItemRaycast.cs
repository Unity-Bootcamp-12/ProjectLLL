using System.Collections;
using UnityEngine;

public class ItemRaycast : MonoBehaviour
{
    private RaycastHit _hit; // 레이캐스트 된 아이템

    [SerializeField] private float _rayDistance; // 레이캐스트 거리

    private bool _isPickUpActive = false; // 아이템의 획득이 가능한가?
    private ItemPickUp _currentItem; // 활성화시 현재 등록된 아이템

    [Header("레이캐스트 쏠 카메라")]
    [SerializeField] private Camera _rayCamera;

    private void Update()
    {
        CheckItem();

        if(_isPickUpActive)
        {
            TryInteract();
        }
    }

    /// <summary>
    /// 아이템을 주울 수 있는지 확인
    /// </summary>
    private void TryInteract()
    {
        if(Input.GetKeyDown(KeyCode.F) && _currentItem != null)
        {
            Destroy(_currentItem.gameObject);

            Logger.Info($"상호작용한 아이템 이름 : {_currentItem.Item.name}");

            ClearCurrentItem();
        }
    }

    /// <summary>
    /// 레이캐스트로 아이템 확인
    /// </summary>
    private void CheckItem()
    {
        if(Physics.Raycast(_rayCamera.transform.position, _rayCamera.transform.forward
            , out _hit, _rayDistance))
        {
            var hitTransform = _hit.transform;
            if(hitTransform.CompareTag("Item"))
            {
                var item = hitTransform.GetComponent<ItemPickUp>();
                if (item == null)
                    return;

                if(_currentItem != item)
                {
                    _currentItem = item;
                    _isPickUpActive = true;

                    Logger.Info($"상호작용 가능 : {_currentItem.Item.name}");
                }
                return;
            }
        }
        // 레이캐스트가 안닿고 태그가 아이템이 아니면 초기화
        ClearCurrentItem();
    }

    /// <summary>
    ///  현재 아이템 정보 초기화
    /// </summary>
    private void ClearCurrentItem()
    {
        _isPickUpActive = false;
        _currentItem = null;
    }
}

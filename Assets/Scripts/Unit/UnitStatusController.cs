using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class UnitStatusController : MonoBehaviour
{
    /// <summary>
    /// 변하지 않는 오리지널 스탯
    /// </summary>
    [SerializeField] private HeroScriptableObject _heroSO;
    /// <summary>
    /// 변경 스탯치
    /// </summary>
    public UnitStatus ChangeStatus => _changeStatus;
    private UnitStatus _changeStatus;
    private ItemData[] _itemList;

    const int MAX_LEVEL = 9;
    const int MAX_ITEM = 3;

    private void Start()
    {
        _changeStatus = new UnitStatus();
        _itemList = new ItemData[MAX_ITEM];
    }

    public string GetHeroName() => _heroSO.Name;

    #region 스탯 관련
    public float GetMaxHP() => _heroSO.HeroStatus.MaxHP + _changeStatus.MaxHP;
    public float GetAttackPower() => _heroSO.HeroStatus.AttackPower + _changeStatus.AttackPower;
    public float GetAttackSpeed() => _heroSO.HeroStatus.AttackSpeed + _changeStatus.AttackSpeed;
    public float GetAttackRange() => _heroSO.HeroStatus.AttackRange + _changeStatus.AttackRange;
    public float GetMoveSpeed() => _heroSO.HeroStatus.MoveSpeed + _changeStatus.MoveSpeed;
    #endregion

    #region 공격 유형 관련
    public AttackType GetAttackType() => _heroSO.AttackType;
    public GameObject GetProjectilePrefab() => _heroSO.ProjectilePrefab;
    #endregion

    public Sprite GetHeroPortrait() => _heroSO.HeroImage;

    #region 아이템 관련

    public ItemData RemoveAndGetItem(ButtonType buttonType)
    {
        ItemData returnItem = null;

        if (buttonType == ButtonType.Q)
        {
            returnItem = _itemList[0];
            _itemList[0] = null;
        }
        else if (buttonType == ButtonType.W)
        {
            returnItem = _itemList[1];
            _itemList[1] = null;
        }
        else if (buttonType == ButtonType.E)
        {
            returnItem = _itemList[2];
            _itemList[2] = null;
        }

        UpdateChangeStatus();

        return returnItem;
    }

    public bool IsItemListFull()
    {
        for (int i = 0; i < _itemList.Length; i++)
        {
            if (_itemList[i] == null)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsItemSlotEmpty(ButtonType buttonType)
    {
        if (buttonType == ButtonType.Q)
        { 
            return (_itemList[0] == null);
        }
        else if (buttonType == ButtonType.W)
        {
            return (_itemList[1] == null);
        }
        else if (buttonType == ButtonType.E)
        {
            return (_itemList[2] == null);
        }

        return false;
    }

    public void AddItem(ItemData item, ButtonType buttonType)
    {
        if (buttonType == ButtonType.Q)
        {
            _itemList[0] = item;
        }
        else if (buttonType == ButtonType.W)
        {
            _itemList[1] = item;
        }
        else if (buttonType == ButtonType.E)
        {
            _itemList[2] = item;
        }

        UpdateChangeStatus();
    }

    private void UpdateChangeStatus()
    {
        UnitStatus changeStatus = new UnitStatus();

        for (int i = 0; i < MAX_ITEM; i++)
        {
            if (_itemList[i] != null)
            {
                changeStatus += _itemList[i].ItemSO.UpgradeStatus;
            }
        }

        _changeStatus = changeStatus;
        UIManager.Instance.UpdatePlayerStatus(this);
    }
    #endregion
}

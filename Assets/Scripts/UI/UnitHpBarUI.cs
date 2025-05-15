using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitHPBarUI : MonoBehaviour
{
    [SerializeField] private Slider _MyhpBarSlider;
    [SerializeField] private Slider _YouhpBarSlider;
    private PlayerController _playerController;
    public UnitTeamType TeamType;

    public void Init(UnityEvent<float, float> onChangeHPEvent)
    {
        onChangeHPEvent.AddListener(UpdateUI);
    }

    public void UpdateUI(float maxHP, float currentHP)
    {
        if (_playerController == null)
        {
            _MyhpBarSlider.value = currentHP / maxHP;
        }
        //if (TeamType == UnitTeamType.RedTeam)
        //{
        //    _YouhpBarSlider.value = currentHP / maxHP;
        //}
    }

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}

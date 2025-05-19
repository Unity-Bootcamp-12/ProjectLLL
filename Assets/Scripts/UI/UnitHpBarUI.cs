using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitHPBarUI : MonoBehaviour
{
    [SerializeField] private Slider _hpBarSlider;

    public void Init(UnityEvent<UnitTeamType, float, float> onChangeHPEvent)
    {
        onChangeHPEvent.AddListener(UpdateUI);
    }

    public void UpdateUI(UnitTeamType teamType, float maxHP, float currentHP)
    {
        if (_hpBarSlider != null)
        {
            _hpBarSlider.value = currentHP / maxHP;
        }
    }

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}

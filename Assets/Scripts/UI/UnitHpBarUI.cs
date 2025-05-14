using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitHpBarUI : MonoBehaviour
{
    [SerializeField] private Slider _hpBarSlider;

    public void Init(UnityEvent<float, float> onChangeHPEvent)
    {
        onChangeHPEvent.AddListener(UpdateUI);
    }

    //MaxHp를 표시가되고, currentHp로 MaxHP가 줄어드는것을 표현한다. 
    public void UpdateUI(float maxHP, float currentHP)
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

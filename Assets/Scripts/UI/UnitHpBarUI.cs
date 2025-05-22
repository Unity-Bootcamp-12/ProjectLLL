using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitHPBarUI : MonoBehaviour
{
    [SerializeField] private Slider _hpBarSlider;
    private Color _allyColor = Color.green;
    private Color _enemyColor = Color.red;

    public void Init(UnityEvent<float, float> onChangeHPEvent)
    {
        onChangeHPEvent.AddListener(UpdateUI);
    }

    public void UpdateUI(float maxHP, float currentHP)
    {
        if (_hpBarSlider != null && currentHP >= 0 && maxHP != 0)
        {
            _hpBarSlider.value = currentHP / maxHP;
        }
    }

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }

    public void SetHpBarColor(bool isAlly)
    {
        _hpBarSlider.fillRect.GetComponent<Image>().color = isAlly ? _allyColor : _enemyColor;
    }
}

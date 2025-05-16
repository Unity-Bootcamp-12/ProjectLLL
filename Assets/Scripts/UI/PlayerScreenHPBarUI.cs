using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerScreenHPBarUI : MonoBehaviour
{
    [SerializeField] private Slider _heroTopHpBarSlider;

    public void Init(UnityEvent<float, float> onChangeHPEvent)
    {
        onChangeHPEvent.AddListener(UpdateUI);
    }

    public void UpdateUI(float maxHP, float currentHP)
    {
        if (_heroTopHpBarSlider != null)
        {
            _heroTopHpBarSlider.value = currentHP / maxHP;
        }
    }
}


using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerScreenHPBarUI : MonoBehaviour
{
    //[SerializeField] private Slider _heroTopHpBarSlider;
    [SerializeField] private Slider _redTeamHPBar;
    [SerializeField] private Slider _blueTeamHPBar;

    public void RedTeamInit(UnityEvent<float, float> onChangeHPEvent)
    {
        onChangeHPEvent.AddListener(RedTeamUpdateUI);
    }

    public void BlueTeamInit(UnityEvent<float, float> onChangeHPEvent)
    {
        onChangeHPEvent.AddListener(BlueTeamUpdateUI);
    }

    public void RedTeamUpdateUI(float maxHP, float currentHP)
    {
        if (_redTeamHPBar != null)
        {
            _redTeamHPBar.value = currentHP / maxHP;
        }
    }

    public void BlueTeamUpdateUI(float maxHP, float currentHP)
    {
        if (_blueTeamHPBar != null)
        {
            _blueTeamHPBar.value = currentHP / maxHP;
        }
    }
}


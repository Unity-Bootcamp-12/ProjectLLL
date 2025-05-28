using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitHPBarUI : MonoBehaviour
{
    [SerializeField] private Slider _hpBarSlider;
    private Color _redColor = new Color(161 / 255f, 46 / 255f, 46 / 255f, 1f);   // #A12E2E
    private Color _blueColor = new Color(85 / 255f, 89 / 255f, 159 / 255f, 1f);  // #55599F

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

    public void SetTeam(UnitTeamType teamType)
    {
        bool isRed = teamType == UnitTeamType.RedTeam;
        _hpBarSlider.fillRect.GetComponent<Image>().color = isRed ? _redColor : _blueColor;
    }
}

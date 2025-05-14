using UnityEngine;


[CreateAssetMenu(fileName = "HeroSO", menuName = "Scriptable Object/HeroSO")]
public class HeroScriptableObject : ScriptableObject
{
    [SerializeField] public UnitStatus HeroStatus { get; private set; }

    [SerializeField] public int _id { get; private set; }
    [SerializeField] public string _name { get; private set; }

    [SerializeField]
    private int[] _amountOfExperience = { 0, 100, 200, 400, 600, 800, 1000, 1200, 1400 };
    public int[] GetAmountOfExperience() => _amountOfExperience;
}

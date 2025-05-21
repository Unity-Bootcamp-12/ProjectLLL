using UnityEngine;


[CreateAssetMenu(fileName = "HeroSO", menuName = "Scriptable Object/HeroSO")]
public class HeroScriptableObject : ScriptableObject
{
    public UnitStatus HeroStatus => _heroStatus;
    [SerializeField] private UnitStatus _heroStatus;

    public int Id => _id;
    [SerializeField] private int _id;

    public string Name => _name;
    [SerializeField] private string _name;

    public int[] GetAmountOfExperience() => _amountOfExperience;
    [SerializeField] private int[] _amountOfExperience = { 0, 100, 200, 400, 600, 800, 1000, 1200, 1400 };
}

using UnityEngine;


[CreateAssetMenu(fileName = "HeroSO", menuName = "Scriptable Object/HeroSO")]
public class HeroScriptableObject : ScriptableObject
{
    [SerializeField] public UnitStatus HeroStatus { get; private set; }

    [SerializeField] private int _id;
    [SerializeField] private string _name;

    [SerializeField]
    private int[] AmountOfExperience = { 0, 100, 200, 400, 600, 800, 1000, 1200, 1400 };
}

using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "HeroSO", menuName = "Scriptable Object/HeroSO")]
public class HeroScriptableObject : ScriptableObject
{
    public UnitStatus HeroStatus => _heroStatus;
    [SerializeField] private UnitStatus _heroStatus;

    public int Id => _id;
    [SerializeField] private int _id;

    public string Name => _name;
    [SerializeField] private string _name;

    public AttackType AttackType => _attackType;
    [SerializeField] private AttackType _attackType;

    public GameObject ProjectilePrefab => _projectilePrefab;
    [SerializeField] private GameObject _projectilePrefab;

    public Sprite HeroImage => _heroImage;
    [SerializeField] private Sprite _heroImage;
}

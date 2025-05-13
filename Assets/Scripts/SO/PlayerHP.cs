using UnityEngine;


[CreateAssetMenu(fileName = "InfoOfplayer", menuName = "ScriptableObj/GameItem", order = 0)]
public class PlayerHP : ScriptableObject
{
    [Header("각각 플레이어들")]
    public int[] _maxHp =
    { 100, 200, 300, 400, 500, 600, 700, 800, 900 };
    public int _currentHp = 100;


}

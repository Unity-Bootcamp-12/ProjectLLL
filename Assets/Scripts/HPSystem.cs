using UnityEngine;

public class HPSystem : MonoBehaviour
{
    //private int[] _maxHp =
    //{ 100, 200, 300, 400, 500, 600, 700, 800, 900 };
    //private int _currentHp = 100;

    public PlayerHP playerHP;

    public void SetHp(int level)
    {
        playerHP._currentHp = playerHP._maxHp[level];
    }

    public int SetCurrentHp(int value)
    {
        return playerHP._currentHp += value;
    }

    public int GetCurrentHP()
    {
        return playerHP._currentHp;
    }

}

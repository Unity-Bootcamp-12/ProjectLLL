using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerStatusController : MonoBehaviour
{
    [SerializeField] private int _playerLevel = 1;

    [SerializeField] private float _playerAttackPower = 10;
    //private bool _isAttacked;
    [SerializeField] private float _playerAttackSpeed = 1.0f;
    [SerializeField] private float _playerAttackRange = 125;
    [SerializeField] private float _playerMoveSpeed = 3.0f;
    
    private int ExperiencePoint = 0;
    private int SkillPoint = 1;
    private float RecoveryTime = 10.0f;
    private int[] LevelUp = {0, 100, 300, 500, 700, 900, 1100, 1300, 1500}; //LevelUp의 경험치를 배열로 나누고 각 요소의 성과를 가지고 LevelUP을 한다.

    /// <summary>
    /// 플레이어 레벨업 메소드
    /// </summary>
    public void PlayerLevelUp()
    {
        if (ExperiencePoint == LevelUp[_playerLevel])
        {
            //스탯 증가(레벨업) 메소드 호출;

            _playerAttackPower += 10;
            _playerLevel++;
        }
    }

    



}

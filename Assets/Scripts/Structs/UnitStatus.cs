using System;
using UnityEngine;

[Serializable]
public struct UnitStatus
{
    // 초기 세팅 값
    public float MaxHP => _maxHP;
    [SerializeField] private float _maxHP;

    public float AttackPower => _attackPower;
    [SerializeField] private float _attackPower;

    public float AttackSpeed => _attackSpeed;
    [SerializeField] private float _attackSpeed;

    public float AttackRange => _attackRange;
    [SerializeField] private float _attackRange;

    public float MoveSpeed => _moveSpeed;
    [SerializeField] private float _moveSpeed;

    public static UnitStatus operator +(UnitStatus a, UnitStatus b)
    {
        return new UnitStatus
        {
            _maxHP = a._maxHP + b._maxHP,
            _attackPower = a._attackPower + b._attackPower,
            _attackSpeed = a._attackSpeed + b._attackSpeed,
            _attackRange = a._attackRange + b._attackRange,
            _moveSpeed = a._moveSpeed + b._moveSpeed,
        };
    }
}

public enum AttackType
{
    Melee,   // 근거리
    Ranged   // 원거리
}
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

    public AttackType AttackType => _attackType;
    [SerializeField] private AttackType _attackType;

    public GameObject ProjectilePrefab => _projectilePrefab;
    [SerializeField] private GameObject _projectilePrefab;
}

public enum AttackType
{
    Melee,   // 근거리
    Ranged   // 원거리
}
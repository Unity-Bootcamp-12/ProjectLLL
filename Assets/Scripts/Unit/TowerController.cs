using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TowerController : UnitController
{
    private UnitController _target;
    private Coroutine _attackCoroutine = null;

    private bool _isAttacking = false;
    private bool _isPreAttacking = false;
    private bool _isPostAttacking = false;

    private List<UnitController> _unitInRangeList = new List<UnitController>();

    public override void Dead()
    {
        StopAttack();
        throw new System.NotImplementedException();
    }

    public override void ReceiveDamage(float damage)
    {
        _hpController.ChangeHPRpc(-damage);
    }

    public override void OnTriggerEnterFromChild(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Unit"))
        {
            if (other.TryGetComponent<UnitController>(out var unit))
            {
                if (unit.TeamType != TeamType)
                {
                    if (_target == null)
                    {
                        _target = unit;
                    }
                    _unitInRangeList.Add(unit);
                    Logger.Info($"Target Found: {unit.name}");
                }
            }
        }
    }

    public override void OnTriggerExitFromChild(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Unit"))
        {
            if (other.TryGetComponent<UnitController>(out var unit))
            { 
                if (_unitInRangeList.Contains(unit))
                {
                    _unitInRangeList.Remove(unit);

                    if (_target == unit)
                    {
                        _target = null;
                        StopAttack();
                        // 임시로 0번이 다음 타겟이 되도록 설정
                        if (_unitInRangeList.Count > 0)
                        {
                            _target = _unitInRangeList[0];
                        }
                    }
                    Logger.Info($"Target Out: {unit.name}");
                }
            }
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (_target == null)
        {
            return;
        }

        if (_target.IsDead)
        {
            _target = null;
            return;
        }


        float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
        if (TeamType != _target.TeamType)
        {
            if (_isAttacking)
            {
                return;
            }
            else
            {
                if (distanceToTarget < GetAttackRange())
                {
                    _attackCoroutine = StartCoroutine(AttackCoroutine(1.0f, 1.0f));
                }
            }
        }
    }

    private IEnumerator AttackCoroutine(float preAttackDelayTime, float postAttackDelayTime)
    {
        _isAttacking = true;
        _isPreAttacking = true;

        UnitController attackTarget = _target;
        Logger.Info("Attack Start");

        yield return new WaitForSeconds(preAttackDelayTime);

        Logger.Info("Attack");
        Attack(attackTarget);

        _isPostAttacking = false;
        yield return new WaitForSeconds(postAttackDelayTime);

        _isAttacking = false;
        _isPreAttacking = false;
        _isPostAttacking = false;
        Logger.Info("Attack End");
    }

    public void Attack(UnitController unitController)
    {
        if (GetAttackType() == AttackType.Melee)
        {
            Logger.Info($"Melee Attack: {unitController.name}");
            unitController.ReceiveDamage(GetAttackPower());
        }
        else if (GetAttackType() == AttackType.Ranged)
        {
            Logger.Info($"Ranged Attack: {unitController.name}");
            FireTargetProjectileRpc(unitController.NetworkObjectId, 2.0f, GetAttackPower());
        }
    }

    public void StopAttack()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
        }
        _isAttacking = false;
        _isPreAttacking = false;
        _isPostAttacking = false;
    }

    [Rpc(SendTo.Server)]
    public void FireTargetProjectileRpc(ulong targetId, float speed, float damage)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetId, out var targetNetworkObject))
        {
            return;
        }

        GameObject projectileObject = Instantiate(GetProjectilePrefab(), transform.position, Quaternion.identity);
        projectileObject.GetComponent<TargetProjectile>().Init(targetNetworkObject.transform, speed, damage);
    }
}

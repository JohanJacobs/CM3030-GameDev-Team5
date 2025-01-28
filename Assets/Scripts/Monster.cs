using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : Creature
{
    public float ApproachDistance = 0.3f;

    public float Speed => AbilitySystemComponent.GetAttributeValue(AttributeType.MoveSpeed);
    public float TurnSpeed => AbilitySystemComponent.GetAttributeValue(AttributeType.TurnSpeed);
    public float KnockBackResistance => AbilitySystemComponent.GetAttributeValue(AttributeType.KnockBackResistance);
    public float DamageMin => AbilitySystemComponent.GetAttributeValue(AttributeType.MinDamage);
    public float DamageMax => AbilitySystemComponent.GetAttributeValue(AttributeType.MaxDamage);
    public float AttackRate => AbilitySystemComponent.GetAttributeValue(AttributeType.AttackRate);
    public float AttackRange => AbilitySystemComponent.GetAttributeValue(AttributeType.AttackRange);

    private GameObject _target;

    private float _toNextAttack = 0f;

    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (IsDead)
            return;

        if (_target == null)
            return;

        UpdateMovement();
        UpdateAttack();
        UpdateAttackCooldown();
    }

    protected override void OnDeath()
    {
        _target = null;
    }

    protected override void OnDamageTaken(GameObject causer, Vector3 origin, float amount)
    {
        KnockBack(origin, amount / MaxHealth);
    }

    private void KnockBack(Vector3 origin, float amount)
    {
        if (!(KnockBackResistance < 1))
            return;

        var direction = transform.position - origin;

        transform.Translate(direction.normalized * amount * (1f - KnockBackResistance), Space.World);
    }

    private void UpdateMovement()
    {
        var myPosition = transform.position;
        var targetPosition = _target.transform.position;

        var targetDelta = targetPosition - myPosition;

        targetDelta.y = 0;

        var targetRotation = Quaternion.LookRotation(targetDelta.normalized, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, TurnSpeed * Time.deltaTime);

        var motionDistance = Speed * Time.deltaTime;

        motionDistance = Mathf.Min(motionDistance, Mathf.Max(targetDelta.magnitude - ApproachDistance, 0));

        transform.Translate(transform.forward * motionDistance, Space.World);
    }

    private void UpdateAttack()
    {
        if (_toNextAttack > 0)
            return;

        if (!IsTargetInAttackRange())
            return;

        _toNextAttack += 1f / AttackRate;

        var targetCreature = _target.GetComponent<Creature>();

        targetCreature.TakeDamage(gameObject, transform.position, Random.Range(DamageMin, DamageMax));
    }

    private bool IsTargetInAttackRange()
    {
        var myPosition = transform.position;
        var targetPosition = _target.transform.position;

        var targetDelta = targetPosition - myPosition;

        targetDelta.y = 0;

        // TODO: magnitude then normalized - could be optimized

        if (targetDelta.magnitude > AttackRange)
            return false;

        // 30 degrees attack "cone"
        if (Vector3.Dot(targetDelta.normalized, transform.forward) < Mathf.Cos(Mathf.Deg2Rad * 15f))
            return false;

        return true;
    }

    private void UpdateAttackCooldown()
    {
        if (_toNextAttack > Time.deltaTime)
        {
            _toNextAttack -= Time.deltaTime;
        }
        else
        {
            _toNextAttack = 0;
        }
    }
}

using UnityEngine;
using UnityEngine.AI;

public class Monster : Creature
{
    public float ApproachDistance = 0.3f;

    public Animator animator;

    public float Speed => AbilitySystemComponent.GetAttributeValue(AttributeType.MoveSpeed);
    public float TurnSpeed => AbilitySystemComponent.GetAttributeValue(AttributeType.TurnSpeed);
    public float KnockBackResistance => AbilitySystemComponent.GetAttributeValue(AttributeType.KnockBackResistance);
    public float DamageMin => AbilitySystemComponent.GetAttributeValue(AttributeType.MinDamage);
    public float DamageMax => AbilitySystemComponent.GetAttributeValue(AttributeType.MaxDamage);
    public float AttackRate => AbilitySystemComponent.GetAttributeValue(AttributeType.AttackRate);
    public float AttackRange => AbilitySystemComponent.GetAttributeValue(AttributeType.AttackRange);

    private NavMeshAgent _navMeshAgent;

    private GameObject _target;

    private float _toNextAttack = 0f;

    private Vector3 _knockBackForce = Vector3.zero;

    public void KnockBack(Vector3 origin, float amount)
    {
        if (!(KnockBackResistance < 1))
            return;

        var direction = transform.position - origin;

        _knockBackForce += direction.normalized * amount * (1f - KnockBackResistance);
    }

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        var deltaTime = Time.deltaTime;

        {
            UpdateKnockBackForce(deltaTime);
        }

        {
            if (IsDead)
                return;

            if (_target == null)
                return;

            UpdateNavMeshMovement();
            UpdateMovementAnimation();
            UpdateAttack();
            UpdateAttackCooldown(deltaTime);
        }
    }

    protected override void OnDeath()
    {
        PlayDeathAnimation();

        _target = null;

        _navMeshAgent.isStopped = true;
        _navMeshAgent.enabled = false;

        var myCollider = GetComponentInChildren<Collider>();
        if (myCollider)
        {
            myCollider.enabled = false;
        }
    }

    protected override void OnDamageTaken(GameObject causer, Vector3 origin, float amount)
    {
        PlayHitAnimation();

        if(GameData.Instance.hitParticleEffect != null)
        {
            GameObject particleEffectInstance = Instantiate(GameData.Instance.hitParticleEffect, transform.position, Quaternion.identity);
            Destroy(particleEffectInstance, 2f);
        }

        // TODO: shouldn't be here
        // var knockBackMagnitude = AbilitySystemUtility.GetAttributeValueOrDefault(causer, AttributeType.KnockBack, 1);
        //
        // KnockBack(origin, knockBackMagnitude * amount / MaxHealth);
    }

    private void UpdateMovementAnimation()
    {
        var walkSpeedMultiplier = Mathf.Max(Speed / WalkAnimationMoveSpeed, 0);

        animator.SetFloat("WalkSpeedMultiplier", walkSpeedMultiplier);
    }

    private void UpdateNavMeshMovement()
    {
        _navMeshAgent.speed = Speed;
        _navMeshAgent.angularSpeed = TurnSpeed;
        _navMeshAgent.stoppingDistance = ApproachDistance;

        _navMeshAgent.SetDestination(_target.transform.position);
    }

    private void UpdateAttack()
    {
        if (_toNextAttack > 0)
            return;

        if (!IsTargetInAttackRange())
            return;

        PlayAttackAnimation();

        _toNextAttack += 1f / AttackRate;

        var targetCreature = _target.GetComponent<Creature>();

        DealDamage(targetCreature, transform.position, Random.Range(DamageMin, DamageMax));
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

    private void UpdateAttackCooldown(float deltaTime)
    {
        if (_toNextAttack > deltaTime)
        {
            _toNextAttack -= deltaTime;
        }
        else
        {
            _toNextAttack = 0;
        }
    }

    private void UpdateKnockBackForce(float deltaTime)
    {
        var motion = _knockBackForce * deltaTime;

        motion.y = 0;

        if (motion.sqrMagnitude > 0.0001f)
        {
            // TODO: ???
            // _navMeshAgent.Move(motion);

            transform.Translate(motion, Space.World);
        }

        // sort of "decay to 0 in 1 second"
        if (_knockBackForce.sqrMagnitude > 0.0001f)
            _knockBackForce = Vector3.Lerp(_knockBackForce, Vector3.zero, deltaTime);
        else
            _knockBackForce = Vector3.zero;
    }

    private void PlayHitAnimation()
    {
        animator.SetTrigger("IsHit");
    }

    private void PlayDeathAnimation()
    {
        animator.SetBool("IsDead", true);
    }

    private void PlayAttackAnimation()
    {
        animator.SetTrigger("IsAttacking");
    }
}

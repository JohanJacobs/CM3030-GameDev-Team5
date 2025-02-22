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

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (IsDead)
            return;

        if (_target == null)
            return;

        UpdateNavMeshMovement();
        // UpdateMovement();
        UpdateAttack();
        UpdateAttackCooldown();
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
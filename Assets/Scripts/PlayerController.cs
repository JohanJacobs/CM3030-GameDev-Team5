using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator Animator;

    public Vector3 MuzzleOffset = new Vector3(0, 0.1f, 0);

    public LayerMask MonsterLayerMask;

    public GameObject BulletTracerFX;
    public GameObject HUD;

    private CharacterController _characterController;
    private Player _player;
    private HUD _hud;

    private float _toNextShot = 0;

    private Vector3? _lookAtPointOnGround = null;

    private int _kills = 0;

    void Awake()
    {
        CreateHUD();
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _player = GetComponent<Player>();

        _player.Kill += (creature, victim) => AddKill();
        _player.Death += (creature) => ShowWasted();

        ResetHUD();
    }

    void Update()
    {
        if (_player.IsDead)
            return;

        UpdateAim();
        UpdateMovement();
        UpdateShooting();
        UpdateShootingCooldown();
    }

    void LateUpdate()
    {
        _hud.UpdateHealth(_player.Health, _player.MaxHealth);
    }

    private void UpdateMovement()
    {
        Vector3 movementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // GetPlayerMovementBasis(out var movementForward, out var movementRight);
        GetCameraMovementBasis(out var movementForward, out var movementRight);

        var movementZ = movementForward * movementInput.z;
        var movementX = movementRight * movementInput.x;

        var combinedMovement = movementZ + movementX;

        if (combinedMovement.magnitude > 1)
        {
            combinedMovement.Normalize();
        }

        _characterController.Move(combinedMovement * _player.Speed * Time.deltaTime);

        // HACK: keep player at Y=0
        transform.Translate(0, -transform.position.y, 0);

        Animator.SetFloat("MovementInput.Forward", movementInput.z);
        Animator.SetFloat("MovementInput.Right", movementInput.x);
    }

    private void GetPlayerMovementBasis(out Vector3 forward, out Vector3 right)
    {
        forward = transform.forward;
        right = transform.right;
    }

    private void GetCameraMovementBasis(out Vector3 forward, out Vector3 right)
    {
        var cameraToPlayer = transform.position - Camera.main.transform.position;

        cameraToPlayer.y = 0;
        cameraToPlayer.Normalize();

        forward = cameraToPlayer;
        right = Vector3.Cross(Vector3.up, cameraToPlayer);
    }

    private void UpdateAim()
    {
        // NOTE: hardcoded ground height
        var plane = new Plane(Vector3.up, 0);

        var mouseAimRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(mouseAimRay, out var rayEnter))
        {
            var lookAtPointOnGround = mouseAimRay.GetPoint(rayEnter);

            if (Vector3.Distance(transform.position, lookAtPointOnGround) > 0.1)
            {
                transform.LookAt(lookAtPointOnGround);

                _lookAtPointOnGround = lookAtPointOnGround;
            }
        }
        else
        {
            _lookAtPointOnGround = null;
        }
    }

    private void UpdateShooting()
    {
        if (_toNextShot > 0)
            return;
        if (!_lookAtPointOnGround.HasValue)
            return;

        bool fireButtonPressed = Input.GetButton("Fire1");

        if (!fireButtonPressed)
            return;

        _toNextShot += 1f / _player.FireRate;

        var aimDirection = _lookAtPointOnGround.Value - transform.position;
        var ray = new Ray(transform.position + MuzzleOffset, aimDirection);

        RandomSpawnBulletTracerFX(ray.origin, ray.direction, 1f);

        if (Physics.Raycast(ray, out var hit, _player.FireRange, MonsterLayerMask))
        {
            var monster = hit.collider.GetComponentInParent<Monster>();

            _player.DealDamage(monster, transform.position, Random.Range(_player.DamageMin, _player.DamageMax));
        }
    }

    private void UpdateShootingCooldown()
    {
        if (_toNextShot > Time.deltaTime)
        {
            _toNextShot -= Time.deltaTime;
        }
        else
        {
            _toNextShot = 0;
        }
    }

    private void RandomSpawnBulletTracerFX(Vector3 origin, Vector3 direction, float probability = 0.6f)
    {
        if (Random.value > probability)
            return;

        var rotation = Quaternion.LookRotation(direction);

        var fxGameObject = GameObject.Instantiate(BulletTracerFX, origin, rotation);

        GameObject.Destroy(fxGameObject, 0.2f);
    }

    private void CreateHUD()
    {
        var hudGameObject = GameObject.Instantiate(HUD);

        _hud = hudGameObject.GetComponent<HUD>();
    }

    private void ResetHUD()
    {
        _hud.UpdateHealth(_player.Health, _player.MaxHealth);
        _hud.UpdateKillCounter(_kills);
    }

    private void AddKill()
    {
        ++_kills;

        _hud.UpdateKillCounter(_kills);
    }

    private void ShowWasted()
    {
        _hud.ShowWasted();
    }
}

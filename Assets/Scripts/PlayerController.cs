using UnityEngine;

public class PlayerController : MonoBehaviour
{
    AudioManager audioManager;

    public Animator Animator;

    public Vector3 MuzzleOffset = new Vector3(0, 0.25f, 0);

    public LayerMask MonsterLayerMask;

    public GameObject BulletTracerFX;
    public GameObject HUD;
    public GameObject GameMenu;


    private CharacterController _characterController;
    private Player _player;
    private HUD _hud;
    private GameMenu _gameMenu;

    private AbilitySystemComponent _asc;

    private float _toNextShot = 0;

    private Vector3? _lookAtPointOnGround = null;

    private int _kills = 0;


    void Awake()
    {
        CreateUI();

        // Set audioManager to external audioManager object with tag Audio
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _player = GetComponent<Player>();

        _asc = GetComponent<AbilitySystemComponent>();
        _asc.OnReady(OnAbilitySystemReady);

        _player.Kill += HandlePlayerKill;
        _player.Death += HandlePlayerDeath;
        _player.DamageTaken += HandlePlayerDamageTaken;
    }

    private void OnAbilitySystemReady(AbilitySystemComponent asc)
    {
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
        UpdateHUD();
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

        var gravity = Vector3.down * 10f;

        _characterController.Move((combinedMovement * _player.Speed + gravity) * Time.deltaTime);

        // convert movement to local space for animation
        var localMovementZ = Vector3.Dot(transform.forward, combinedMovement);
        var localMovementX = Vector3.Dot(transform.right, combinedMovement);

        PlayMoveAnimation(new Vector3(localMovementX, 0, localMovementZ));
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
            var lookAtPointOnGroundDelta = lookAtPointOnGround - transform.position;

            lookAtPointOnGroundDelta.y = 0;

            if (lookAtPointOnGroundDelta.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(lookAtPointOnGroundDelta, Vector3.up);

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
        {
            PlayShootAnimation(false);
            return;
        }

        // Play shooting sound
        audioManager.PlaySFX(audioManager.sfxexample);
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

        var fxGameObject = Instantiate(BulletTracerFX, origin, rotation);

        Destroy(fxGameObject, 0.2f);
    }

    private void AddKill()
    {
        ++_kills;

        _hud.UpdateKillCounter(_kills);

        // Play dead skeleton sound
        audioManager.PlaySFX(audioManager.killedSkeletonSound);
    }

    private void ShowWasted()
    {
        _hud.ShowGameOver();
    }

    private void PlayHitAnimation()
    {
        Animator.SetTrigger("IsHit");
    }

    private void PlayDeathAnimation()
    {
        Animator.SetBool("IsDead", true);
    }

    private void PlayShootAnimation(bool isShooting)
    {
        Animator.SetBool("IsShooting", isShooting);
    }

    private void PlayMoveAnimation(Vector3 movementInput)
    {
        Animator.SetFloat("ForwardMovement", movementInput.z);
        Animator.SetFloat("RightMovement", movementInput.x);
    }

    private void HandlePlayerKill(Creature creature, Creature victim)
    {
        AddKill();
    }

    private void HandlePlayerDeath(Creature creature)
    {
        PlayDeathAnimation();

        ShowWasted();
    }

    private void HandlePlayerDamageTaken()
    {
        PlayHitAnimation();
    }

    #region UI
    private void CreateUI()
    {
        CreateHUD();
        CreateGameMenu(_hud);
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
        _hud.UpdateExperience(_player.Experience);
        _hud.UpdateLevel(_player.Level);
    }

    private void UpdateHUD()
    {
        _hud.UpdateHealth(_player.Health, _player.MaxHealth);
        _hud.UpdateExperience(_player.Experience);
        _hud.UpdateLevel(_player.Level);
    }

    private void CreateGameMenu(HUD hud)
    {
        var gameMenuGameObject = GameObject.Instantiate(GameMenu);
        _gameMenu = gameMenuGameObject.GetComponent<GameMenu>();

        // callback to hide the hud when the menu is displayed
        _gameMenu.SetHudVisibilityToggleCallback((bool isVisible) => { _hud.gameObject.SetActive(isVisible); });
    }
    #endregion UI

}

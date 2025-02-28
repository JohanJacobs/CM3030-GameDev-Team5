/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

PlayerController.cs

Class PlayerController is used to manage attributes and player actions such as movement, aiming, firing, being hit, dying and others


*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController), typeof(Player))]
public class PlayerController : MonoBehaviour
{
    // cache reference for easier access
    private static AudioManager audioManager => AudioManager.Instance;

    public Animator Animator;

    public Vector3 MuzzleOffset = new Vector3(0, 0.25f, 0);

    public GameObject BulletTracerFX;
    public GameObject HUD;
    public GameObject GameMenu;

    public InputMappingContext DefaultInputMappingContext;

    private CharacterController _characterController;
    private Player _player;
    private HUD _hud;
    private GameMenu _gameMenu;

    private AbilitySystemComponent _asc;
    private InputComponent _inputComponent;
    private EquipmentComponent _equipmentComponent;

    private int _kills = 0;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        _player = GetComponent<Player>();

        _asc = GetComponent<AbilitySystemComponent>();
        _asc.OnReady(OnAbilitySystemReady, 5);

        _inputComponent = GetComponent<InputComponent>();
        _inputComponent.InputActionPressed += OnInputActionPressed;
        _inputComponent.InputActionReleased += OnInputActionReleased;

        _equipmentComponent = GetComponent<EquipmentComponent>();
        _equipmentComponent.ItemEquipped += OnItemEquipped;
        _equipmentComponent.ItemUnequipped += OnItemUnequipped;

        CreateUI();
    }

    void Start()
    {
        if (DefaultInputMappingContext)
        {
            _inputComponent.AddInputMappingContext(DefaultInputMappingContext);
        }

        _player.Kill += HandlePlayerKill;
        _player.Death += HandlePlayerDeath;
        _player.DamageTaken += HandlePlayerDamageTaken;
        _player.CommittedAttack += HandlePlayerCommittedAttack;
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

        if (!plane.Raycast(mouseAimRay, out var rayEnter))
            return;

        var lookAtPointOnGround = mouseAimRay.GetPoint(rayEnter);
        var lookAtPointOnGroundDelta = lookAtPointOnGround - transform.position;

        lookAtPointOnGroundDelta.y = 0;

        // less than 0.1 units away from player - ignore
        if (lookAtPointOnGroundDelta.sqrMagnitude < 0.01f)
            return;

        transform.rotation = Quaternion.LookRotation(lookAtPointOnGroundDelta, Vector3.up);

        var aimOrigin = transform.position + MuzzleOffset;
        var aimTarget = lookAtPointOnGround;
        var aimDirection = lookAtPointOnGround - aimOrigin;

        aimDirection.y = 0f;
        aimDirection.Normalize();

        _player.UpdateAttackAbilityAim(aimOrigin, aimTarget, aimDirection);
    }

    public void RandomSpawnBulletTracerFX(Vector3 origin, Vector3 direction, float probability = 0.6f)
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
        _hud.ShowGameOver(HighScoreManager.Instance.GetHighestScore(), _kills);
    }

    private void PlayHitAnimation()
    {
        Animator.SetTrigger("IsHit");

        // Play player hit sound
        audioManager.PlaySFX(audioManager.playerHitSound);
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

        var walkSpeedMultiplier = Mathf.Max(_player.Speed / _player.WalkAnimationMoveSpeed, 0);

        Animator.SetFloat("WalkSpeedMultiplier", walkSpeedMultiplier);
    }

    private void HandlePlayerKill(Creature creature, Creature victim)
    {
        AddKill();
    }

    private void HandlePlayerDeath(Creature creature)
    {
        audioManager.PlaySFX(audioManager.playerDeadSound);
        PlayDeathAnimation();
        ShowWasted();
        UpdatePlayerScore();
    }

    private void HandlePlayerDamageTaken()
    {
        PlayHitAnimation();
    }

    private void HandlePlayerCommittedAttack(AbilityInstance abilityInstance, Vector3 origin, Vector3 direction, float damage, EquipmentSlot abilityEquipmentSlot)
    {
        audioManager.PlaySFX(audioManager.sfxexample);

        TryGetEquipmentItemMuzzleTransform(abilityEquipmentSlot, out var muzzleOrigin);

        // TODO: use muzzle direction as well?
        RandomSpawnBulletTracerFX(muzzleOrigin, direction, 1f);

        // TODO: change IsShooting to trigger?
        StartCoroutine(PlayShootAnimationForSeconds(.2f));
    }

    private IEnumerator PlayShootAnimationForSeconds(float seconds)
    {
        PlayShootAnimation(true);

        yield return new WaitForSeconds(seconds);

        PlayShootAnimation(false);
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

    private void UpdatePlayerScore()
    {
        HighScoreManager.Instance.AddNewScore(_kills);
    }

    private void OnInputActionPressed(InputAction action)
    {
        _asc.OnInputActionPressed(action);
    }

    private void OnInputActionReleased(InputAction action)
    {
        _asc.OnInputActionReleased(action);
    }

    private void OnItemEquipped(Item item, EquipmentItem equipmentItem, EquipmentSlot equipmentSlot)
    {
        switch (item)
        {
            case WeaponItem weaponItem:
                OnWeaponEquipped(weaponItem, equipmentItem, equipmentSlot);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(item));
        }
    }

    private void OnItemUnequipped(Item item, EquipmentItem equipmentItem, EquipmentSlot equipmentSlot)
    {
        switch (item)
        {
            case WeaponItem weaponItem:
                OnWeaponUnequipped(weaponItem, equipmentItem, equipmentSlot);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(item));
        }
    }

    private void OnWeaponEquipped(WeaponItem item, EquipmentItem equipmentItem, EquipmentSlot equipmentSlot)
    {
        var abilityHandle = _asc.AddAbility(item.AttackAbility);
        if (abilityHandle == null)
            throw new Exception("Failed to add weapon attack ability");

        var abilityInstance = abilityHandle.AbilityInstance;

        switch (equipmentSlot)
        {
            case EquipmentSlot.MainHand:
                abilityInstance.InputTag = GameData.Instance.InputData.MainHandAbilityInputTag;
                break;
            case EquipmentSlot.OffHand:
                abilityInstance.InputTag = GameData.Instance.InputData.OffHandAbilityInputTag;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(equipmentSlot), equipmentSlot, null);
        }

        var attackAbilityInstanceData = abilityInstance.GetData<AttackAbilityInstanceData>();

        attackAbilityInstanceData.EquipmentItem = equipmentItem;
        attackAbilityInstanceData.EquipmentSlot = equipmentSlot;
    }

    private void OnWeaponUnequipped(WeaponItem item, EquipmentItem equipmentItem, EquipmentSlot equipmentSlot)
    {
        _asc.RemoveAbility(item.AttackAbility);
    }

    private bool TryGetEquipmentItemMuzzleTransform(EquipmentSlot equipmentSlot, out Vector3 origin)
    {
        var equippedItem = _equipmentComponent.GetEquippedItem(equipmentSlot);

        if (equippedItem is WeaponEquipmentItem weapon)
        {
            origin = weapon.MuzzleTransform.position;
            return true;
        }

        if (_player.TryFindEquipmentAttachmentSlot(equipmentSlot, out var attachmentSlot))
        {
            origin = attachmentSlot.Socket.position;
            return true;
        }

        origin = Vector3.zero;
        return false;
    }

    private bool TryGetEquipmentItemMuzzleTransform(EquipmentSlot equipmentSlot, out Vector3 origin, out Vector3 direction)
    {
        var equippedItem = _equipmentComponent.GetEquippedItem(equipmentSlot);

        if (equippedItem is WeaponEquipmentItem weapon)
        {
            origin = weapon.MuzzleTransform.position;
            direction = weapon.MuzzleTransform.forward;
            return true;
        }

        if (_player.TryFindEquipmentAttachmentSlot(equipmentSlot, out var attachmentSlot))
        {
            origin = attachmentSlot.Socket.position;
            direction = attachmentSlot.Socket.forward;
            return true;
        }

        origin = Vector3.zero;
        direction = Vector3.forward;
        return false;
    }
}

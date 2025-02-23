using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField] Color defaultColor;
    [SerializeField] Color enemyColor;

    [SerializeField] LayerMask worldLayer;
    [SerializeField] LayerMask enemyLayer;

    [SerializeField] Transform visual;

    SpriteRenderer _spriteRender;
    Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        _spriteRender = GetComponentInChildren<SpriteRenderer>();
    }

    public void LateUpdate()
    {
        if (GetMouseWorldPosition(out var world_position, out var hitEnemy))
        {
            EnableCrosshair();
            SetCrosshairPosition(world_position);
            SetCrosshairColor(hitEnemy);
        }
        else
        {
            DisableCrosshair();
        }
    }

    // calculate the world position and determine if we hit an enemy
    bool GetMouseWorldPosition(out Vector3 worldPosition, out bool hitEnemy)
    {
        worldPosition = Vector3.zero;
        hitEnemy = false;

        // keep track if we actually hit shows how the world point
        bool foundWorldPoint = false;

        // find the location of the mouse in the world by raycasting into the world 
        // through the camera
        var mouse_screen_position = Input.mousePosition; // mouse screen coordinate
        var ray = _camera.ScreenPointToRay(mouse_screen_position);
        var hitAll = Physics.RaycastAll(ray, 50f, worldLayer | enemyLayer);

        // hit nothing.
        if (hitAll.Length == 0)
            return false;


        // check all the gameObjects we hit and update the state 
        // based on the layer the object is in.
        foreach (var target in hitAll)
        {
            if (enemyLayer.TestGameObjectLayer(target.collider.gameObject))
            {
                hitEnemy = true;
            }
            else if (worldLayer.TestGameObjectLayer(target.collider.gameObject))
            {
                // save the point that we collide with in the world
                worldPosition = target.point;
                worldPosition.y = worldPosition.y > 0.5f ? 0.5f : worldPosition.y;
                foundWorldPoint = true;
            }
        }

        return foundWorldPoint;
    }

    private void SetCrosshairPosition(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }

    // hides the cross hair visual sprite
    private void DisableCrosshair()
    {
        // already disabled so do nothing
        if (!visual.gameObject.activeSelf)
            return;

        visual.gameObject.SetActive(false);
    }

    // Shows the cross hair visual sprite
    private void EnableCrosshair()
    {
        // already Enabled so do nothing
        if (visual.gameObject.activeSelf)
            return;

        visual.gameObject.SetActive(true);
    }

    // Set the color of the crosshair 
    // based on if we hit an enemy or not.
    private void SetCrosshairColor(bool hitEnemy)
    {
        _spriteRender.color = hitEnemy ? enemyColor : defaultColor;
    }
}
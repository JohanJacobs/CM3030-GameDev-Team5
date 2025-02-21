using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField] LayerMask worldLayer;
    [SerializeField] Transform visual;

    Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }
    public void LateUpdate()
    {        
        if (GetMouseWorldPosition(out var world_position))
        {
           EnableCrosshair();
           SetCrosshairPosition(world_position);
        }
        else
        {
            DisableCrosshair();
        }
    }

    // calculate the world position of the mouse 
    bool GetMouseWorldPosition(out Vector3 worldPosition)
    {
        worldPosition = Vector3.zero;

        // get the screen coordinate that the mouse is at 
        var mouse_screen_position = Input.mousePosition;
        
        // do a raycast and find where it collides with the world
        var ray = camera.ScreenPointToRay(mouse_screen_position);

        if (Physics.Raycast(ray, out var hitInfo, 50f, worldLayer))
        {
            // hit an object, clamp the position to 0 y axis and return 
            worldPosition = hitInfo.point;
            // clamp the world position Y axis
            worldPosition.y = worldPosition.y > 0.5f ? 0.5f : worldPosition.y;
            return true;
        }

        // did not hit anything so the aim might be outside the window
        return false;
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
}

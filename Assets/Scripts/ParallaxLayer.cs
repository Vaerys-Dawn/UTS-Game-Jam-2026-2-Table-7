using UnityEngine;

// Basic parallax: put this on a layer root (e.g. "Layer01(BG)" or "Layer03(FG)").
// The layer is shifted by a fraction of how far the camera has moved since the scene started.
//   factor  0    = moves like the terrain (no parallax)
//   factor  0..1 = background, moves slower than the terrain (feels far away). 1 = glued to the camera
//   factor  < 0  = foreground, moves faster than the terrain (feels close to the camera)
public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("X/Y parallax strength. Background: 0..1 (e.g. 0.5). Foreground: negative (e.g. -0.3). Set Y to 0 to disable vertical parallax.")]
    [SerializeField]
    private Vector2 parallaxFactor = new Vector2(0.5f, 0.5f);

    [Tooltip("Camera to follow. Leave empty to use the camera tagged MainCamera.")]
    [SerializeField]
    private Transform cameraTransform;

    private Vector3 layerStartPos;
    private Vector3 cameraStartPos;

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        if (cameraTransform == null)
        {
            Debug.LogWarning($"ParallaxLayer on {name}: no camera found, parallax disabled.");
            enabled = false;
            return;
        }

        layerStartPos = transform.position;
        cameraStartPos = cameraTransform.position;
    }

    // LateUpdate runs after the player (and its child camera) has moved this frame.
    void LateUpdate()
    {
        Vector3 cameraMoved = cameraTransform.position - cameraStartPos;
        transform.position = new Vector3(
            layerStartPos.x + cameraMoved.x * parallaxFactor.x,
            layerStartPos.y + cameraMoved.y * parallaxFactor.y,
            layerStartPos.z);
    }
}

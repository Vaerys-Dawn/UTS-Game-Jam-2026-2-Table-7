using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    [SerializeField]
    private bool loadOnPlayerTouch = true;

    [SerializeField]
    private bool loadOnClick = true;

    [SerializeField]
    private string playerTag = "Player";

    private Collider2D ownCollider;
    private bool isLoading = false;

    void Awake()
    {
        ownCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!loadOnClick || ownCollider == null || Mouse.current == null || Camera.main == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        if (ownCollider.OverlapPoint(mouseWorld))
        {
            LoadScene();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (loadOnPlayerTouch && IsPlayer(collision)) LoadScene();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (loadOnPlayerTouch && IsPlayer(collision.collider)) LoadScene();
    }

    private bool IsPlayer(Collider2D col)
    {
        if (col.CompareTag(playerTag)) return true;
        return col.attachedRigidbody != null && col.attachedRigidbody.CompareTag(playerTag);
    }

    public void LoadScene()
    {
        if (isLoading) return;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError($"SceneLoader on {name}: no scene name set.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"SceneLoader on {name}: scene '{sceneName}' is not in the Build Profiles scene list.");
            return;
        }

        isLoading = true;
        SceneManager.LoadScene(sceneName);
    }
}

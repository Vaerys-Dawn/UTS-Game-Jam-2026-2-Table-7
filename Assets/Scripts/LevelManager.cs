using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Win/lose rules for a level:
//   Win  = every enemy (NPCController tagged "Enemy") is dead.
//   Lose = every NPC (NPCController tagged "NPC") is dead, or the player dies.
// Also shows the "Enemies: alive / max" counter.
//
// You don't need to place this in a scene: it adds itself to any scene that has
// an object tagged "Player". Retrying the level from the Lose scene reloads it fresh.
public class LevelManager : MonoBehaviour
{
    // The level the player was last in, so the Lose scene's "Try Again" can reload it.
    public static string LastLevelScene { get; private set; }

    private const string WinScene = "Win";
    private const string LoseScene = "Lose";
    private const float EndDelay = 1f; // lets the last death play out before switching scene

    private readonly List<NPCController> enemies = new List<NPCController>();
    private readonly List<NPCController> npcs = new List<NPCController>();
    private TextMeshProUGUI counterText;
    private int lastAliveEnemies = -1;
    private bool levelOver = false;

    private static LevelManager current;
    private static bool hooked = false;

    // Reset statics each play session (needed when domain reload is turned off in the editor).
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        hooked = false;
        current = null;
        LastLevelScene = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        if (!hooked)
        {
            SceneManager.sceneLoaded += (scene, mode) => TryCreate();
            hooked = true;
        }
        TryCreate(); // the first scene has already loaded by now
    }

    private static void TryCreate()
    {
        if (current != null) return;
        if (GameObject.FindWithTag("Player") == null) return; // menus, Win, Lose, etc.
        new GameObject("LevelManager").AddComponent<LevelManager>();
    }

    // Call this when the player is killed (e.g. by an obstacle).
    public static void PlayerDied()
    {
        if (current == null || current.levelOver) return;
        Debug.Log("LevelManager: player died.");
        current.EndLevel(LoseScene, 0f);
    }

    void Awake()
    {
        current = this;
        LastLevelScene = SceneManager.GetActiveScene().name;

        foreach (NPCController c in FindObjectsByType<NPCController>())
        {
            if (c.CompareTag("Enemy")) enemies.Add(c);
            else if (c.CompareTag("NPC")) npcs.Add(c);
        }

        CreateCounterUI();
        UpdateCounter(CountAlive(enemies));

        if (enemies.Count == 0) Debug.LogWarning("LevelManager: this level has no enemies, so it can't be won.");
        if (npcs.Count == 0) Debug.LogWarning("LevelManager: this level has no NPCs, so it can't be lost by NPCs dying.");
    }

    void OnDestroy()
    {
        if (current == this) current = null;
    }

    void Update()
    {
        if (levelOver) return;

        int aliveEnemies = CountAlive(enemies);
        UpdateCounter(aliveEnemies);

        // Lose is checked first so losing every NPC at the same moment as the last enemy is still a loss.
        if (npcs.Count > 0 && CountAlive(npcs) == 0)
            EndLevel(LoseScene, EndDelay);
        else if (enemies.Count > 0 && aliveEnemies == 0)
            EndLevel(WinScene, EndDelay);
    }

    private static int CountAlive(List<NPCController> list)
    {
        int alive = 0;
        foreach (NPCController c in list)
            if (c != null && !c.IsDead) alive++;
        return alive;
    }

    private void EndLevel(string sceneName, float delay)
    {
        levelOver = true;
        Debug.Log($"LevelManager: loading {sceneName}.");
        if (delay <= 0f) SceneManager.LoadScene(sceneName);
        else StartCoroutine(LoadAfter(sceneName, delay));
    }

    private System.Collections.IEnumerator LoadAfter(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    private void UpdateCounter(int aliveEnemies)
    {
        if (aliveEnemies == lastAliveEnemies || counterText == null) return;
        lastAliveEnemies = aliveEnemies;
        counterText.text = $"Enemies: {aliveEnemies} / {enemies.Count}";
    }

    // Builds a screen overlay with the counter in the top-left corner.
    private void CreateCounterUI()
    {
        GameObject canvasGO = new GameObject("LevelHUD", typeof(Canvas), typeof(CanvasScaler));
        canvasGO.transform.SetParent(transform, false);
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject textGO = new GameObject("EnemyCounter", typeof(RectTransform));
        textGO.transform.SetParent(canvasGO.transform, false);
        RectTransform rt = textGO.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(40f, -30f);
        rt.sizeDelta = new Vector2(600f, 80f);

        counterText = textGO.AddComponent<TextMeshProUGUI>();
        counterText.fontSize = 48;
        counterText.color = Color.white;
        counterText.alignment = TextAlignmentOptions.TopLeft;
        counterText.outlineWidth = 0.2f;
        counterText.outlineColor = Color.black;
        counterText.raycastTarget = false;
    }
}

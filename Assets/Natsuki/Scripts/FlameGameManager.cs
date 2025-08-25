using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class FlameGameManager : MonoBehaviour
{
    public static FlameGameManager Instance { get; private set; }

    [Header("Fire Icon")]
    public Image[] flameIcons;
    public Color onColor = Color.white;
    public Color offColor = new Color(1,1,1,0.25f);

    [Header("Win need")]
    public int targetCount = 5;
    public FirstPersonController player;

    [Header("Win UI")]
    public CanvasGroup winCanvas;
    public Image dimmer;
    public TextMeshProUGUI winTitle;
    public TextMeshProUGUI timeText;
    public Button backToStartButton;
    public string startSceneName = "Start_Scene";
    public float dimmerTargetAlpha = 0.85f;
    public float uiFadeTime = 0.35f;

    [Header("Win timing")]
    public float waitAfterLastTorch = 0.25f;  // 额外延迟秒数
    public float maxHintWait = 0.6f;          // 等待提示淡出的上限


    readonly List<Ignitable> litTorches = new List<Ignitable>();
    bool won = false;
    float levelStartTime;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        levelStartTime = Time.time;

        if (winCanvas)
        {
            winCanvas.alpha = 0f;
            winCanvas.interactable = false;
            winCanvas.blocksRaycasts = false;
        }
        if (dimmer) dimmer.color = new Color(0,0,0,0);

        if (backToStartButton)
            backToStartButton.onClick.AddListener(ReturnToStart);

        UpdateIcons();
    }

    public void OnTorchIgnited(Ignitable t)
    {
        if (!litTorches.Contains(t))
        {
            litTorches.Add(t);
            UpdateIcons();
            CheckWin();
        }
    }

    public void OnTorchExtinguished(Ignitable t)
    {
        if (litTorches.Remove(t))
        {
            UpdateIcons();
        }
    }

    void UpdateIcons()
    {
        if (flameIcons == null) return;
        int litCount = Mathf.Min(litTorches.Count, flameIcons.Length);
        for (int i = 0; i < flameIcons.Length; i++)
        {
            if (!flameIcons[i]) continue;
            flameIcons[i].color = i < litCount ? onColor : offColor;
        }
    }


    void CheckWin()
    {
        if (won) return;
        if (litTorches.Count >= targetCount)
        {
            won = true;
            StartCoroutine(WinSequence());
        }
    }

IEnumerator WinSequence()
{
    float elapsed = Time.time - levelStartTime;

    CanvasGroup hint = player ? player.interactHintGroup : null;
    if (hint)
    {
        float waited = 0f;
        while (hint.alpha > 0.01f && waited < maxHintWait)
        {
            waited += Time.deltaTime;
            yield return null;
        }
        if (waitAfterLastTorch > 0f)
            yield return new WaitForSeconds(waitAfterLastTorch);
    }

    if (player) player.enabled = false;
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    foreach (var ag in FindObjectsOfType<UnityEngine.AI.NavMeshAgent>())
        ag.isStopped = true;

    if (winCanvas)
    {
        winCanvas.alpha = 1f;
        winCanvas.interactable = true;
        winCanvas.blocksRaycasts = true;
    }

    float a0 = dimmer ? dimmer.color.a : 0f;
    float t = 0f;
    while (t < uiFadeTime)
    {
        t += Time.unscaledDeltaTime;
        float k = Mathf.Clamp01(t / uiFadeTime);
        if (dimmer)
        {
            var c = dimmer.color;
            c.a = Mathf.Lerp(a0, dimmerTargetAlpha, k);
            dimmer.color = c;
        }
        yield return null;
    }
    if (dimmer) { var c = dimmer.color; c.a = dimmerTargetAlpha; dimmer.color = c; }

    if (winTitle) winTitle.text = "ゲームクリア";
    if (timeText) timeText.text = $"プレイ時間：{FormatTime(elapsed)}";

    Time.timeScale = 0f;
}


    string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        float s = seconds - m * 60f;
        return $"{m:00}:{s:00.00}";
    }

    public void ReturnToStart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(startSceneName);
    }
}

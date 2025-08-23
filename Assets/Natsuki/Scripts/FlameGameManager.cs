using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FlameGameManager : MonoBehaviour
{
    public static FlameGameManager Instance { get; private set; }

    [Header("UI")]
    public Image[] flameIcons;              // 5個
    public Color onColor = Color.white;     // 明
    public Color offColor = new Color(1,1,1,0.25f); // 暗

    readonly List<Ignitable> litTorches = new List<Ignitable>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        UpdateIcons();
    }

    public void OnTorchIgnited(Ignitable t)
    {
        if (!litTorches.Contains(t))
        {
            litTorches.Add(t);
            UpdateIcons();
        }
    }

    public void OnTorchExtinguished(Ignitable t)
    {
        if (litTorches.Remove(t))
            UpdateIcons();
    }

    void UpdateIcons()
    {
        if (flameIcons == null) return;
        int litCount = Mathf.Min(litTorches.Count, flameIcons.Length);
        for (int i = 0; i < flameIcons.Length; i++)
        {
            if (flameIcons[i] == null) continue;
            flameIcons[i].color = i < litCount ? onColor : offColor;
        }
    }

    // マイク用
    public bool SpendFlameForShout()
    {
        if (litTorches.Count == 0) return false;
        int idx = Random.Range(0, litTorches.Count);
        var target = litTorches[idx];
        if (target != null) { target.Extinguish(); return true; }
        litTorches.RemoveAt(idx);
        UpdateIcons();
        return false;
    }
}

using UnityEngine;

public class MicInput1 : MonoBehaviour
{
    public EnemyMove enemyMove; 
    public ParticleSystem shoutEffect;
    
    [Header("マイク設定")]
    [Range(0f, 1f)] public float threshold = 0.1f; 
    public float stunDuration = 3.0f; 

    // ★重要：Microphone ではなく MicrophoneWebGL に変更
    private uMicrophoneWebGL.MicrophoneWebGL _mic;

    void Start()
    {
        // ★ここも MicrophoneWebGL に変更
        _mic = GetComponent<uMicrophoneWebGL.MicrophoneWebGL>();

        if (_mic == null)
        {
            Debug.LogError("同じオブジェクトに Microphone Web GL コンポーネントをアタッチしてください。");
            return;
        }

        // インスペクターの Events 欄で OnDataReceived を設定済みなので、
        // コードでの AddListener は不要です。

        _mic.Begin();
    }

    public void OnDataReceived(float[] samples)
    {
        float maxVolume = 0f;
        foreach (var s in samples)
        {
            maxVolume = Mathf.Max(maxVolume, Mathf.Abs(s));
        }

        if (maxVolume > threshold && enemyMove != null)
        {
            TriggerStun();
        }
    }

    void TriggerStun()
    {
        if (shoutEffect != null && !shoutEffect.isPlaying)
        {
            shoutEffect.Play();
        }
        enemyMove.Stun(stunDuration);
    }
}
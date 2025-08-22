using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicInput : MonoBehaviour
{
    // Inspectorから設定するEnemyMoveスクリプト
    [Tooltip("ひるませたい敵のEnemyMoveスクリプトを設定")]
    public EnemyMove enemyMove; 
    
    [Header("マイク設定")]
    [Tooltip("この音量を超えたら敵がひるむ")]
    [Range(0f, 1f)]
    public float threshold = 0.1f; 

    [Tooltip("敵がひるんでいる時間（秒）")]
    public float stunDuration = 3.0f; 

    private AudioSource audioSource;
    private string microphoneDevice;
    private float[] samples = new float[128]; // 音声データ解析用の配列

    void Start()
    {
        // 利用可能なマイクがない場合、処理を中断
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("マイクが見つかりません。");
            return;
        }

        audioSource = GetComponent<AudioSource>();
        microphoneDevice = Microphone.devices[0]; // 最初のマイクを使用

        // マイクからの入力をAudioClipとして設定し、再生を開始
        audioSource.clip = Microphone.Start(microphoneDevice, true, 10, 44100);
        audioSource.loop = true;

        // マイクが準備できるまで待機
        while (!(Microphone.GetPosition(microphoneDevice) > 0)) { }
        audioSource.Play();
    }

    void Update()
    {
        // マイクが認識されていない場合は何もしない
        if (string.IsNullOrEmpty(microphoneDevice)) return;

        // 現在の音量を取得
        float volume = GetMicrophoneVolume();

        // 音量がしきい値を超え、かつ敵が設定されている場合
        if (volume > threshold && enemyMove != null)
        {
            // 敵のStunメソッドを呼び出す
            enemyMove.Stun(stunDuration);
        }
    }

    // マイクの音量を取得するメソッド
    float GetMicrophoneVolume()
    {
        float maxVolume = 0f;
        // 現在の再生位置から音声データを取得
        audioSource.GetOutputData(samples, 0);

        // 配列内の最大値（絶対値）を探す
        foreach (var s in samples)
        {
            maxVolume = Mathf.Max(maxVolume, Mathf.Abs(s));
        }
        return maxVolume;
    }
}
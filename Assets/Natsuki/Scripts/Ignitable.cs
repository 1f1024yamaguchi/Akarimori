using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Ignitable : MonoBehaviour
{
    [Header("Lights")]
    public Light mainLight;
    public Light bounceLight;
    [Range(0.05f, 3f)] public float fadeTime = 0.5f;
    public float mainTargetIntensity = 2.5f;
    public float bounceTargetIntensity = 0.5f;

    [Header("FX")]
    public ParticleSystem flameFX;
    [Tooltip("3D 音")]
    public AudioSource igniteSfx;
    public AudioSource extinguishSfx;
    [Range(1f, 50f)] public float sfxMaxDistance = 15f;

    [Header("State")]
    public bool isLit = false;

    void Awake()
    {
        Configure3DAudio(igniteSfx);
        Configure3DAudio(extinguishSfx);

        ApplyInstant(isLit);

        if (isLit) FlameGameManager.Instance?.OnTorchIgnited(this);
    }

    void OnValidate()
    {
        Configure3DAudio(igniteSfx);
        Configure3DAudio(extinguishSfx);

        if (!Application.isPlaying) ApplyInstant(isLit);
    }

    [ContextMenu("Ignite")]
    public void Ignite()
    {
        if (isLit) return;
        isLit = true;

        igniteSfx?.Play();
        if (flameFX) flameFX.Play();

        StartFade(toOn: true);
        FlameGameManager.Instance?.OnTorchIgnited(this);
    }

    [ContextMenu("Extinguish")]
    public void Extinguish()
    {
        if (!isLit) return;
        isLit = false;

        extinguishSfx?.Play();
        if (flameFX) flameFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        StartFade(toOn: false);
        FlameGameManager.Instance?.OnTorchExtinguished(this);
    }


    void StartFade(bool toOn)
    {
        if (mainLight)
            StartCoroutine(FadeLight(mainLight, mainLight.intensity, toOn ? mainTargetIntensity : 2f, fadeTime, disableAtEnd: !toOn));
        if (bounceLight)
            StartCoroutine(FadeLight(bounceLight, bounceLight.intensity, toOn ? bounceTargetIntensity : 0f, fadeTime, disableAtEnd: !toOn));
    }

    IEnumerator FadeLight(Light l, float from, float to, float t, bool disableAtEnd)
    {
        l.enabled = true;
        float e = 0f;
        t = Mathf.Max(0.0001f, t);
        while (e < t)
        {
            e += Time.deltaTime;
            l.intensity = Mathf.Lerp(from, to, e / t);
            yield return null;
        }
        l.intensity = to;
        if (disableAtEnd && to <= 0f) l.enabled = false;
    }

    void Configure3DAudio(AudioSource s)
    {
        if (!s) return;
        s.playOnAwake = false;
        s.spatialBlend = 1f;                         // 3D
        s.rolloffMode = AudioRolloffMode.Linear;
        s.minDistance = 1f;
        s.maxDistance = sfxMaxDistance;
        s.dopplerLevel = 0f;
    }

    void ApplyInstant(bool on)
    {
        if (mainLight)
        {
            mainLight.enabled = on;
            mainLight.intensity = on ? mainTargetIntensity : 0f;
        }
        if (bounceLight)
        {
            bounceLight.enabled = on;
            bounceLight.intensity = on ? bounceTargetIntensity : 0f;
        }
        if (flameFX)
        {
            if (on && !flameFX.isPlaying) flameFX.Play(true);
            if (!on && flameFX.isPlaying) flameFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

#if UNITY_EDITOR
    void Reset()
    {
        if (!mainLight) mainLight = GetComponentInChildren<Light>(true);
        if (!flameFX) flameFX = GetComponentInChildren<ParticleSystem>(true);
        var audios = GetComponentsInChildren<AudioSource>(true);
        if (!igniteSfx && audios.Length > 0) igniteSfx = audios[0];
        if (!extinguishSfx && audios.Length > 1) extinguishSfx = audios[1];

        isLit = false;
        ApplyInstant(false);
        Configure3DAudio(igniteSfx);
        Configure3DAudio(extinguishSfx);
    }
#endif
}

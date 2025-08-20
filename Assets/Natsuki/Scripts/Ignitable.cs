// Assets/Scripts/Ignitable.cs
using UnityEngine;
using System.Collections;

public class Ignitable : MonoBehaviour
{
    public Light flameLight;
    public ParticleSystem flameFX;
    public AudioSource igniteSfx;
    public bool isLit = false;
    public float fadeTime = 0.5f;
    public float targetIntensity = 5f;

    [ContextMenu("Ignite")]
    public void Ignite()
    {
        if (isLit) return;
        isLit = true;
        if (igniteSfx) igniteSfx.Play();
        if (flameFX) flameFX.Play();
        if (flameLight) StartCoroutine(FadeLight(flameLight, 0f, targetIntensity, fadeTime));
    }

    IEnumerator FadeLight(Light l, float from, float to, float t)
    {
        l.enabled = true;
        l.intensity = from;
        float e = 0f;
        while (e < t)
        {
            e += Time.deltaTime;
            l.intensity = Mathf.Lerp(from, to, e / t);
            yield return null;
        }
        l.intensity = to;
    }

    void Reset()
    {
        if (!flameLight) flameLight = GetComponentInChildren<Light>(true);
        if (!flameFX) flameFX = GetComponentInChildren<ParticleSystem>(true);
        if (!igniteSfx) igniteSfx = GetComponentInChildren<AudioSource>(true);
        if (flameLight) { flameLight.enabled = false; flameLight.intensity = 0f; }
        if (flameFX) flameFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}

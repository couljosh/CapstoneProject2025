using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class BombAmmoBar : MonoBehaviour
{
    public List<Image> ammoSegments = new List<Image>();
    public Image regenerationFill;

    [Header("Visual Settings")]
    public Color chargedColor = Color.yellow;
    public Color chargingColor = Color.gray;
    public Color emptyColor = Color.black;
    public float fullIntensity = 1.0f;
    public float unchargedIntensity = 0.5f;

    [Tooltip("Alpha value (0.0 to 1.0) for segments that are not yet charged.")]
    public float emptySegmentAlpha = 0.5f;

    [Header("Regen Pop Effect Settings")]
    public RectTransform bombIconTransform;
    public float regenPopScale = 1.3f;
    public float regenPopDuration = 0.15f;

    [Header("Bomb Used Effect Settings")]
    public float usedPopScale = 0.8f;
    public float usedPopDuration = 0.1f;

    private BombSpawn bombSpawn;
    private bool popEffectActive = false;
    private int lastKnownBombCount = 0;

    public void Initialize(BombSpawn spawnScript, float maxRegenTime)
    {
        bombSpawn = spawnScript;
        if (ammoSegments.Count != bombSpawn.playerStats.maxBombsHeld)
        {
            Debug.LogError("bomb ui player max bombs doesnt match ammo segments");
        }
        lastKnownBombCount = bombSpawn.CurrentBombsHeld;
        UpdateUI(bombSpawn.CurrentBombsHeld, 0);
    }

    void Update()
    {
        if (bombSpawn != null)
        {
            int currentBombs = bombSpawn.CurrentBombsHeld;

            if (currentBombs > lastKnownBombCount)
            {
                if (!popEffectActive && bombIconTransform != null)
                {
                    StartRegenPopEffect();
                }
            }

            UpdateUI(currentBombs, bombSpawn.RegenerationProgress);

            lastKnownBombCount = currentBombs;
        }
    }

    private void UpdateUI(int currentBombs, float progress)
    {
        int maxSegments = ammoSegments.Count;

        for (int i = 0; i < maxSegments; i++)
        {
            Image segment = ammoSegments[i];

            if (i < currentBombs)
            {
                segment.color = chargedColor;
                segment.fillAmount = 1f;
                segment.material.SetFloat("_EmissionIntensity", fullIntensity);
            }

            else if (i == currentBombs && currentBombs < maxSegments)
            {
                if (progress >= 1f)
                {
                    segment.color = chargedColor;
                    segment.fillAmount = 1f;
                    segment.material.SetFloat("_EmissionIntensity", fullIntensity);
                }
                else
                {
                    segment.color = chargingColor;
                    segment.fillAmount = progress;
                    segment.material.SetFloat("_EmissionIntensity", unchargedIntensity);
                }
            }

            else
            {
                Color dimmed = emptyColor;
                dimmed.a = emptySegmentAlpha;

                segment.color = dimmed;
                segment.fillAmount = 0f;
                segment.material.SetFloat("_EmissionIntensity", unchargedIntensity);
            }
        }
    }

    private void StartRegenPopEffect()
    {
        if (bombIconTransform == null) return;
        popEffectActive = true;
        StartCoroutine(RegenPopAnimation());
    }

    private IEnumerator RegenPopAnimation()
    {
        Vector3 startScale = bombIconTransform.localScale;
        Vector3 maxScale = startScale * regenPopScale;
        float timer = 0f;

        while (timer < regenPopDuration)
        {
            timer += Time.deltaTime;
            float t = timer / regenPopDuration;
            bombIconTransform.localScale = Vector3.Lerp(startScale, maxScale, t);
            yield return null;
        }
        bombIconTransform.localScale = maxScale;

        timer = 0f;

        while (timer < regenPopDuration)
        {
            timer += Time.deltaTime;
            float t = timer / regenPopDuration;
            bombIconTransform.localScale = Vector3.Lerp(maxScale, startScale, t);
            yield return null;
        }

        bombIconTransform.localScale = startScale;
        popEffectActive = false;
    }

    public void StartUsedPopEffect()
    {
        if (bombIconTransform == null) return;
        if (!popEffectActive)
        {
            popEffectActive = true;
            StartCoroutine(UsedPopAnimation());
        }
    }

    private IEnumerator UsedPopAnimation()
    {
        Vector3 startScale = bombIconTransform.localScale;
        Vector3 maxScale = startScale * usedPopScale;
        float timer = 0f;

        while (timer < usedPopDuration)
        {
            timer += Time.deltaTime;
            float t = timer / usedPopDuration;
            bombIconTransform.localScale = Vector3.Lerp(startScale, maxScale, t);
            yield return null;
        }
        bombIconTransform.localScale = maxScale;

        timer = 0f;

        while (timer < usedPopDuration)
        {
            timer += Time.deltaTime;
            float t = timer / usedPopDuration;
            bombIconTransform.localScale = Vector3.Lerp(maxScale, startScale, t);
            yield return null;
        }

        bombIconTransform.localScale = startScale;
        popEffectActive = false;
    }
}
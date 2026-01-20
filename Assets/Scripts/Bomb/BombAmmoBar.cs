using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BombAmmoBar : MonoBehaviour
{
    public List<Image> ammoSegments = new List<Image>();
    public Image regenerationFill;

    [Header("Visual Settings")]
    public Color chargedColorBlue = Color.blue;
    public Color chargedColorYellow = Color.yellow;
    public Color chargingColor = Color.gray;
    public Color emptyColor = Color.black;
    public TextMeshProUGUI playerNumberText;

    public float fullIntensity = 1.0f;
    public float unchargedIntensity = 0.5f;

    [Tooltip("Alpha value (0.0 to 1.0) for segments that are not yet charged.")]
    public float emptySegmentAlpha = 0.5f;

    //team variable
    [HideInInspector] public bool isBlueTeam;

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

    public void Initialize(BombSpawn spawnScript, bool playerIsBlueTeam, int playerNumber)
    {
        bombSpawn = spawnScript;
        isBlueTeam = playerIsBlueTeam;

        if (playerNumberText != null)
        {
            playerNumberText.text = "P" + playerNumber.ToString();
        }

        // Make materials unique (prevents shared-material bugs)
        foreach (var seg in ammoSegments)
        {
            if (seg.material != null)
                seg.material = Instantiate(seg.material);
        }

        if (ammoSegments.Count != bombSpawn.playerStats.maxBombsHeld)
        {
            Debug.LogError("BombAmmoBar: ammo segments count mismatch");
        }

        lastKnownBombCount = bombSpawn.CurrentBombsHeld;
        UpdateUI(bombSpawn.CurrentBombsHeld, 0);
    }

    void Update()
    {
        if (bombSpawn == null) return;

        int currentBombs = bombSpawn.CurrentBombsHeld;

        if (currentBombs > lastKnownBombCount &&
            !popEffectActive &&
            bombIconTransform != null)
        {
            StartRegenPopEffect();
        }

        UpdateUI(currentBombs, bombSpawn.RegenerationProgress);
        lastKnownBombCount = currentBombs;
    }

    private Color GetChargedColor()
    {
        return isBlueTeam ? chargedColorBlue : chargedColorYellow;
    }

    private void UpdateUI(int currentBombs, float progress)
    {
        for (int i = 0; i < ammoSegments.Count; i++)
        {
            Image segment = ammoSegments[i];

            // Fully charged
            if (i < currentBombs)
            {
                segment.color = GetChargedColor();
                segment.fillAmount = 1f;
                segment.material.SetFloat("_EmissionIntensity", fullIntensity);
            }
            // Charging
            else if (i == currentBombs && currentBombs < ammoSegments.Count)
            {
                segment.color = chargingColor;
                segment.fillAmount = progress;
                segment.material.SetFloat("_EmissionIntensity", unchargedIntensity);
            }
            // Empty
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

    //pop effects
    private void StartRegenPopEffect()
    {
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
            bombIconTransform.localScale =
                Vector3.Lerp(startScale, maxScale, timer / regenPopDuration);
            yield return null;
        }

        timer = 0f;

        while (timer < regenPopDuration)
        {
            timer += Time.deltaTime;
            bombIconTransform.localScale =
                Vector3.Lerp(maxScale, startScale, timer / regenPopDuration);
            yield return null;
        }

        bombIconTransform.localScale = startScale;
        popEffectActive = false;
    }

    public void StartUsedPopEffect()
    {
        if (!popEffectActive)
            StartCoroutine(UsedPopAnimation());
    }

    private IEnumerator UsedPopAnimation()
    {
        Vector3 startScale = bombIconTransform.localScale;
        Vector3 minScale = startScale * usedPopScale;
        float timer = 0f;

        while (timer < usedPopDuration)
        {
            timer += Time.deltaTime;
            bombIconTransform.localScale =
                Vector3.Lerp(startScale, minScale, timer / usedPopDuration);
            yield return null;
        }

        timer = 0f;

        while (timer < usedPopDuration)
        {
            timer += Time.deltaTime;
            bombIconTransform.localScale =
                Vector3.Lerp(minScale, startScale, timer / usedPopDuration);
            yield return null;
        }

        bombIconTransform.localScale = startScale;
        popEffectActive = false;
    }
}
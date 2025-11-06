using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BombAmmoBar : MonoBehaviour
{
    public List<Image> ammoSegments = new List<Image>(); //list of ammo bar segments to spawn and we can change this to add more or less if we want to balance
    public Image regenerationFill;

    [Header("Visual Settings")]
    public Color chargedColor = Color.yellow;
    public Color chargingColor = Color.gray;
    public Color emptyColor = Color.black;
    public float fullIntensity = 1.0f;
    public float unchargedIntensity = 0.5f;

    [Tooltip("Alpha value (0.0 to 1.0) for segments that are not yet charged.")]
    public float emptySegmentAlpha = 0.5f; //transparency in bar so its easier to see the loading bar.

    private BombSpawn bombSpawn;

    public void Initialize(BombSpawn spawnScript, float maxRegenTime)
    {
        bombSpawn = spawnScript;
        if (ammoSegments.Count != bombSpawn.playerStats.maxBombsHeld)
        {
            Debug.LogError("bomb ui player max bombs doesnt match ammo segments");
        }
        UpdateUI(0, 0);
    }

    void Update()
    {
        if (bombSpawn != null)
        {
            UpdateUI(bombSpawn.CurrentBombsHeld, bombSpawn.RegenerationProgress);
        }
    }

    private void UpdateUI(int currentBombs, float progress)
    {
        int maxBombs = bombSpawn.playerStats.maxBombsHeld;
        int maxSegments = ammoSegments.Count;

        //total charge progress
        float totalMax = (float)maxBombs;
        float totalChargeProgress = (currentBombs + progress) / totalMax;

        //update ammo segments based on total charge
        for (int i = 0; i < maxSegments; i++)
        {
            Image segment = ammoSegments[i];

            //percentage for segment to be on
            float segmentThreshold = (float)(i + 1) / totalMax;

            if (totalChargeProgress >= segmentThreshold)
            {
                //fully charged
                segment.color = chargedColor;
                segment.material.SetFloat("_EmissionIntensity", fullIntensity);
            }
            else
            { //not charged yet
                Color dimmedColor = emptyColor;
                dimmedColor.a = emptySegmentAlpha;
                segment.color = dimmedColor;

                segment.material.SetFloat("_EmissionIntensity", unchargedIntensity);
            }
        }

        //update fill
        if (maxBombs > 0)
        {
            //scale progress to fill the whole regenerationFill bar from 0 to 1
            regenerationFill.color = chargingColor;
            regenerationFill.fillAmount = totalChargeProgress;
            regenerationFill.enabled = true;
        }

        if (totalChargeProgress >= 1.0f)
        {
            regenerationFill.enabled = false;
        }
    }
}
using UnityEngine;

public class OutlineColorCycle : MonoBehaviour
{

    public Gradient colorGradient;
    private Outline outlineScript;
    private Light spotLight;
    private float startTime;
    public float timeToFullCycle;

    private void Start()
    {
        //reference crate's outline and spot light
        outlineScript = gameObject.GetComponent<Outline>();
        spotLight = gameObject.GetComponentInChildren<Light>();
    }

    void Update()
    {
        //increase progress
        startTime += Time.deltaTime;

        //set color based on gradient progress
        float progress = startTime / timeToFullCycle;
        outlineScript.OutlineColor = colorGradient.Evaluate(progress);
        spotLight.color = colorGradient.Evaluate(progress);

        //reset progress at 0
        if(startTime > timeToFullCycle)
        {
            startTime = 0;
        }
    }
}

using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public AnimationCurve curve;
    public float duration = 1f;


    public ParticleSystem caveinVFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            caveinVFX.enableEmission = false;

        //camera starts at 0,0,0 relative to parent
        transform.localPosition = Vector3.zero;
    }

    public void CallShake()
    {
        StopAllCoroutines();
        StartCoroutine(Shaking());
    }

    public IEnumerator Shaking()
    {
        Vector3 startPositionLocal = Vector3.zero;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.localPosition = startPositionLocal + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.localPosition = startPositionLocal;
    }
}
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool start = false;
    public AnimationCurve curve;
    public float duration;

    public ParticleSystem caveinVFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        caveinVFX.enableEmission = false;
    }

    // Update is called once per frame
    void Update()
    {
       if (start)
       {
            //start = false;
           // StartCoroutine(Shaking());
        }
    }

    public void CallShake()
    {
        StartCoroutine(Shaking());
    }



    public IEnumerator Shaking()
    {

        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.position = startPosition;
    }
}

using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RandomGemParticleColor : MonoBehaviour
{
    public GemRef gemRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int rand = Random.Range(0, gemRef.colors.Count);
        print(rand);
        var ps = gameObject.GetComponent<ParticleSystem>().main;
        ps.startColor = new MinMaxGradient(gemRef.colors[rand]);
    }

    
}

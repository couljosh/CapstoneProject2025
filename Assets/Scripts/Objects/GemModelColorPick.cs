using UnityEngine;

public class GemModelColorPick : MonoBehaviour
{
    public GemRef gemRef;
    public bool needsModel;
    public bool needsColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int rand = Random.Range(0, gemRef.meshes.Count);

        if (needsModel)
        {
            //pick mesh, material and size
            gameObject.GetComponent<MeshFilter>().mesh = gemRef.meshes[rand];
            gameObject.GetComponent<MeshRenderer>().material = gemRef.materials[rand];
            gameObject.transform.localScale = (gemRef.scales[rand]);

        }

        if (needsColor)
        {
            //pick color
            rand = Random.Range(0, gemRef.colors.Count);
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", gemRef.colors[rand]);
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", gemRef.emiColors[rand]);
        }


        //pick random rotation
        gameObject.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

    }

}

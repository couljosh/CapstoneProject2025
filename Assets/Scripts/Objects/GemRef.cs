using System.Collections.Generic;
using UnityEditor.SpeedTree.Importer;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "GemRef", menuName = "Scriptable Objects/GemRef")]

//scriptable object for gem models and colors
public class GemRef : ScriptableObject
{
    [Header("BASE GEM COLORS")]
    public List<Color32> colors = new List<Color32>();

    [Header("EMISSION GEM COLORS")]
    [Header("NEEDS TO MATCH BASEGEM COLOR - the emission is a different shade of the base color as set by Yates originally.")]
    [ColorUsage(true, true)]
    public List<Color32> emiColors = new List<Color32>();

    [Header("GEM MESHES")]
    public List<Mesh> meshes = new List<Mesh>();

    [Header("GEM SCALE")]
    [Header("Will pick the same number as the mesh. - this was done because some models are much smaller than others. \n For example, the diamond will use the scale in the same element position as it.")]
    public List <Vector3> scales = new List<Vector3>();

    [Header("GEM MATERIALS")]
    [Header("NOTE: In order for each gem to have the correct material for its mesh,\n ensure that the gem mesh has its associated material. For example, Diamond (mesh) and Diamond \n (material) should be on the same element number.")]
    public List<Material> materials = new List<Material>();
}

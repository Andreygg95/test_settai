using System.Collections.Generic;
using UnityEngine;

public class MaterialManager
{
    public static List<Material> CopySharedMaterials(Transform target)
    {
        Renderer[] childRenderers = target.GetComponentsInChildren<Renderer>(true);

        Dictionary<Material, Material> materialMap = new Dictionary<Material, Material>();

        foreach (Renderer childRenderer in childRenderers)
        {
            var sharedMaterials = childRenderer.sharedMaterials;

            var newMaterials = new Material[sharedMaterials.Length];

            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                Material originalMaterial = sharedMaterials[i];
                Material newMaterial;

                if (!materialMap.ContainsKey(originalMaterial))
                {
                    newMaterial = new Material(originalMaterial);
                    materialMap.Add(originalMaterial, newMaterial);
                }
                
                newMaterial = materialMap[originalMaterial];

                newMaterials[i] = newMaterial;
            }

            childRenderer.sharedMaterials = newMaterials;
        }

        return new List<Material>(materialMap.Values);
    }
}
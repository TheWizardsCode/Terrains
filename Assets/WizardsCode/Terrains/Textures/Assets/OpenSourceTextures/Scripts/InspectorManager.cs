using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardsCode.Texture
{
    public class InspectorManager : MonoBehaviour
    {
        public GameObject[] inspectorObjects;

        public void SetMaterial(Material material)
        {
            for (int i = 0; i < inspectorObjects.Length; i++)
            {
                inspectorObjects[i].GetComponent<MeshRenderer>().material = material;
            }
        }
    }
}

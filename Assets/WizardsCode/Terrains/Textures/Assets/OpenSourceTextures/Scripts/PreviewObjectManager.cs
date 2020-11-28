using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace wizardscode.utility.textures
{
    public class PreviewObjectManager : MonoBehaviour
    {
        public SceneManager manager;

        private void Awake()
        {
            manager = GameObject.FindObjectOfType<SceneManager>();
        }

        void OnMouseDown()
        {
            manager.SetMaterialOnInspector(GetComponent<MeshRenderer>().material);
        }
    }
}

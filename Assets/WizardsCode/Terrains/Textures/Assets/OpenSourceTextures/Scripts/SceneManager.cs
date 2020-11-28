using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using WizardsCode.Controller;
using WizardsCode.Materials;
using WizardsCode.Texture;

namespace wizardscode.utility.textures
{
    public class SceneManager : MonoBehaviour
    {
        public Dropdown typeDropdown;

        [Header("Preview Area")]
        public GameObject previewArea;
        public Camera previewCamera;
        public GameObject previewObjectParent;
        public int gridSpacing = 3;

        [Header("Inspector Area")]
        public GameObject inspectorArea;
        public Camera inspectorCamera;

        [Header("Misc.")]
        Rect primaryViewportRect = new Rect(0, 0, 1, 1);
        Rect secondaryViewportRect = new Rect(0.75f, 0.75f, 1, 1);

        private InspectorManager inspectorManager;
        private bool isPreviewPrimaryView = true;

        private MaterialDatabase materialDatabase;
        private PlantDatabase plantDatabase;

        private void Start()
        {
            SetSelectionType();
            materialDatabase.InitData();
            plantDatabase.InitData();
        }

        public void SetSelectionType()
        {
            if (typeDropdown.value == 0)
            {
                plantDatabase.CategoryDropdown.gameObject.SetActive(false);
                plantDatabase.PlantDropdown.gameObject.SetActive(false);
                plantDatabase.texturePanel.gameObject.SetActive(false);
                materialDatabase.CategoryDropdown.gameObject.SetActive(true);
                materialDatabase.MaterialDropdown.gameObject.SetActive(true);
            }
            else
            {
                materialDatabase.CategoryDropdown.gameObject.SetActive(false);
                materialDatabase.MaterialDropdown.gameObject.SetActive(false);
                plantDatabase.CategoryDropdown.gameObject.SetActive(true);
                plantDatabase.PlantDropdown.gameObject.SetActive(true);
                plantDatabase.texturePanel.gameObject.SetActive(true);
            }
        }

        public void SetCategory()
        {
            if (typeDropdown.value == 0)
            {
                materialDatabase.PopulateMaterialList();
            }
            else
            {
                plantDatabase.PopulatePlantList();
            }
        }

        internal void SetMaterialOnInspector(Material material)
        {
            inspectorManager.SetMaterial(material);
        }

        private void Awake()
        {
            inspectorManager = inspectorArea.GetComponent<InspectorManager>();
            previewCamera.GetComponent<FlyCameraController>().enabled = true;
            inspectorCamera.GetComponent<FlyCameraController>().enabled = false;

            materialDatabase = GetComponent<MaterialDatabase>();
            plantDatabase = GetComponent<PlantDatabase>();
        }

        /// <summary>
        /// Sets up the preview area to view a set of materials
        /// </summary>
        /// <param name="materials"></param>
        internal void SetupPreview(Texture[] textures)
        {
            foreach (Transform child in plantDatabase.texturePanel.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < textures.Length; i++)
            {
                GameObject img = new GameObject("2D Texture");
                RawImage raw = img.AddComponent<RawImage>();
                raw.texture = textures[i];

                img.transform.SetParent(plantDatabase.texturePanel.transform);
            }
        }

        /// <summary>
        /// Sets up the preview area to view a set of materials
        /// </summary>
        /// <param name="materials"></param>
        internal void SetupPreview(Material[] materials)
        {
            foreach(Transform child in previewObjectParent.transform)
            {
                Destroy(child.gameObject);
            }

            int gridSize = (int)Mathf.Sqrt(materials.Length) + 1;

            Vector3 pos = Vector2.zero;
            int row = 0;
            int col = 0;

            for (int i = 0; i < materials.Length; i++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.AddComponent<PreviewObjectManager>();
                sphere.name = materials[i].name + " Sphere";
                sphere.transform.SetParent(previewObjectParent.transform);
                pos.x = row * gridSpacing;
                pos.y = 1;
                pos.z = col * gridSpacing;
                sphere.transform.position = pos;

                MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
                meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                meshRenderer.receiveShadows = false;
                meshRenderer.material = materials[i];

                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.AddComponent<PreviewObjectManager>();
                plane.name = materials[i].name + " Plane";
                plane.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                plane.transform.SetParent(previewObjectParent.transform);
                pos.x = row * gridSpacing;
                pos.y = 0;
                pos.z = col * gridSpacing;
                plane.transform.position = pos;

                meshRenderer = plane.GetComponent<MeshRenderer>();
                meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                meshRenderer.receiveShadows = false;
                meshRenderer.material = materials[i];

                col++;
                if (col == gridSize)
                {
                    col = 0;
                    row++;
                    if (row == gridSize)
                    {
                        row = 0;
                    }
                }
            }

            isPreviewPrimaryView = false;
            ToggleView();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                ToggleView();
            }
        }

        private void ToggleView()
        {
            if (isPreviewPrimaryView)
            {
                inspectorCamera.rect = primaryViewportRect;
                inspectorCamera.GetComponent<FlyCameraController>().enabled = true;
                inspectorCamera.depth = -1;

                previewCamera.rect = secondaryViewportRect;
                previewCamera.GetComponent<FlyCameraController>().enabled = false;
                previewCamera.depth = 0;

                isPreviewPrimaryView = false;
            }
            else
            {
                previewCamera.rect = primaryViewportRect;
                previewCamera.GetComponent<FlyCameraController>().enabled = true;
                previewCamera.depth = -1;

                inspectorCamera.rect = secondaryViewportRect;
                inspectorCamera.GetComponent<FlyCameraController>().enabled = false;
                inspectorCamera.depth = 0;

                isPreviewPrimaryView = true;
            }
        }
    }
}

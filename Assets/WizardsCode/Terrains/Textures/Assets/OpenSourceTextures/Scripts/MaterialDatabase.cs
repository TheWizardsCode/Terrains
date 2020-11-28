using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using wizardscode.utility.textures;
using WizardsCode.Controller;

namespace WizardsCode.Materials
{
    /// <summary>
    /// The model database collects and manages information about all the models available in the system.
    /// </summary>
    public class MaterialDatabase : MonoBehaviour
    {
        // UI
        public Dropdown CategoryDropdown;
        public Dropdown MaterialDropdown;

        private SceneManager sceneManager;

        Material currentMaterial;
        GameObject lookAt;
        List<MaterialMetaData> materialData = new List<MaterialMetaData>();

        private void Awake()
        {
            sceneManager = FindObjectOfType<SceneManager>();
        }

        internal string ProjectFolder
        {
            get
            {
                string folder = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
                folder = folder.Substring(0, folder.IndexOf("/Scripts/"));
                return folder;
            }
        }

        internal void InitData()
        {
            MaterialMetaData[] data = FindAllMaterials();
            foreach (MaterialMetaData material in data)
            {
                materialData.Add(material);
            }

            PopulateCategoryList();
            PopulateMaterialList();

            while (MaterialDropdown.options.Count <= 0)
            {
                CategoryDropdown.value++;
            }
        }

        private void PopulateCategoryList()
        {
            CategoryDropdown.options.Clear();
            List<Dropdown.OptionData> categories = new List<Dropdown.OptionData>();
            foreach (MaterialMetaData.Category category in Enum.GetValues(typeof(MaterialMetaData.Category)))
            {
                categories.Add(new MaterialMetaData.CategoryOptionData(category));
            }

            CategoryDropdown.AddOptions(categories);
        }

        public void PopulateMaterialList()
        {
            MaterialDropdown.options.Clear();
            List<Dropdown.OptionData> items = new List<Dropdown.OptionData>();
            List<Material> materials = new List<Material>();

            foreach (MaterialMetaData data in materialData)
            {
                int idx = CategoryDropdown.value;
                if (data.m_category == ((MaterialMetaData.CategoryOptionData)CategoryDropdown.options[idx]).m_category)
                {
                    items.Add(new MaterialMetaData.MetaDataOptionData(data));
                    materials.Add(data.m_Material);
                }
            }

            MaterialDropdown.AddOptions(items);

            sceneManager.SetupPreview(materials.ToArray());

            SelectMaterial(0);
        }

        public void SelectMaterial(int index)
        {
            if (MaterialDropdown.options.Count == 0)
            {
                return;
            }

            currentMaterial = ((MaterialMetaData.MetaDataOptionData)MaterialDropdown.options[MaterialDropdown.value]).data.m_Material;

            EditorGUIUtility.PingObject(currentMaterial);

            sceneManager.SetMaterialOnInspector(currentMaterial);
        }

        internal MaterialMetaData[] FindAllMaterials()
        {
            List<MaterialMetaData> metaData = new List<MaterialMetaData>();

            string[] prefabGUIDs = AssetDatabase.FindAssets("t:material", new[] { ProjectFolder });
            foreach (string guid in prefabGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Material asset = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

                string dataPath = assetPath.Replace(".mat", ".asset");
                MaterialMetaData data = AssetDatabase.LoadAssetAtPath<MaterialMetaData>(dataPath);

                if (data == null)
                {
                    data = ScriptableObject.CreateInstance<MaterialMetaData>();
                    data.m_Material = asset;
                    data.name = asset.name;
                    AssetDatabase.CreateAsset(data, dataPath);
                }

                metaData.Add(data);
            }

            AssetDatabase.SaveAssets();

            return metaData.ToArray();
        }

        private void OnDestroy()
        {
            DestroyImmediate(lookAt);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using wizardscode.utility.textures;
using WizardsCode.Controller;

namespace WizardsCode.Texture
{
    /// <summary>
    /// The plant database collects and manages information about all the plants available in the system.
    /// </summary>
    public class PlantDatabase : MonoBehaviour
    {
        // UI
        public Dropdown CategoryDropdown;
        public Dropdown PlantDropdown;
        public GameObject texturePanel;

        private SceneManager sceneManager;

        UnityEngine.Texture currentPlant;
        GameObject lookAt;
        List<PlantMetaData> PlantData = new List<PlantMetaData>();

        private void Awake()
        {
            sceneManager = FindObjectOfType<SceneManager>();
        }

        internal void InitData()
        {
            PlantMetaData[] data = FindAllPlants();
            foreach (PlantMetaData plant in data)
            {
                PlantData.Add(plant);
            }

            PopulateCategoryList();
            PopulatePlantList();

            while (PlantDropdown.options.Count <= 0)
            {
                CategoryDropdown.value++;
            }
        }

        private void PopulateCategoryList()
        {
            CategoryDropdown.options.Clear();
            List<Dropdown.OptionData> categories = new List<Dropdown.OptionData>();
            foreach (PlantMetaData.Category category in Enum.GetValues(typeof(PlantMetaData.Category)))
            {
                categories.Add(new PlantMetaData.CategoryOptionData(category));
            }

            CategoryDropdown.AddOptions(categories);
        }

        public void PopulatePlantList()
        {
            PlantDropdown.options.Clear();
            List<Dropdown.OptionData> items = new List<Dropdown.OptionData>();
            List<UnityEngine.Texture> textures = new List<UnityEngine.Texture>();

            foreach (PlantMetaData data in PlantData)
            {
                int idx = CategoryDropdown.value;
                if (data.m_category == ((PlantMetaData.CategoryOptionData)CategoryDropdown.options[idx]).m_category)
                {
                    items.Add(new PlantMetaData.MetaDataOptionData(data));
                    textures.Add(data.m_Texture);
                }
            }

            PlantDropdown.AddOptions(items);

            sceneManager.SetupPreview(textures.ToArray());

            SelectTexture(0);
        }

        public void SelectTexture(int index)
        {
            if (PlantDropdown.options.Count == 0)
            {
                return;
            }

            currentPlant = ((PlantMetaData.MetaDataOptionData)PlantDropdown.options[PlantDropdown.value]).data.m_Texture;

            EditorGUIUtility.PingObject(currentPlant);
        }

        internal PlantMetaData[] FindAllPlants()
        {
            List<PlantMetaData> metaData = new List<PlantMetaData>();

            string[] prefabGUIDs = AssetDatabase.FindAssets("t:texture", new[] { "Assets/OpenSourceTextures/Plant" });
            foreach (string guid in prefabGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Texture asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Texture>(assetPath);

                string dataPath = assetPath.Replace(".jpg", ".asset");
                dataPath = dataPath.Replace(".bmp", ".asset");
                dataPath = dataPath.Replace(".gif", ".asset");
                dataPath = dataPath.Replace(".hdr", ".asset");
                dataPath = dataPath.Replace(".iff", ".asset");
                dataPath = dataPath.Replace(".pict", ".asset");
                dataPath = dataPath.Replace(".tiff", ".asset");
                dataPath = dataPath.Replace(".psd", ".asset");
                dataPath = dataPath.Replace(".exr", ".asset");
                dataPath = dataPath.Replace(".png", ".asset");
                dataPath = dataPath.Replace(".tga", ".asset");
                if (!dataPath.EndsWith(".asset"))
                {
                    Debug.LogError("Unable to process texture file: " + dataPath + " Probably just need to pattern match on the file extension.");
                }
                PlantMetaData data = AssetDatabase.LoadAssetAtPath<PlantMetaData>(dataPath);

                if (data == null)
                {
                    data = ScriptableObject.CreateInstance<PlantMetaData>();
                    data.m_Texture = asset;
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

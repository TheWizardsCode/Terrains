using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using UnityEditor.SceneManagement;
using System;

namespace WizardsCode.EditorUtils
{
    /// <summary>
    /// TerrainExporter enables a terrain, optionally including heightmap,
    /// splatmap, terrain details and trees, to be exported as a UnityPackage.
    /// </summary>
    public class TerrainExportWindow : EditorWindow
    {
        // TODO: Handle the situation where the Terrains submodule is not in the usual place
        static string m_TerrainDirectory = "Assets/Exported Terrains";
        static string m_TerrainSceneDirectory = m_TerrainDirectory + "/Scenes";
        static string m_TerrainDataDirectory = m_TerrainDirectory + "/Terrain Data";
        static string m_TerrainLayersDirectory = m_TerrainDirectory + "/Terrain Layers";

        bool exportWithTextures = false;
        bool exportWithDetails = false;
        bool exportWithTrees = false;
        bool exportWithSkybox = false;

        [MenuItem("Tools/Wizards Code/Terrain Export")]
        public static void ShowWindow()
        {
            GetWindow<TerrainExportWindow> (false, "Terrain Export", true);
        }

        private void OnGUI()
        {
            string docs = "1. Open a scene containing the terrain(s) you want to export"
                + "\n\n2. Select the information you want to export with the heightmap. "
                + "\n\n3. Click the export button, taking note of the folder your terrain will be placed in. "
                + "A new scene containing the terrain and chosen items will be created in the designated folder."
                //TODO automatically create a Unity Package for the terrain
                + "\n\n4. Select the export scene in the project window, right click and click `Select Dependencies`."
                + "\n\n5. Select `Assets/Export Package...`, remember to uncheck `Include Dependencies` if you don't want to include models etc. "
                + "\n\n6. Click Export to save a unity package containing the exported scene.";
            EditorGUILayout.HelpBox(docs, MessageType.Info);

            EditorGUILayout.BeginVertical();
            exportWithTextures = EditorGUILayout.ToggleLeft("Textures", exportWithTextures);
            exportWithDetails = EditorGUILayout.ToggleLeft("Details", exportWithDetails);
            exportWithTrees = EditorGUILayout.ToggleLeft("Trees", exportWithTrees);
            EditorGUILayout.EndVertical();

            string label = "Export heightmap";
            if (exportWithTextures) label += ", textures";
            if (exportWithDetails) label += ", details";
            if (exportWithTrees) label += ", trees";
            label += "\nto " + m_TerrainSceneDirectory;

            //TODO only show an active terrain button if a terrain is selected in the scene view
            if (GUILayout.Button(label))
            {
                ExportTerrain();
                //TODO ping the generated scene in the project view
            }
        }

        private void ExportTerrain()
        {
            AssetDatabase.Refresh();

            Directory.CreateDirectory(m_TerrainDirectory);
            Directory.CreateDirectory(m_TerrainSceneDirectory);
            Directory.CreateDirectory(m_TerrainDataDirectory);
            Directory.CreateDirectory(m_TerrainLayersDirectory);

            string exportSceneName = GenerateSceneName();
            Scene exportScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            exportScene.name = exportSceneName;

            CopyCameras(exportScene);
            CopyDirectionalLights(exportScene);
            CopyTerrains(exportScene, exportWithTextures, exportWithDetails, exportWithTrees);

            EditorSceneManager.SaveScene(exportScene, m_TerrainSceneDirectory + "/" + exportScene.name + ".unity");
            EditorSceneManager.CloseScene(exportScene, true);
        }

        private string GenerateSceneName()
        {
            string exportSceneName = EditorSceneManager.GetActiveScene().name;
            string features = "";
            if (exportWithTextures) features += " Textures";
            if (exportWithDetails) features += " Details";
            if (exportWithTrees) features += " Trees";
            if (features.Length > 0)
            {
                exportSceneName += " with";
                exportSceneName += features;
            }
            exportSceneName += " ";
            exportSceneName += DateTime.Now.ToFileTimeUtc();
            return exportSceneName.Replace(' ', '_');
        }

        private static void CopyTerrains(Scene exportScene, bool exportWithTextures, bool exportWithDetails, bool exportWithTrees)
        {
            Terrain[] terrains = Terrain.activeTerrains;
            for (int i = 0; i < terrains.Length; i++)
            {
                // Copy the Terrain Game Object
                Terrain newTerrain = CopyToScene(terrains[i].gameObject, exportScene).GetComponent<Terrain>();

                // Copy the Terrain Data
                TerrainData data = terrains[i].terrainData;
                string originalDataPath = AssetDatabase.GetAssetPath(data);
                string exportDataPath = m_TerrainDataDirectory + "/" + exportScene.name + "_" + i + ".asset";
                AssetDatabase.CopyAsset(originalDataPath, exportDataPath);
                TerrainData newData = AssetDatabase.LoadAssetAtPath<TerrainData>(exportDataPath);

                // Copy or Strip Textures
                if (exportWithTextures)
                {
                    TerrainLayer[] newLayers = new TerrainLayer[newData.terrainLayers.Length];

                    for (int l = 0; l < newData.terrainLayers.Length; l++)
                    {
                        TerrainLayer originalLayer = newData.terrainLayers[l];
                        string originalLayerPath = AssetDatabase.GetAssetPath(originalLayer);
                        string exportLayerPath = m_TerrainLayersDirectory + "/" + exportScene.name + "_layer_" + l + ".asset";
                        AssetDatabase.CopyAsset(originalLayerPath, exportLayerPath);
                        newLayers[l] = AssetDatabase.LoadAssetAtPath<TerrainLayer>(exportLayerPath);
                    }

                    newData.terrainLayers = newLayers;
                }
                else
                {
                    newData.terrainLayers = null;
                }

                // Strip Details?
                if (!exportWithDetails)
                {
                    for (int l = 0; l < newData.detailPrototypes.Length; l++)
                    {
                        int[,] map = newData.GetDetailLayer(0, 0, newData.detailWidth, newData.detailHeight, l);
                        for (var y = 0; y < newData.detailHeight; y++)
                        {
                            for (var x = 0; x < newData.detailWidth; x++)
                            {
                                map[x, y] = 0;
                            }
                        }
                        newData.SetDetailLayer(0, 0, l, map);
                    }

                    newData.detailPrototypes = new DetailPrototype[0];
                }

                // Strip Trees?
                if (!exportWithTrees)
                {
                    newData.treeInstances = new TreeInstance[0];
                    newData.treePrototypes = new TreePrototype[0];
                }

                newTerrain.terrainData = newData;
                newTerrain.GetComponent<TerrainCollider>().terrainData = newData;
            }
        }

        private static void CopyDirectionalLights(Scene exportScene)
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i].type == LightType.Directional)
                {
                    CopyToScene(lights[i].gameObject, exportScene);
                }
            }
        }

        private static void CopyCameras(Scene exportScene)
        {
            Camera[] cameras = Camera.allCameras;
            for (int i = 0; i < cameras.Length; i++)
            {
                CopyToScene(cameras[i].transform.root.gameObject, exportScene);
            }
        }

        /// <summary>
        /// Copy a game object to a new scene and return the new object
        /// </summary>
        /// <param name="originalGo">The object to copy</param>
        /// <param name="toScene">The scene to copy to</param>
        /// <returns>The object in the new scene</returns>
        private static GameObject CopyToScene(GameObject originalGo, Scene toScene)
        {
            GameObject newGo = GameObject.Instantiate(originalGo); ;
            newGo.transform.parent = null;
            SceneManager.MoveGameObjectToScene(newGo, toScene);
            newGo.name = newGo.name.Substring(0, newGo.name.Length - "(Clone)".Length);
            return newGo;
        }
    }
}

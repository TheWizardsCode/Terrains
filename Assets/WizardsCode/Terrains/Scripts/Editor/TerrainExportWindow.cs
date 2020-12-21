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
        static string m_TerrainPackagesDirectory = "Assets/WizardsCode/Terrains/Terrain Packages";
        static string m_TerrainSceneDirectory = m_TerrainPackagesDirectory + "/Scenes";
        static string m_TerrainDataDirectory = m_TerrainPackagesDirectory + "/Terrain Data";
        static string m_TerrainLayersDirectory = m_TerrainPackagesDirectory + "/Terrain Layers";

        bool exportWithTextures = false;
        bool exportWithDetails = false;
        bool exportWithTrees = false;

        static List<string> assetsToPackage;

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
                + "\n\n5. Select `Assets/Export Package...`, if you don't have the rights to redistribute the models and " +
                "textures remember to uncheck `Include Dependencies`."
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

        static float numOfSteps = 10;
        private void ExportTerrain()
        {
            assetsToPackage = new List<string>();

            EditorUtility.DisplayProgressBar("Exporting Terrain", "Preparing assets...", 0 / numOfSteps);

            AssetDatabase.Refresh();

            Directory.CreateDirectory(m_TerrainPackagesDirectory);
            Directory.CreateDirectory(m_TerrainSceneDirectory);
            Directory.CreateDirectory(m_TerrainDataDirectory);
            Directory.CreateDirectory(m_TerrainLayersDirectory);

            EditorUtility.DisplayProgressBar("Exporting Terrain", "Creating scene...", 1 / numOfSteps);

            string exportSceneName = GenerateSceneName();
            string scenePath = m_TerrainSceneDirectory + "/" + exportSceneName;
            if (File.Exists(scenePath + "( clone).unity"))
            {
                if (!EditorUtility.DisplayDialog("Scene Exists", "A scene with the name " + exportSceneName + " already exists. Do you want to overwrite the scene?", "Yes", "No"))
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }
            }

            Scene exportScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            exportScene.name = exportSceneName;

            EditorUtility.DisplayProgressBar("Exporting Terrain", "Copying lights and camera...", 2 / numOfSteps);
            CopyCameras(exportScene);
            CopyDirectionalLights(exportScene);

            CopyTerrains(exportScene, exportWithTextures, exportWithDetails, exportWithTrees);

            EditorUtility.DisplayProgressBar("Exporting Terrain", "Saving Scene...", numOfSteps - 3 / numOfSteps);
            EditorSceneManager.SaveScene(exportScene, scenePath + " (clone).unity");

            EditorUtility.DisplayProgressBar("Exporting Terrain", "Saving unitypackage...", numOfSteps - 2 / numOfSteps);
            assetsToPackage.Add(exportScene.path);
            AssetDatabase.ExportPackage(assetsToPackage.ToArray(), Application.dataPath + "/" + m_TerrainPackagesDirectory.Substring(6) + "/" + exportSceneName + ".unitypackage");

            EditorUtility.DisplayProgressBar("Exporting Terrain", "Deleting temporary files...", numOfSteps-1 / numOfSteps);
            foreach (string path in assetsToPackage)
            {
                AssetDatabase.DeleteAsset(path);
            }

            EditorUtility.DisplayProgressBar("Exporting Terrain", "Finalizing...", numOfSteps / numOfSteps);
            EditorSceneManager.CloseScene(exportScene, true);
            Application.OpenURL("file:///" + Application.dataPath + m_TerrainPackagesDirectory.Substring(6));
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();
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
            return exportSceneName.Replace(' ', '_');
        }

        private static void CopyTerrains(Scene exportScene, bool exportWithTextures, bool exportWithDetails, bool exportWithTrees)
        {
            EditorUtility.DisplayProgressBar("Exporting Terrain", "Copying Terrain(s)...", 3 / numOfSteps);
            Terrain[] terrains = Terrain.activeTerrains;
            for (int i = 0; i < terrains.Length; i++)
            {
                // Copy the Terrain Game Object
                Terrain newTerrain = CopyToScene(terrains[i].gameObject, exportScene).GetComponent<Terrain>();

                // Copy the Terrain Data
                TerrainData data = terrains[i].terrainData;
                string originalDataPath = AssetDatabase.GetAssetPath(data);
                string exportDataPath = m_TerrainDataDirectory + "/" + exportScene.name + "_" + i + "(clone).asset";
                AssetDatabase.CopyAsset(originalDataPath, exportDataPath);
                TerrainData newData = AssetDatabase.LoadAssetAtPath<TerrainData>(exportDataPath);
                assetsToPackage.Add(exportDataPath);

                // Copy or Strip Textures
                if (exportWithTextures)
                {
                    EditorUtility.DisplayProgressBar("Exporting Terrain", "Copying Textures...", 4 / numOfSteps);
                    TerrainLayer[] newLayers = new TerrainLayer[newData.terrainLayers.Length];

                    for (int l = 0; l < newData.terrainLayers.Length; l++)
                    {
                        TerrainLayer originalLayer = newData.terrainLayers[l];
                        string originalLayerPath = AssetDatabase.GetAssetPath(originalLayer);
                        string exportLayerPath = m_TerrainLayersDirectory + "/" + exportScene.name + "_layer_" + l + "(clone).asset";
                        AssetDatabase.CopyAsset(originalLayerPath, exportLayerPath);
                        newLayers[l] = AssetDatabase.LoadAssetAtPath<TerrainLayer>(exportLayerPath);
                        assetsToPackage.Add(exportLayerPath);
                    }

                    newData.terrainLayers = newLayers;
                }
                else
                {
                    EditorUtility.DisplayProgressBar("Exporting Terrain", "Stripping Textures...", 4 / numOfSteps);
                    newData.terrainLayers = null;
                }

                // Strip Details?
                if (!exportWithDetails)
                {
                    EditorUtility.DisplayProgressBar("Exporting Terrain", "Copying Details...", 5 / numOfSteps);
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
                } else
                {
                    EditorUtility.DisplayProgressBar("Exporting Terrain", "Stripping Details...", 5 / numOfSteps);
                }

                // Strip Trees?
                if (!exportWithTrees)
                {
                    EditorUtility.DisplayProgressBar("Exporting Terrain", "Copying Trees...", 6 / numOfSteps);
                    newData.treeInstances = new TreeInstance[0];
                    newData.treePrototypes = new TreePrototype[0];
                } else
                {
                    EditorUtility.DisplayProgressBar("Exporting Terrain", "Stripping Trees...", 6 / numOfSteps);
                }

                EditorUtility.DisplayProgressBar("Exporting Terrain", "Setting terrain data...", 7 / numOfSteps);
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

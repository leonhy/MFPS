using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEditor;

[InitializeOnLoad]
public class PunStartup : MonoBehaviour
{
    private static bool runOnce;

    // paths to demo scenes, to be included in a build. relative to "Assets/Photon Unity Networking/"
    private static string[] demoPaths = { "DemoHub/DemoHub-Scene.unity", "DemoBoxes/DemoBoxes-Scene.unity", "DemoWorker/DemoWorker-Scene.unity", "DemoWorker/DemoWorkerGame-Scene.unity", "MarcoPolo-Tutorial/MarcoPolo-Scene.unity", "DemoSynchronization/DemoSynchronization-Scene.unity", "DemoFriendsAndCustomAuth/DemoFriends-Scene.unity" };
	private const string demoBasePath = "Assets/Photon Unity Networking/Demos/";
	
    static PunStartup()
    {
        EditorApplication.update += OnLevelWasLoaded;
        runOnce = true;
    }

    static void OnLevelWasLoaded()
    {
        if (!runOnce) 
        {
            return;
        }

        runOnce = false;
        EditorApplication.update -= OnLevelWasLoaded;

        if (string.IsNullOrEmpty(EditorApplication.currentScene))
        {
            bool ret = EditorApplication.OpenScene(demoBasePath + demoPaths[0]);
            if (ret)
            {
                Debug.Log("No scene was open. Loaded PUN Demo Hub. Delete script 'PunStartup' to avoid this step.");
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(demoBasePath + demoPaths[0]);
            }
        }

        if (EditorBuildSettings.scenes.Length == 0)
        {
            SetPunDemoBuildSettings();

            Debug.Log("Applied new scenes to build settings. Delete script 'PunStartup' to avoid this step.");
        }
	}

    /// <summary>
    /// Finds scenes in "Assets/Photon Unity Networking/Demos/", excludes those in folder "PUNGuide_M2H" and applies remaining scenes to build settings. The one with "Hub" in it first.
    /// </summary>
    static public void SetPunDemoBuildSettings()
    {
        // find path of pun guide
        string[] tempPaths = Directory.GetDirectories(Application.dataPath + "/Photon Unity Networking", "Demos", SearchOption.AllDirectories);
        if (tempPaths == null || tempPaths.Length != 1)
        {
            return;
        }

        // find scenes of guide
        string guidePath = tempPaths[0];
        tempPaths = Directory.GetFiles(guidePath, "*.unity", SearchOption.AllDirectories);

        if (tempPaths == null || tempPaths.Length == 0)
        {
            return;
        }

        // add found guide scenes to build settings
        List<EditorBuildSettingsScene> sceneAr = new List<EditorBuildSettingsScene>();
        for (int i = 0; i < tempPaths.Length; i++)
        {
            //Debug.Log(tempPaths[i]);
            string path = tempPaths[i].Substring(Application.dataPath.Length - "Assets".Length);
            path = path.Replace('\\', '/');
            //Debug.Log(path);

            if (path.Contains("PUNGuide_M2H"))
            {
                continue;
            }

            if (path.Contains("Hub"))
            {
                sceneAr.Insert(0, new EditorBuildSettingsScene(path, true));
                continue;
            }

            sceneAr.Add(new EditorBuildSettingsScene(path, true));
        }
        
        EditorBuildSettings.scenes = sceneAr.ToArray();
        EditorApplication.OpenScene(sceneAr[0].path);
    }

}
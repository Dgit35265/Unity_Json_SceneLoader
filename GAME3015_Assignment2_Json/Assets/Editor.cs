using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;

public class Editor : EditorWindow
{
    [MenuItem("SceneLoader/Load Scene")]
    static void LoadScene()
    {
        string path = EditorUtility.OpenFilePanel("Load Scene", Application.dataPath + "/Scenes", "txt");
        if(path.Length != 0)
        {
            Debug.Log(path);
            //Parse Json File
            Players playersInfo = JsonUtility.FromJson<Players>(File.ReadAllText(path));
            CleanCurrentScene();
            CreateObjects(playersInfo);
        }
    }
    [MenuItem("SceneLoader/Save Scene")]
    static void SaveScene()
    {
        GameObject[] GOs = FindObjectsOfType<GameObject>();
        Players savePlayersInfo = new Players();
        foreach(GameObject go in GOs)
        {
            Player p = new Player();
            p.name = go.name;
            if(go.GetComponent<Renderer>())
            {
                p.color = go.GetComponent<Renderer>().sharedMaterial.color;
            }
            p.position = go.transform.position;
            p.rotation = go.transform.rotation.eulerAngles;
            savePlayersInfo.players.Add(p);
        }
        //Write Json File
        string saveScene = JsonUtility.ToJson(savePlayersInfo);
        string path = EditorUtility.SaveFilePanel("Save Scene", Application.dataPath + "/Scenes", "SavedScene", "txt");
        if(path.Length != 0)
            File.WriteAllText(path, saveScene);
    }
    static void CreateObjects(Players playersinfo)
    {
        foreach (var it in playersinfo.players)
        {
            if (it.name == "Main Camera" || it.name == "Directional Light")
                continue;
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Cube);
            p.tag = "Player";
            p.name = it.name;
            p.transform.position = it.position;
            p.transform.rotation = Quaternion.Euler(it.rotation);
            p.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            p.GetComponent<Renderer>().sharedMaterial.color = it.color;
        }
    }
    static void CleanCurrentScene()
    {
        //Clean all previous player objects 
        GameObject[] GOs = FindObjectsOfType<GameObject>();
        foreach(var it in GOs)
        {
            if (it.tag == "Player")
                DestroyImmediate(it);
        }
    }
}

//Json Structure
[Serializable]
public class Players
{
    public List<Player> players;
    
    public Players()
    {
        players = new List<Player>();
    }
}

[Serializable]
public class Player
{
    public string name;
    public Color color;
    public Vector3 position;
    public Vector3 rotation;
}




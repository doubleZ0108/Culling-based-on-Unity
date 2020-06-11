using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateVertsAndTris : MonoBehaviour
{
    public float f_UpdateInterval = 0.5F;  //刷新间隔
    private float f_LastInterval;      //上一次刷新的时间间隔

    public static int verts;
    public static int tris;

    // Use this for initialization
    void Start()
    {
        f_LastInterval = Time.realtimeSinceStartup;
    }

    void GetAllObjects()
    {
        verts = 0;
        tris = 0;
        GameObject[] ob = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject obj in ob)
        {
            GetAllVertsAndTris(obj);
        }
    }

    //得到三角面和顶点数
    void GetAllVertsAndTris(GameObject obj)
    {
        Component[] filters;
        filters = obj.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter f in filters)
        {
            tris += f.sharedMesh.triangles.Length / 3;
            verts += f.sharedMesh.vertexCount;
        }
    }

    void OnGUI()
    {
        Rect area = new Rect(10.0f, 10.0f, 300.0f, 300.0f);
        GUILayout.BeginArea(area);


        GUILayout.BeginVertical();

		GUI.skin.label.fontSize = 40;
        GUILayout.Label("tris:" + tris);
        GUILayout.Label("verts: " + verts);
        GUILayout.Label("FPS: " + Mathf.RoundToInt(1.0f / Time.deltaTime).ToString());

		GUILayout.EndVertical();

        GUILayout.EndArea();
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.realtimeSinceStartup > f_LastInterval + f_UpdateInterval)
        {
            f_LastInterval = Time.realtimeSinceStartup;
            GetAllObjects();
        }
    }
}

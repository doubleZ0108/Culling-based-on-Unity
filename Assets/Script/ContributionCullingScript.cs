using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContributionCullingScript : MonoBehaviour
{
    // Use this for initialization
    public Transform sphere;
    void Start()
    {
        //定义大小为32的一维数组，用来存储所有层的剔除信息
        float[] distances = new float[32];
        //设置第9层的剔除距离
        distances[9] = Vector3.Distance(transform.position, sphere.position);
        //将数组赋给摄像机的layerCullDistances
        Camera.main.layerCullDistances = distances;
    }

    // Update is called once per frame
    void Update()
    {
        //摄像机远离物体
        transform.Translate(-transform.forward * Time.deltaTime);
    }
}

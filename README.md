#  基于Unity的Culling算法性能分析与评估

<div align="center">1754060</div>

<div align="center">张喆</div>

<div align="center">指导老师：张乾老师 贾金原老师</div>



Table of Contents
=================

   * [基于Unity的Culling算法性能分析与评估](#基于unity的culling算法性能分析与评估)
      * [作业要求](#作业要求)
      * [准备工作](#准备工作)
         * [复杂度衡量标准](#复杂度衡量标准)
         * [性能衡量标准](#性能衡量标准)
            * [性能分析工具](#性能分析工具)
         * [构建场景](#构建场景)
            * [高复杂度场景](#高复杂度场景)
            * [中复杂度场景](#中复杂度场景)
            * [低复杂度场景](#低复杂度场景)
         * [场景漫游](#场景漫游)
      * [Backface culling](#backface-culling)
         * [高复杂度场景](#高复杂度场景-1)
         * [中复杂度场景](#中复杂度场景-1)
         * [低复杂度场景](#低复杂度场景-1)
      * [Occlusion culling](#occlusion-culling)
         * [高复杂度场景](#高复杂度场景-2)
         * [中复杂度场景](#中复杂度场景-2)
         * [低复杂度场景](#低复杂度场景-2)
      * [Contribution culling(Small Feature Culling)](#contribution-cullingsmall-feature-culling)
         * [高复杂度场景](#高复杂度场景-3)
         * [中复杂度场景](#中复杂度场景-3)
         * [低复杂度场景](#低复杂度场景-3)

------

## 作业要求

基于Unity来进行主要Cullig算法性能分析与评估

1. 构建不同复杂度的三维场景(高、中、低)
2. 测试Backface culling, Occlusion Culling, Small Feature Culling对于性能的影响
   - 其中:Backface culling: 引擎直接支持
   - Occlusion culling: 引擎进行0c烘焙后支持
   - Contribution culling: 独立实现



## 准备工作

### 复杂度衡量标准

基于Unity引擎构建高、中、低复杂度的场景，其中用<u>三角面数</u>和<u>顶点数</u>作为衡量场景复杂度的主要依据

在unity中使用如下脚本进行三角面和顶点数的计算，并通过Unity UI两种复杂度衡量标准实时显示该数据

```c#
Component[] filters;
filters = obj.GetComponentsInChildren<MeshFilter>();
foreach (MeshFilter f in filters)
{
  tris += f.sharedMesh.triangles.Length / 3;
  verts += f.sharedMesh.vertexCount;
}
```



### 性能衡量标准

**帧率 FPS**：反应渲染一帧所消耗的时间，GPU处理时每秒能更新的次数。不仅帧率反应性能，帧率的稳定性也十分重要

#### 性能分析工具

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3srkgp1j30wj0u0k4g.jpg" alt="截屏2020-06-13 15.32.03" width="70%;" />

使用Unity Profiler进行分析，主要包括如下方面

- CPU

  - Total: 当前任务的时间消耗占当前帧CPU消耗时间的比例
  - Self: 任务自身时间消耗占当前CPU消耗的时间比例
  - Calls: 当前任务在当前帧内被调用的次数
  - GC Alloc: 当前任务在当前帧进行过内存回收和分配的次数
  - Time ms: 当前任务在当前帧内的耗时总时间
  - Self ms: 当前任务自身时间消耗

  <img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3sxhu01j313k0ggmzt.jpg" alt="image-20200613154026033" width="70%;" />

- GPU

- 渲染

- 内存 

- ...

项目运行时，每个profilers会随着运行时间显示数据，有些性能问题是持续性的，有些仅出现在某一帧，还有些性能问题可能会随着时间推移而逐渐显现出来



### 构建场景

根据复杂度衡量标准构建高、中、低三种不同复杂度的三维场景（受限于笔记本性能原因，三个三维场景如下）

#### 高复杂度场景

通过城市场景构建高复杂度场景，场景中主要由大量高精度建筑物等构成

- 三角面数：150w
- 顶点数：100w

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3t0f1jnj31ak0qwqv5.jpg" alt="image-20200611124323839" width="90%;" />

#### 中复杂度场景

通过地形 + 高精度树进行中复杂度场景构建，其中地形并不参与unity中gameobject的三角面计算，可以通过树的数量动态的进行复杂度的调整

- 三角面数：10w
- 顶点数：10w

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3t4g6h8j31ao0qq7wi.jpg" alt="image-20200611125520318" width="90%;" />

#### 低复杂度场景

通过low poly形式的卡通风格场景进行模拟，由于场景中的gameobject大都采用low poly方式构建，因此场景的总顶点数是三角面片的两倍

- 三角面数：2w
- 顶点数：4w

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3t7syr8j31ao0pa7oz.jpg" alt="image-20200611134105442" width="90%;" />



### 场景漫游

对camera添加脚本进行场景漫游，在后续的Culling的操作中通过场景漫游的方式进行动态观察

```c#
public float moving_factor = 100.0f;
public float rotate_factor = 15.0f;

void Update()
{
  if (Input.GetKey(KeyCode.W))
  {
    this.transform.Translate(0.0f, 0.0f, Time.deltaTime * 1.0f * moving_factor, Space.Self);
  }
  if (Input.GetKey(KeyCode.D))
  {
    transform.Rotate(0.0f, Time.deltaTime * 1.0f * rotate_factor, 0.0f);
  }
	// ...
}
```



------

## Backface culling

移除没有面对这镜头的三角形，**只考虑三角形与摄像机的相对位置而不依赖与摄像机朝向**。依靠三角形顶点顺序直接判断法向量方向（左手法则）。只需要将物体的normal与相机的朝向进行一个计算，即可以得到是否为backface，也就是是否需要裁剪

在unity中默认就是背面剔除，例如一个平面，添加了标准材质后他只有正面是可以看到的，背面是观察不到的；对于立方体来说，立方体的每个面都是有正面和背面的，而背面是无法观察到的。

Culling操作是位于渲染管线中的光栅化阶段，在顶点着色器之后，片元着色器之前，这样是为了提高渲染性能，避免做一些无用片元的渲染计算。例如一个立方体，在unity中最多只能看到三个面。开启了背面裁剪后其他面在背对摄像机时它对于摄像机来说就是一个背面，导致其他三个面被裁减掉。

对于一个物体而言，背面剔除会剔除掉几乎所有不需要渲染的面片，而不需要用到深度测试；深度测试多用于物体之间，保证各个物体间正确的排列顺序，而背面剔除用于剔除自身背面。

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3tbystcj311a0kytgn.jpg" alt="image-20200613210530968" width="50%;" />

由于unity默认即为Backface culling，这里做一个简单的Frontface culling作为学习演示

```shder
Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (34,45,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Cull Front
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

```

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3tf7xryj31ak0qwn3p.jpg" alt="image-20200613162726750" width=" 50%;" />



### 高复杂度场景

可以看到由于场景非常庞大，大多数时间都是60FPS一下，甚至每隔一段时间会掉到30FPS之下

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3ti6ygsj30xb0u0kfj.jpg" alt="image-20200613214005010" width="70%;" />

### 中复杂度场景

有近一半的时间维持在60FPS之下，甚至有部分时间持续维持在很低的FPS水平

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3tlsu0ej30xe0u07wh.jpg" alt="image-20200613214622532" width="70%;" />

### 低复杂度场景

由于CPU中VSync和Physics的同时消耗，导致低复杂度场景的FPS也维持在较低水平，大约一半的情况智能达到30FPS，有时更差

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3tp8jrmj30vb0u0k8f.jpg" alt="image-20200613182715442" width=" 70%;" />



------

## Occlusion culling

当一个物体被其他物体遮挡而不在摄像机的可视范围内时对其进行渲染

1. 设置遮挡静态 Occluder Static / Occludee Static，并且将子物体同样设置为遮挡静态

   <img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3trnp6dj30ls0bejxk.jpg" alt="image-20200613182942375" width="50%;" />

2. 烘焙遮挡提出 Window -> Rendering -> Occlusion Culling

   - Smallest Occluder: 最小遮挡物的尺寸，当遮挡物的长度或宽度大于设定值时，该物体才能够遮挡住后面的物体
   - Smallest Hole: 最小孔的尺寸，当穿过物体内部的孔或者多个物体堆叠形成的孔大于设定的值时，遮挡剔除烘焙将忽略孔的存在
   - Baceface Threshold: 设置背面已处于之，用于优化场景

   <img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3u53q0qj30mg0n4x2e.jpg" alt="image-20200613183133083" width=" 33%;" /><img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3tw1nhxj30ly19ymzg.jpg" alt="image-20200613183159465" width=" 33%;" />

3. 选择遮挡剔除窗口的Visualizator，就可以观察到效果了

   <img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3ua8eisj30ma0qyhap.jpg" alt="image-20200613183529745" width="50%;" /><img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3ugr02cj310u0qkb29.jpg" alt="image-20200613183748721" width=" 70%;" /><img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3ul3iafj31240qoe81.jpg" alt="image-20200613183853174" width="70%;" />



> 由于并非在同一时间进行测算，电脑当时的负载可能不尽相同，因此数据只具有相对意义

### 高复杂度场景

相较于只有Backface culling的情况，大都数时间都能维持在60FPS之上，只有少部分在30～60之间，极少数位于30FPS之下

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3uoeygvj31de0sckhb.jpg" alt="截屏2020-06-13 21.29.36" width="70%;" />

### 中复杂度场景

相较于只有Backface culling的情况，可以看到60FPS以上的比重很大，这说明culling对性能的提升比较明显

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3usndutj31aq0r8x6p.jpg" alt="image-20200613200756528" width="70%;" />

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3uxpiquj30vj0u07wh.jpg" alt="image-20200613214432328" width="70%;" />

### 低复杂度场景

相较于只有Backface culling的情况，有更少的时候处于60FPS之下，更少的处于30FPS之下

> 这里有部分电脑当前CPU利用率影响的问题，无法做到绝对的控制变量

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3v1vr50j31ag0qwu06.jpg" alt="image-20200613201019551" width="70%;" />

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3v59nyaj30ut0u01bh.jpg" alt="image-20200613201046010" width="70%;" />

------

## Contribution culling(Small Feature Culling)

非常小的物体并且距离相机非常远，在近平面进行投影，由于它的体积非常小，投影之后的像素点也是非常小的，如果对画面不是非常精确，可以忽略掉这个点。

- if small details contribute little or nothing to the scene then it can be discarded

- when the viewer is in motion details can be culled, when the viewer stops the detail culling is usally disabled

- consider an object with a bounding volumn(BV), it is projected onto the projection plane

- The area of the projection is then estimated in pixels nPixels

  if nPixels < threshold value then don't draw the object



1. 新建smallfeature剔除层

   <img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3vb7kjwj315a0r6gpx.jpg" alt="image-20200613215723776" width="50%;" />

2. 设置需要剔除的小物体的Layer为刚刚设置的剔除层

   <img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3vf07kcj30lu0dw799.jpg" alt="image-20200613220736867" width="50%;" />

3. 控制小物体隐藏的脚本

4. 运行移动摄像机可以看到，在场景中确实存在该gameobject，但是由于超出设定的范围，并没有进行渲染，达到small feature culling 的效果

   <img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3vhht2wj312c0j8gp5.jpg" alt="image-20200613221008359" width="50%;" />

### 高复杂度场景

可以看到main包含绝大多数的高楼（主要的feature）

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3vnl88wj30te0r8qmj.jpg" alt="image-20200613222856480" width="70%;" />

而node节点包含的主要为smallfeature，例如道路上的石头等

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3vortczj30v60qgtt1.jpg" alt="image-20200613222940705" width="70%;" />

可以看到由于该场景复杂度之大，去掉一些smallfeature对性能的影响非常大，加入contribution culling之后多数时间可以达到80FPS，可以说性能对于城市这种大场景比较不错

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3vq212ij311u0u0ked.jpg" alt="image-20200613223506903" width="50%;" />

### 中复杂度场景

将所有树作为smallfeature，与之对应的是terrain，当摄像机逐渐走远可以看到响应的smallfeature也不再渲染。也可以看到后期性能有比较明显的提升，基本一致保持在60FPS之上。

> 这里特意将树设置的比较大时就认定为smallfeature方便测试

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3vw95wgj30w00u0b2a.jpg" alt="image-20200613222551812" width="70%;" />

### 低复杂度场景

将围栏、石头、草等归为small feature并设置平面

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3vzya63j30ec0b2wg6.jpg" alt="image-20200613222101729" width="33%;" />

可以看到性能有显著提升，由于该场景中gameobject本来就很少，而且复杂度较低，去掉small feature之后性能损耗更低

<img src="https://tva1.sinaimg.cn/large/007S8ZIlly1gfr3w1xdcnj311e0u07hi.jpg" alt="image-20200613230449561" width="70%;" />

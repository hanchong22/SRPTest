using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

[ExecuteInEditMode]
public class MyScriptableRenderPipeline : RenderPipelineAsset
{
    public Color clearColor = Color.blue;
    protected override RenderPipeline CreatePipeline()
    {
        var pipeline = new  MyScriptableRenderPipelineInstance();
        pipeline.ClearColor = clearColor;
        return pipeline;
    }
}


public class MyScriptableRenderPipelineInstance : RenderPipeline
{ 
    private CullingResults cull;
    private ScriptableCullingParameters cullingParams;
    private CommandBuffer cmd;   

    private ShaderTagId zPrepass = new ShaderTagId("ZPrepass");
    private ShaderTagId basicPass = new ShaderTagId("BasicPass");

    public static MyScriptableRenderPipelineInstance Instance { get; private set; }

    public Color ClearColor { get; set; }


    public MyScriptableRenderPipelineInstance()
    {
        Instance = this;
    }


    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {        
        //base.Render(context, cameras);
        if (cmd == null)
        {
            cmd = new CommandBuffer();
        }
        //int idx = 0;
        //foreach (var camera in cameras)
        for(int i = 0; i < cameras.Length; ++i)
        {
            var camera = cameras[i];

            //�޳���Щ���������׶֮�������
            if (!camera.TryGetCullingParameters(out cullingParams)) continue;

            //����Cull �����������޳�
            cull = context.Cull(ref cullingParams);

            //����camera��һЩview, projection and clipping planes����Ϣ
            context.SetupCameraProperties(camera);

#if UNITY_EDITOR
            //UI ��ʱunityΪ������renderʱ���ӵ�һ��overlay �������ڱ༭��ʱ��Ҫ����һ�����޳�
            if (camera.cameraType == CameraType.SceneView)
                ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
#endif

            var clearFlags = camera.clearFlags;
            //�����ɫ�������Ȼ���
            cmd.ClearRenderTarget(
                (clearFlags & CameraClearFlags.Depth) != 0,
                (clearFlags & CameraClearFlags.Color) != 0,
                ClearColor);
            //��ʼ����
            // cmd.BeginSample("My Pipeline Camera");
            UnityEngine.Profiling.Profiler.BeginSample("My Pipeline Camera");            
            context.ExecuteCommandBuffer(cmd);
            //�����һ֡
            cmd.Clear();           
            
            context.DrawSkybox(camera);
           
            // Directional Light
            SetUpDirectionalLightParam(cull.visibleLights.ToList());

            SortingSettings sortSets = new SortingSettings(camera);
            sortSets.criteria = SortingCriteria.RenderQueue;           
            DrawCharacter(context, camera, zPrepass, sortSets);

            SortingSettings sortSets4 = new SortingSettings(camera);
            sortSets4.criteria = SortingCriteria.CommonOpaque;           
            DrawBg(context, camera, sortSets4);

            

            SortingSettings sortSets2 = new SortingSettings(camera);
            sortSets2.criteria = SortingCriteria.OptimizeStateChanges;           
            DrawCharacter(context, camera, basicPass, sortSets2);


            SortingSettings sortSets3 = new SortingSettings(camera);
            sortSets3.criteria = SortingCriteria.CommonTransparent;            
            DrawShadow(context, camera, sortSets3);
            
            context.Submit();

            UnityEngine.Profiling.Profiler.EndSample();
        }
    }    


    private void DrawCharacter(ScriptableRenderContext context, Camera camera, ShaderTagId pass,SortingSettings sortFlags)
    {       
        var settings = new DrawingSettings(pass, sortFlags);
        settings.enableInstancing = true;
        settings.enableDynamicBatching = true;

        var filterSettings = new FilteringSettings(RenderQueueRange.transparent, 1 << LayerDefine.CHARA, 1 << LayerDefine.CHARA);
        context.DrawRenderers(cull, ref settings,ref filterSettings);       
    }
  
    private void DrawBg(ScriptableRenderContext context, Camera camera, SortingSettings sortFlags)
    {
        var settings = new DrawingSettings(basicPass, sortFlags);

        var filterSettings = new FilteringSettings(RenderQueueRange.opaque, 1 << LayerDefine.BG, 1 << LayerDefine.BG);
        
        context.DrawRenderers(cull, ref settings, ref filterSettings);
    }
   
    private void DrawShadow(ScriptableRenderContext context, Camera camera, SortingSettings sortFlags)
    {
        var settings = new DrawingSettings(basicPass, sortFlags);
        settings.enableInstancing = true;
        settings.enableDynamicBatching = true;
        var filterSettings = new FilteringSettings(RenderQueueRange.transparent, 1 << LayerDefine.SHADOW, 1 << LayerDefine.SHADOW);       
        context.DrawRenderers(cull, ref settings,ref filterSettings);
    }

   
    private void SetUpDirectionalLightParam(List<VisibleLight> visibleLights)
    {
        if( visibleLights.Count <= 0 ){
            return;
        }
        foreach( var visibleLight in visibleLights)
        {
            if (visibleLight.lightType == LightType.Directional)
            {
                Vector4 dir = -visibleLight.localToWorldMatrix.GetColumn(2) ;
                Shader.SetGlobalVector(ShaderNameHash.LightColor0, visibleLight.finalColor);
                Shader.SetGlobalVector(ShaderNameHash.WorldSpaceLightPos0, new Vector4(dir.x,dir.y,dir.z,0.0f) );
                break;
            }
        }
    }
   
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Instance = null;
        if (cmd != null)
        {
            cmd.Dispose();
            cmd = null;
        }
    }
}

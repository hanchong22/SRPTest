using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;


// 管理所有角色
public class CharaManager : MonoBehaviour
{
    public GameObject prefab;   
    public AppAnimationInfo animationInfo;
   
    public Material drawMaterial;
   
    public int characterNum = 2000;
  
    private const float InitPosXParam = 15.0f;
    private const float InitPosZParam = 15.0f;
 
    private Transform[] characterTransforms;
  
    private BoardRenderer[] boardRenderers;

    //Matrix4x4[] matrix;

   
    void Start()
    {
      
        animationInfo.Initialize();
      
        boardRenderers = new BoardRenderer[characterNum];
        characterTransforms = new Transform[characterNum];

        //matrix = new Matrix4x4[characterNum];

        var material = new Material(drawMaterial);
        material.mainTexture = animationInfo.texture;
        for (int i = 0; i < characterNum; ++i)
        {
            var gmo = GameObject.Instantiate(prefab, new Vector3(Random.Range(-InitPosXParam, InitPosXParam), 0.5f, Random.Range(-InitPosZParam, InitPosZParam)), Quaternion.identity);
           
            characterTransforms[i] = gmo.transform;
            boardRenderers[i] = gmo.GetComponent<BoardRenderer>();
            boardRenderers[i].SetMaterial(material );
            int idx = i % animationInfo.sprites.Length;
            boardRenderers[i].SetRect( animationInfo.GetUvRect( 0 ) );
            boardRenderers[i].SetColor(ShaderNameHash.ColorList[i % ShaderNameHash.ColorList.Length]);
        }
    }

    
    void Update()
    {      
        int animationLength = animationInfo.animationLength;
        for (int i = 0; i < characterNum; ++i)
        {
            int rectIndex = ((int)(i * 0.3f + Time.realtimeSinceStartup * 10.0f)) % animationLength;            
            boardRenderers[i].SetRect(animationInfo.GetUvRect(rectIndex));
            //matrix[i] = Matrix4x4.TRS(characterTransforms[i].position, characterTransforms[i].rotation, characterTransforms[i].lossyScale);
        }
    }

}

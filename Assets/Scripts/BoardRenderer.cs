using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class BoardRenderer : MonoBehaviour {

    private Renderer rendererCache;
    public Rect rect = new Rect(0, 0, 1, 1);
    private MaterialPropertyBlock prop;

    void Awake() {
        rendererCache = this.GetComponent<Renderer>();
        prop = new MaterialPropertyBlock();
    }

    public void SetMaterial(Material material)
    {
        rendererCache.material = material;
    }

    public void SetColor(Color c)
    {
        prop.SetColor(ShaderNameHash.TintColor, c);
        rendererCache.SetPropertyBlock(prop);
    }

    public void SetRect(Rect r)
    {
        Vector4 val = new Vector4( r.x,r.y,r.width,r.height);
        prop.SetVector( ShaderNameHash.RectValue, val);
        rendererCache.SetPropertyBlock(prop);
        this.rect = r;
    }
}

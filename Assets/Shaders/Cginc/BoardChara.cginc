
v2f vert (appdata v)
{
	v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    UNITY_TRANSFER_INSTANCE_ID(v,   o);      

	float4 rect = UNITY_ACCESS_INSTANCED_PROP(Props,_RectValue);
	v.vertex.x *= rect.z /_ExpectedRect.z;
	v.vertex.y *= rect.w / _ExpectedRect.w;
	o.vertex = UnityObjectToClipPos(v.vertex);

	v.uv.x = (v.uv.x * rect.z) + rect.x;
	v.uv.y = (v.uv.y * rect.w) + rect.y;

	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	return o;
}
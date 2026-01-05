float4x4 worldViewProjection;

sampler2D uImage0 : register(s0);

float4 color;
float radius;
float scale;

float2 ScaleAroundCenter(float2 uv, float scale)
{
    float2 center = float2(0.5f, 0.5f);
    uv -= center;
    uv *= scale;
    uv += center;
    return uv;
}

float4 PSMain(float2 textureCoordinates : TEXCOORD0, float4 interpolatedColor : COLOR0, float2 position : VPOS) : COLOR0
{
    float2 scaledUV = ScaleAroundCenter(textureCoordinates, scale);

    float4 newColor = interpolatedColor * color;
    newColor.a = 255;

    return tex2D(uImage0, scaledUV) * newColor;
}

float4 VSMain(inout float2 textureCoordinates : TEXCOORD0, inout float4 interpolatedColor : COLOR0, float3 position : SV_Position) : SV_Position
{
    return mul(float4(position, 1), worldViewProjection);
}

technique
{
    pass
    {
        PixelShader = compile ps_3_0 PSMain();
        VertexShader = compile vs_3_0 VSMain();
    }
}
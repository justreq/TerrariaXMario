
sampler2D uImage0 : register(s0);

// 0 -----------------------------------------> 1
// none ---> outerThreshold ---> innerThreshold 
// 
// if the 
float uOuterThreshold;
float uInnerThreshold;
float4 uInnerColor;
float4 uOuterColor;

void MainPS(inout float4 color : COLOR0, in float2 texCoords : TEXCOORD0)
{
    float4 sampledColor = tex2D(uImage0, texCoords);
    color *= sampledColor;
    float colorAvg = sampledColor.a; //(color.r + color.g + color.b + color.a) / 4 ;
    if (colorAvg > uInnerThreshold)
    {
        color = uInnerColor;
    }
    else if (colorAvg > uOuterThreshold)
    {
        color = uOuterColor;
    }
    else
    {
        color = float4(0, 0, 0, 0);
    }
}

technique t0
{
    pass p0
    {
        PixelShader = compile ps_2_0 MainPS();
    }
}
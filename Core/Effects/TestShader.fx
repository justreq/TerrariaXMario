sampler2D image : register(s0);

float4 color;

float4 PSMain(float2 textureCoordinates : TEXCOORD0, float4 interpolatedColor : COLOR0, float2 position : VPOS) : COLOR0
{
    float x = textureCoordinates.x;
    float y = textureCoordinates.y * 2 % 1;
    float4 newColor = color;
    
    if (textureCoordinates.y > 0.5f)
    {
        x = 1 - x;
        y = 1 - y;
        newColor.rgb *= 0.5f;
    }
    
    float percentScreenPosition = (position.x + 1) / 2;
    
    newColor.r *= percentScreenPosition / 255;
    newColor.b *= 255 - percentScreenPosition / 255;
        
    return tex2D(image, float2(x, y)) * interpolatedColor * newColor;
}

technique
{
    pass
    {
        PixelShader = compile ps_3_0 PSMain();
    }
}
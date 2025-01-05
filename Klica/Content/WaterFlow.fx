sampler TextureSampler : register(s0);

float2 Time; // Time passed, passed from the game
float DistortionStrength = 0.1; // Increase strength for more visibility
float Frequency = 20.0; // Adjust frequency for faster/more visible waves

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR
{
    // Generate distortion
    float2 distortion = float2(
        sin(texCoord.y * Frequency + Time.x) * DistortionStrength,
        cos(texCoord.x * Frequency + Time.y) * DistortionStrength
    );

    // Apply distortion
    float2 distortedTexCoord = texCoord + distortion;

    // Sample texture
    return tex2D(TextureSampler, distortedTexCoord);
}

technique WaterFlow
{
    pass P0
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

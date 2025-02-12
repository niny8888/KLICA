sampler TextureSampler : register(s0);

float seed = 714.434;
float iTime;
float lineValueLimit = 0.005; //debeline crte
float line2ValueLimit = 0.05; ///to lahko za seeno crto da se prekrivajo barve - hotla sm da bi biu en z visjim opacityjem in pol en z mn

//barva
float3 lineColor = float3(1.0, 1.0, 1.0); // White lines
float3 line2Color = float3(0.0, 1.0, 1.0); // Cyan lines npr
float lineAlpha = 0.2; // 20% opacity

float2 distortUv(float2 uv) { // rdi casovno odvisne vjugice
    float x = uv.x * 10.0 + iTime;
    float y = uv.y * 10.0 + iTime;
    uv.x += sin(x - y) * 0.01 * cos(y);
    uv.y += cos(x + y) * 0.01 * sin(y);
    return uv;
}

float2 randomGradient(float2 corner) { // to nrdi psevdo-random smerni vektor na vsaki tocki v gridu
    float x = dot(corner, float2(123.4, 567.8));
    float y = dot(corner, float2(321.321, 654.654));
    float2 gradient = float2(x, y);
    gradient = sin(gradient);
    gradient *= seed + iTime * 0.1;
    gradient = sin(gradient);
    return gradient;
}

float quintic(float p) { //to preventa ostre robove - smootha
    return p * p * p * (10.0 + p * (-15.0 + p * 6.0));
}

float perlinNoise(float2 uv) {
    float gridDivision = 3.0; // 3x3 grid per unit
    uv *= gridDivision;

    float2 gridId = floor(uv);
    float2 gridUv = frac(uv);

    float2 tl = gridId;
    float2 tr = gridId + float2(1.0, 0.0);
    float2 bl = gridId + float2(0.0, 1.0);
    float2 br = gridId + float2(1.0, 1.0);

    // random gradient vectors v usakmu kotu 
    float2 gradTl = randomGradient(tl);
    float2 gradTr = randomGradient(tr);
    float2 gradBl = randomGradient(bl);
    float2 gradBr = randomGradient(br);

    float2 fragToTl = gridUv;
    float2 fragToTr = gridUv - float2(1.0, 0.0);
    float2 fragToBl = gridUv - float2(0.0, 1.0);
    float2 fragToBr = gridUv - float2(1.0, 1.0);

    // dot product med temi vektorji
    float dotTl = dot(gradTl, fragToTl);
    float dotTr = dot(gradTr, fragToTr);
    float dotBl = dot(gradBl, fragToBl);
    float dotBr = dot(gradBr, fragToBr);

    gridUv = float2(quintic(gridUv.x), quintic(gridUv.y));

    //lerp <3
    float t = lerp(dotTl, dotTr, gridUv.x);
    float b = lerp(dotBl, dotBr, gridUv.x);
    return lerp(t, b, gridUv.y);
}

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR {
    texCoord = distortUv(texCoord);
    float noise = abs(perlinNoise(texCoord));

    //čist transparentno
    float3 color = float3(0.0, 0.0, 0.0);
    float alpha = 0.2;

    // bele crte with 20% opacity -- ne dela :C
    if (noise < lineValueLimit) {
        color = lineColor;
        alpha = lineAlpha;
    }

    return float4(color, alpha);
}

technique PerlinNoiseOverlay {
    pass P0 {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}



void GetMainLightData_float(out half3 Direction, out half3 Color, out half DistanceAttenuation, out half ShadowAttenuation)
{
    #ifdef SHADERGRAPH_PREVIEW
    // In Shader Graph Preview we will assume a default light direction and white color
    Direction = half3(-0.3, -0.8, 0.6);
    Color = half3(1, 1, 1);
    DistanceAttenuation = 1.0;
    ShadowAttenuation = 1.0;
    #else

    // Universal Render Pipeline
    #if defined(UNIVERSAL_LIGHTING_INCLUDED)
    
    // GetMainLight defined in Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl
    Light mainLight = GetMainLight();
    Direction = -mainLight.direction;
    Color = mainLight.color;
    DistanceAttenuation = mainLight.distanceAttenuation;
    ShadowAttenuation = mainLight.shadowAttenuation;
    
    #endif
    
    #endif
}

Shader "Custom/CrackShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Crack Mask", 2D) = "white" {}
        _DamageProgress ("Damage Progress", Range(0,1)) = 0
        _TimeSinceCrack ("Time Since Crack", Float) = 0
        _EmissionFadeInTime ("Emission Fade In Time", Float) = 1.5
        _ThickenAmount ("Thicken Amount", Range(0, 2.0)) = 0.5
        _EmissionColor ("Emission Color", Color) = (1, 0.3, 0, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 5)) = 2
        _PulseSpeed ("Pulse Speed", Float) = 3
        _RevealMode ("Reveal Mode", Float) = 0 // 0 = radial, 1 = gradient
        
        // Required for sprite rendering
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            
            half _DamageProgress;
            half _TimeSinceCrack;
            half _EmissionFadeInTime;
            half _ThickenAmount;
            half4 _EmissionColor;
            half _EmissionIntensity;
            half _PulseSpeed;
            half _RevealMode;
            half4 _RendererColor;
            
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _RendererColor;
                
                #ifdef PIXELSNAP_ON
                o.vertex = UnityPixelSnap(o.vertex);
                #endif
                
                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                // Sample the crack mask
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                
                // Crack mask is black (0) on transparent background
                // We want to work with the inverse for easier calculations
                half crackValue = 1.0 - texColor.r;
                
                // === FEATURE 1: THICKEN CRACK LINES ===
                // Use dilation effect to thicken the crack
                half thickened = crackValue;
                if (_ThickenAmount > 0.001)
                {
                    // Calculate sampling radius in UV space
                    // _ThickenAmount of 0.5 = 0.05 UV units, 1.0 = 0.10, 2.0 = 0.20
                    // This works regardless of texture size
                    // Increased multiplier for more visible thickening
                    float sampleRadius = _ThickenAmount * 0.10;
                    
                    half samples = crackValue;
                    
                    // Use a fixed 5x5 kernel for consistent results
                    // Sample in a grid pattern around the current pixel
                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            // Calculate offset in UV space
                            // Scale by sample radius to control thickness
                            float2 offset = float2(x, y) * sampleRadius;
                            
                            // Sample the texture at offset position
                            half4 samp = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + offset);
                            
                            // Invert to get crack value (black = crack = 1.0)
                            half sampCrackValue = 1.0 - samp.r;
                            
                            // Take maximum to dilate (expand) the crack
                            samples = max(samples, sampCrackValue);
                        }
                    }
                    thickened = samples;
                }
                
                // === FEATURE 2: PROGRESSIVE REVEAL ===
                // Smoothly reveal crack based on damage progress
                half revealMask = 0;
                
                if (_RevealMode < 0.5) // Radial reveal (default)
                {
                    float2 center = float2(0.5, 0.5);
                    float dist = distance(i.texcoord, center);
                    float maxDist = 0.707; // sqrt(0.5^2 + 0.5^2)
                    
                    // Smooth transition instead of hard step
                    float edgeSoftness = 0.1;
                    revealMask = smoothstep(
                        _DamageProgress * maxDist * 1.3 + edgeSoftness,
                        _DamageProgress * maxDist * 1.3 - edgeSoftness,
                        dist
                    );
                }
                else // Gradient reveal (bottom to top)
                {
                    float edgeSoftness = 0.05;
                    revealMask = smoothstep(
                        _DamageProgress - edgeSoftness,
                        _DamageProgress + edgeSoftness,
                        i.texcoord.y
                    );
                }
                
                // Apply reveal mask to thickened crack - only show what should be revealed
                half finalCrack = thickened * revealMask;
                
                // === FEATURE 3: EMISSION FADE-IN + PULSE ===
                // Emission fades in over time after crack appears, then pulses
                half fadeIn = saturate(_TimeSinceCrack / _EmissionFadeInTime); // 0 to 1 over fade time
                half pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5; // 0 to 1 oscillation
                
                // Combine fade-in with pulse, weighted by damage progress
                half emissionStrength = fadeIn * _DamageProgress * _EmissionIntensity * pulse;
                half emission = finalCrack * emissionStrength;
                
                // Start black (0), gradually add emission glow
                half3 crackColor = lerp(half3(0, 0, 0), _EmissionColor.rgb, emission);
                
                // Final output
                half4 output;
                output.rgb = crackColor;
                output.a = finalCrack * texColor.a; // Use original alpha for transparency
                
                return output * i.color;
            }
            ENDHLSL
        }
    }
    
    Fallback "Sprites/Default"
}

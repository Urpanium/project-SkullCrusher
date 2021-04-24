Shader "Skull Crusher/Post Processing/JPEG Artifacts"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        // TODO: hide in Inspector and pass render texture through script
        _Mask ("Mask Texture", 2D) = "white" {}

        _Artifact1 ("Artifacts Texture (R)", 2D) = "white" {}
        _Artifact2 ("Artifacts Texture (G)", 2D) = "white" {}
        _Artifact3 ("Artifacts Texture (B)", 2D) = "white" {}
        //_Artifact4 ("Artifacts Texture (A)", 2D) = "white" {}

        // степень сжатия по шкале ебучих шакалов
        _ArtifactIntensity ("Artifacts Intensity", Range(0, 1)) = 0.5
        _PixelizationIntensity ("Pixelization Intensity", Range(0,1)) = 0.2

        _Depth ("Blend Smoothness", Range(0.001, 1)) = 0.015
    }
    SubShader
    {
        // No culling or depth
        // TODO: consider using depth buffer to blend textures
        ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma exclude_renderers d3d11_9x
            #pragma exclude_renderers d3d9

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _Mask;

            sampler2D _Artifact1;
            sampler2D _Artifact2;
            sampler2D _Artifact3;
            //sampler2D _Artifact4;

            float _ArtifactIntensity;
            float _PixelizationIntensity;

            float _Depth;

            float3 height_blend(float4 color1, float a1, float4 color2, float a2, float4 color3, float a3,
                                float4 color4, float a4)
            {
                float t1 = color1.a + a1;
                float t2 = color2.a + a2;
                float t3 = color3.a + a3;
                float t4 = color4.a + a4;


                float maxValue = max(
                        max(t1, t2),
                        max(t3, t4))
                    - _Depth;

                float k1 = max(t1 - maxValue, 0);
                float k2 = max(t2 - maxValue, 0);
                float k3 = max(t3 - maxValue, 0);
                float k4 = max(t4 - maxValue, 0);

                return (color1.rgb * k1 + color2.rgb * k2 + color3.rgb * k3 + color4.rgb * k4) / (k1 + k2 + k3 + k4);
            }

            // TODO: make get_pixelized_uv
            float pixelate_uv_dimension(float uvDimension, float dimensionSize, float intensity)
            {
                if (_PixelizationIntensity < 0.001 || intensity < 0.01)
                    return uvDimension;
                float oneMinusPixelization = 1 - _PixelizationIntensity * intensity;
                return round(uvDimension * dimensionSize * oneMinusPixelization) / (dimensionSize * oneMinusPixelization
                );
            }


            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;


                float4 maskColor = tex2D(_Mask, uv);
                //float4 maskColor = mask_box_blur(uv);

                float glitchMask = maskColor.a;
                float2 uvPixelized = float2(pixelate_uv_dimension(uv.x, _ScreenParams.x, glitchMask),
                                            pixelate_uv_dimension(uv.y, _ScreenParams.y, glitchMask));


                float4 aColor1 = tex2D(_Artifact1, uvPixelized);
                float4 aColor2 = tex2D(_Artifact2, uvPixelized);
                float4 aColor3 = tex2D(_Artifact3, uvPixelized);

                float4 pixelCameraColor = tex2D(_MainTex, uvPixelized);

                float4 pixelMaskColor = tex2D(_Mask, uvPixelized);

                float glitch = pixelMaskColor.a * _ArtifactIntensity;
                // additive variant
                float3 blended = height_blend(pixelCameraColor, 1 - glitch,
                                              clamp(aColor1 + pixelCameraColor.r, 0, 1), glitch * pixelMaskColor.r,
                                              clamp(aColor2 + pixelCameraColor.g, 0, 1), glitch * pixelMaskColor.g,
                                              clamp(aColor3 + pixelCameraColor.b, 0, 1), glitch * pixelMaskColor.b);
                /*float3 blended = height_blend(pixelCameraColor, 1 - glitch,
                                              aColor1, glitch * pixelMaskColor.r,
                                              aColor2, glitch * pixelMaskColor.g,
                                              aColor3, glitch * pixelMaskColor.b);*/
                return float4(blended, 1);
            }
            ENDCG
        }
    }
}
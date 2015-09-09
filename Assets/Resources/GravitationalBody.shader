
Shader "Custom/GravitationalBody"
{
    SubShader
    {
        Tags
        {
            "IgnoreProjector" = "True"
            "RenderType"      = "Transparent"
            "Queue"           = "Transparent"
        }

        LOD      200
        ZWrite   Off
        Lighting Off

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #include "UnityCG.cginc"
            #include "GravitationalField.cginc"

            #pragma target         5.0
            #pragma vertex         VS_Main
            #pragma fragment       FS_Main
            #pragma only_renderers d3d11

            struct  VS_Input
            {
                float4 position : POSITION;
            };

            struct FS_Input
            {
                float4 position : POSITION;
            };

            RWStructuredBuffer<BodyData> body_data;
            uniform float4x4 object_to_world;

            FS_Input VS_Main(VS_Input input)
            {
                float4 position = input.position;

                FS_Input output =
                {
                    position,
                };

                return output;
            }

            float4 FS_Main(FS_Input input) : COLOR
            {
                return float4(0, 0, 0, 1);
            }

            ENDCG
        }
    }
}

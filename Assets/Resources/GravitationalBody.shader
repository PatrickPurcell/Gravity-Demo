
Shader "Custom/GravitationalBody"
{
    SubShader
    {
        LOD      200
        Lighting Off

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
                float4 color    : COLOR;
            };

            uniform StructuredBuffer<Body> body_buffer;
            uniform float scale;
            uniform uint  index;

            FS_Input VS_Main(VS_Input input)
            {
                float4 position = body_buffer[index].position + input.position * scale;
                position.w = 1;
                position = mul(UNITY_MATRIX_VP, position);

                FS_Input output =
                {
                    position,
                    float4(body_buffer[index].position),
                };

                return output;
            }

            float4 FS_Main(FS_Input input) : COLOR
            {
                return input.color * scale;
            }

            ENDCG
        }
    }
}

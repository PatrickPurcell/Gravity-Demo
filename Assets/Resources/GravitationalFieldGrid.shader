
Shader "Custom/GravitationalFieldGrid"
{
    Properties
    {
    }

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

            #pragma target         5.0
            #pragma vertex         VS_Main
            #pragma fragment       FS_Main
            #pragma geometry       GS_Main
            #pragma only_renderers d3d11

            struct GS_Input
            {
                float4 position : POSITION;
                float4 up       : TEXCOORD0;
                float4 right    : TEXCOORD1;
                float4 forward  : TEXCOORD2;
                float4 color    : COLOR;
            };

            struct FS_Input
            {
                float4 position : POSITION;
                float4 color    : COLOR;
            };

            struct FieldPoint
            {
                float3 position;
                float3 displaced;
            };

            uniform StructuredBuffer<FieldPoint> point_buffer;
            uniform StructuredBuffer<uint3>      grid_buffer;
            uniform float4x4 object_to_world;

            GS_Input VS_Main(uint id : SV_VertexID)
            {
                float  l     = saturate(length(point_buffer[id].position - point_buffer[id].displaced));
                float4 color = lerp(float4(0, 0, 0, 0), float4(0, 0, 0, 0.1f), l);

                uint3 index = grid_buffer[id];
                GS_Input output =
                {
                    mul(object_to_world, float4(point_buffer[id     ].displaced, 1)),
                    mul(object_to_world, float4(point_buffer[index.x].displaced, 1)),
                    mul(object_to_world, float4(point_buffer[index.y].displaced, 1)),
                    mul(object_to_world, float4(point_buffer[index.z].displaced, 1)),
                    color,
                };

                return output;
            }

            void AppendLine(float4 color, float4 p_0, float4 p_1, inout LineStream<FS_Input> line_stream)
            {
                FS_Input output;
                output.color = color;
                output.position = p_0; line_stream.Append(output);
                output.position = p_1; line_stream.Append(output);
            }

            [maxvertexcount(6)]
            void GS_Main(point GS_Input input[1], inout LineStream<FS_Input> line_stream)
            {
                float4 position = mul(UNITY_MATRIX_MVP, input[0].position);

                AppendLine(input[0].color, position, mul(UNITY_MATRIX_MVP, input[0].up     ), line_stream);
                AppendLine(input[0].color, position, mul(UNITY_MATRIX_MVP, input[0].right  ), line_stream);
                AppendLine(input[0].color, position, mul(UNITY_MATRIX_MVP, input[0].forward), line_stream);
            }

            float4 FS_Main(FS_Input input) : COLOR
            {
                return input.color;
            }

            ENDCG
        }
    }
}

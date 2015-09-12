
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
            #include "GravitationalField.cginc"

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

            uniform StructuredBuffer<FieldPoint> point_buffer;
            uniform StructuredBuffer<uint3>      grid_buffer;
            uniform float4x4 object_to_world;

            float4 DisplacedPosition(uint index)
            {
                FieldPoint field_point = point_buffer[index];
                return float4(field_point.position + field_point.displacement, 1);
            }

            GS_Input VS_Main(uint id : SV_VertexID)
            {
                float3 position     = point_buffer[id].position;
                float3 displacement = point_buffer[id].displacement;
                float  l            = saturate(length(position - displacement));
                float4 color        = lerp(float4(0, 0, 0, 0), float4(0, 0, 0, 0.1f), l);

                uint3 index = grid_buffer[id];
                GS_Input output =
                {
                    mul(object_to_world, DisplacedPosition(id     )),
                    mul(object_to_world, DisplacedPosition(index.x)),
                    mul(object_to_world, DisplacedPosition(index.y)),
                    mul(object_to_world, DisplacedPosition(index.z)),
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
                //return float4(0,0,0,1);//input.color;

                return input.color;
            }

            ENDCG
        }
    }
}

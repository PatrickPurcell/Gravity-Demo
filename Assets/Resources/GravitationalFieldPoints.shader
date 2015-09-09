
Shader "Custom/GravitationalFieldPoints"
{
    Properties
    {
        _MainTex("Texture", 2D         ) = "white" { }
        _Size   ("Size",    Range(0, 2)) = 0.15
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
                float4 position  : POSITION;
                float  displaced : TEXCOORD0;
                float4 color     : COLOR;
            };

            struct FS_Input
            {
                float4 position : POSITION;
                float2 texcoord : TEXCOORD0;
                float  distance : TEXCOORD1;
                float4 color    : COLOR;
            };

            struct FieldPoint
            {
                float3 position;
                float3 displaced;
            };

            uniform sampler2D _MainTex;
            uniform float     _Size;

            uniform StructuredBuffer<FieldPoint> point_buffer;
            uniform float4x4 object_to_world;

            GS_Input VS_Main(uint id : SV_VertexID)
            {
                float4 position =
                float4(point_buffer[id].displaced, 1);
                position = mul(object_to_world, position);

                float  l     = saturate(length(point_buffer[id].position - point_buffer[id].displaced));
                float4 color = lerp(float4(0, 0, 0, 0), float4(0, 0, 0.5f, 0.1f), l);
                //color = float4(1, 1, 1, 1);

                GS_Input output =
                {
                    position,
                    distance(position, _WorldSpaceCameraPos),
                    color,
                };

                return output;
            };

            [maxvertexcount(4)]
            void GS_Main(point GS_Input input[1], inout TriangleStream<FS_Input> triangle_stream)
            {
                float3 normal = normalize(_WorldSpaceCameraPos - input[0].position);
                float3 right  = normalize(cross(normal, float3(0, 1, 0)));
                float3 up     = normalize(cross(right,  normal         ));

                float s = _Size * 0.5f;
                right  *= s;
                up     *= s;

                float4 vertex[4];
                float4 position = input[0].position;
                vertex[0] = float4(position.xyz - right + up, 1);
                vertex[1] = float4(position.xyz + right + up, 1);
                vertex[2] = float4(position.xyz - right - up, 1);
                vertex[3] = float4(position.xyz + right - up, 1);

                FS_Input output;
                output.color = input[0].color;

                output.position = mul(UNITY_MATRIX_MVP, vertex[0]);
                output.texcoord = float2(1, 0);
                output.distance = 1.0f * output.position.w;
                triangle_stream.Append(output);

                output.position = mul(UNITY_MATRIX_MVP, vertex[1]);
                output.texcoord = float2(1, 1);
                output.distance = 1.0f * output.position.w;
                triangle_stream.Append(output);

                output.position = mul(UNITY_MATRIX_MVP, vertex[2]);
                output.texcoord = float2(0, 0);
                output.distance = 1.0f * output.position.w;
                triangle_stream.Append(output);

                output.position = mul(UNITY_MATRIX_MVP, vertex[3]);
                output.texcoord = float2(0, 1);
                output.distance = 1.0f * output.position.w;
                triangle_stream.Append(output);
            };

            float4 FS_Main(FS_Input input) : COLOR
            {
                float4 color = tex2D(_MainTex, input.texcoord);
                //color.a *= 0.5f;

                return color * input.color;
            };

            ENDCG
        }
    }
}

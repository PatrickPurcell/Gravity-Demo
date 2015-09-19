
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
                float3 normal   : NORMAL;
            };

            struct FS_Input
            {
                float4 position       : POSITION;
                float3 normal         : NORMAL;
                float4 color          : COLOR;
                float4 world_position : TEXCOORD0;
            };

            uniform StructuredBuffer<Body> body_buffer;
            uniform float4 color;
            uniform float  scale;
            uniform uint   index;

            FS_Input VS_Main(VS_Input input)
            {
                Body  body = body_buffer[index];
                float x    = body.position.x + input.position.x;
                float y    = body.position.y + input.position.y;
                float z    = body.position.z + input.position.z;
                float s    = scale;
                float4x4 object_to_world = float4x4(s, 0, 0, x,
                                                    0, s, 0, y,
                                                    0, 0, s, z,
                                                    0, 0, 0, 1);

                float4 position = body_buffer[index].position + input.position * scale;
                position.w = 1;
                position = mul(UNITY_MATRIX_VP, position);

                FS_Input output =
                {
                    position,
                    normalize((float3)mul(object_to_world, input.normal)),
                    float4(body_buffer[index].position),
                    mul(object_to_world, input.position),
                };

                return output;
            }

            float4 FS_Main(FS_Input input) : COLOR
            {
                float3 light_direction =
                normalize(-input.world_position);
                float n_dot_l = saturate(dot(light_direction, input.normal));

                float4 output = color * n_dot_l;

                return output + float4(0.15f, 0.15f, 0.15f, 1);
            }

            ENDCG
        }
    }
}

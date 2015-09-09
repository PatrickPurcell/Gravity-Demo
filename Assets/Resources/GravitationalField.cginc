
#ifndef GRAVITATIONAL_FIELD_CGINC
#define GRAVITATIONAL_FIELD_CGINC

struct FieldPoint
{
    float3 position;
    float3 displaced_position;
};

struct BodyData
{
    float4 position;
    float3 velocity;
    float  mass;
};

uint w, h, d;

uint Index(int x, int y, int z)
{
    return x + y * w + z * w * h;
}

uint Index(uint3 i)
{
    return Index(i.x, i.y, i.z);
}

#endif

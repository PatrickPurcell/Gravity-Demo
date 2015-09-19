
#ifndef GRAVITATIONAL_FIELD_CGINC
#define GRAVITATIONAL_FIELD_CGINC

#define G (0.6674f)

struct FieldPoint
{
    float3 position;
    float3 displacement;
};

struct Body
{
    float4 position;
    float3 velocity;
    float  mass;
};

uint w, h, d;

uint Index(uint x, uint y, uint z)
{
    return x + y * w + z * w * h;
}

uint Index(uint3 i)
{
    return Index(i.x, i.y, i.z);
}

float3 GravitationalDisplacement(Body body, FieldPoint field_point)
{
    float3 displacement = (float3)0;

    if (body.mass)
    {
        float3 v = body.position.xyz - field_point.position;
        float  r = length(v);
        float  f = G * (body.mass / (r * r));
        displacement = v / r * min(f, r);
    }

    return displacement;
}

#endif

#ifndef WAVE_FUNCTIONS_INCLUDED
#define WAVE_FUNCTIONS_INCLUDED


float3 GerstnerWave(float4 wave, float3 inPos, inout float3 tangent, inout float3 binormal)
{
	float steepness = wave.z;
	float wavelength = wave.w;

	float t = _Time.y;

	float k = 2.0 * 3.14159265 / wavelength;
	float c = sqrt(9.81 / k);
	float2 d = normalize(wave.xy);
	float f = k * (dot(d, inPos.xz) - c * t);
	float amplitude = steepness / k;

	tangent += float3(
		-d.x * d.x * (steepness * sin(f)),
		d.x * (steepness * cos(f)),
		-d.x * d.y * (steepness * sin(f))
		);

	binormal += float3(
		-d.x * d.y * (steepness * sin(f)),
		d.y * (steepness * cos(f)),
		-d.y * d.y * (steepness * sin(f))
		);

	return float3(
		d.x * (amplitude * cos(f)),
		amplitude * sin(f),
		d.y * (amplitude * cos(f))
		);
}

void GetWavePos_float(float3 objectPos, float3 worldPos, float4 waveA, float4 waveB, float3 scale,
	out float3 outPos, out float3 tangent, out float3 normal)
{

	tangent = float3(1, 0, 0);
	float3 binormal = float3(0, 0, 1);

	float3 displacement = 0;

	displacement += GerstnerWave(waveA, worldPos, tangent, binormal);
	displacement += GerstnerWave(waveB, worldPos, tangent, binormal);

	displacement.x = displacement.x / scale.x;
	displacement.y = displacement.y / scale.y;
	displacement.z = displacement.z / scale.z;

	normal = normalize(cross(binormal, tangent));
	outPos.xyz = objectPos + displacement;

}



#endif //WAVE_FUNCTIONS_INCLUDED
#ifndef WAVE_FUNCTIONS_INCLUDED
#define WAVE_FUNCTIONS_INCLUDED

void GetWavePos_float(float3 inPos, float frequency, float steepness, float wavelength,
	out float3 outPos, out float3 tangent, out float3 normal)
{
	float t = _Time.y;
	float pi = 3.14;
	float k = 2 * pi / wavelength;
	float c = sqrt(9.8 / k);
	float f = k * (inPos.x - c * t);
	float amplitude = steepness / k;
	
	outPos.x = inPos.x + amplitude * cos(f);
	outPos.y = amplitude * sin(f);
	outPos.z = inPos.z;

	tangent = normalize(float3(
		1 - k * steepness * sin(f),
		k * steepness * cos(f),
		0));
	normal = float3(-tangent.y, tangent.x, 0);
}



#endif //WAVE_FUNCTIONS_INCLUDED
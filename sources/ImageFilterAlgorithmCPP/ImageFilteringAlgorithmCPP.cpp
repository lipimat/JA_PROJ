#include "pch.h"
#include "ImageFilteringAlgorithmCPP.h"

void cppProc(UINT8* pixels, int len)
{
	for (int i = 0; i < len; i++) {
		//blue
		if (i % 4 == 0) pixels[i] = 255;
		//green
		else if (i % 4 == 1) pixels[i] = 0;
		//blue
		else if (i % 4 == 2) pixels[i] = 0;


		//else alpha - we don't care
	}
}
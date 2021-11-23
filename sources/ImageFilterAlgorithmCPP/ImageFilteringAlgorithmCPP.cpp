#include "pch.h"
#include "ImageFilteringAlgorithmCPP.h"

void cppProc(ImageInfoStruct* imageInfo)
{
	for (int i = 0; i < imageInfo->pixelsLen; i++) {
		//blue
		if (i % 4 == 0) imageInfo->pixels[i] = 255;
		//green
		else if (i % 4 == 1) imageInfo->pixels[i] = 0;
		//blue
		else if (i % 4 == 2) imageInfo->pixels[i] = 0;
		//else alpha - we don't care
	}
}
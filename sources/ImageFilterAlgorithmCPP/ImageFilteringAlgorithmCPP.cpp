#include "pch.h"
#include "ImageFilteringAlgorithmCPP.h"

void cppProc(ImageInfoStruct* imageInfo)
{
	int width = imageInfo->width;
	int height = imageInfo->height;
	Pixel* pixelArray = imageInfo->pixels;
	float* kernel = imageInfo->matrix;

	//loop through all pixels*
	//*for now we skip first and last row, and first and last column
	for (int i = width; i < width * (height - 1); ++i)
	{
		//we hit first or last column
		if (i % width == 0 || i % width  == width - 1) continue;

		//calculate pixel member values
		float bValues = 0, gValues = 0, rValues = 0;
		int curKernelRow = 0, curKernelCol = 0;
		for (int k = i - width - 1; k <= i + width - 1; k += width)
		{

			for (int j = k; j <= k + 2; ++j)
			{
				bValues += pixelArray[j].bValue * kernel[curKernelRow * 3 + curKernelCol];
				gValues += pixelArray[j].gValue * kernel[curKernelRow * 3 + curKernelCol];
				rValues += pixelArray[j].rValue * kernel[curKernelRow * 3 + curKernelCol];
				++curKernelCol;
			}
			++curKernelRow;
			curKernelCol = 0;
		}

		if (bValues > 255)
			bValues = 255;
		else if (bValues < 0)
			bValues = 0;
		if (gValues > 255)
			gValues = 255;
		else if (gValues < 0)
			gValues = 0;
		if (rValues > 255)
			rValues = 255;
		else if (rValues < 0)
			rValues = 0;

		pixelArray[i].bValue = bValues;
		pixelArray[i].gValue = gValues;
		pixelArray[i].rValue = rValues;
	}
}
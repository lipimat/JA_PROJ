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

		//kernel and pixel nieghborhood are indexed as follows
		//kernel						//neighborhood
		//0 3 6							//i-width-1	i-width	i-width+1
		//1 4 7							//i-1	i(currently filtered)	i+1	
		//2 5 8							//i+width-1	i+width	i+width+1

		//we are not using loop to lower down complexity

		bValues +=
			pixelArray[i - width - 1].bValue * kernel[0] + pixelArray[i - width].bValue * kernel[3] +
			pixelArray[i - width + 1].bValue * kernel[6] + pixelArray[i - 1].bValue * kernel[1] +
			pixelArray[i].bValue * kernel[4] + pixelArray[i + 1].bValue * kernel[7] +
			pixelArray[i + width - 1].bValue * kernel[2] + pixelArray[i + width].bValue * kernel[5] +
			pixelArray[i + width + 1].bValue * kernel[8];
		gValues += 
			pixelArray[i - width - 1].gValue * kernel[0] + pixelArray[i - width].gValue * kernel[3] +
			pixelArray[i - width + 1].gValue * kernel[6] + pixelArray[i - 1].gValue * kernel[1] +
			pixelArray[i].gValue * kernel[4] + pixelArray[i + 1].gValue * kernel[7] +
			pixelArray[i + width - 1].gValue * kernel[2] + pixelArray[i + width].gValue * kernel[5] +
			pixelArray[i + width + 1].gValue * kernel[8];
		rValues += 
			pixelArray[i - width - 1].rValue * kernel[0] + pixelArray[i - width].rValue * kernel[3] +
			pixelArray[i - width + 1].rValue * kernel[6] + pixelArray[i - 1].rValue * kernel[1] +
			pixelArray[i].rValue * kernel[4] + pixelArray[i + 1].rValue * kernel[7] +
			pixelArray[i + width - 1].rValue * kernel[2] + pixelArray[i + width].rValue * kernel[5] +
			pixelArray[i + width + 1].rValue * kernel[8];

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
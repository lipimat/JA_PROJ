#include "pch.h"
#include "ImageFilteringAlgorithmCPP.h"

void cppProc(ImageInfoStruct* imageInfo)
{
	int alphaCounter = 1;
	int counter = 0;
	int width = imageInfo->countOfBytesInRow;
	int height = imageInfo->height;
	UINT8* byteArray = imageInfo->bytes;
	int* kernel = imageInfo->matrix;
	int checkSum;
	if (imageInfo->checkSum == 0)
		checkSum = 1;
	else checkSum = imageInfo->checkSum;

	int newByteVal;
	//loop through all bytes
	//*for now we skip first and last row, and first and last column
	for (int i = width + 4; i < width * (height - 1); i++)
	{
		//we hit first or last column so skip 3 bytes and 1 from loop counter
		if (i % width == 0 || i % width == width - 4)
		{
			i += 3; //and one from loop counter
			continue;
		}
		//we hit alpha
		if (alphaCounter == 4) {
			alphaCounter = 1;
			continue;
		}

		alphaCounter++;
		//kernel and *one of pixelvals(r,g,b) nieghborhood are indexed as follows
		//kernel						//neighborhood
		//0 3 6							//i-width-4	i-width	i-width+4
		//1 4 7							//i-4	i(currently filtered)	i+4	
		//2 5 8							//i+width-4	i+width	i+width+4
		newByteVal =
			byteArray[i  - width - 4] * kernel[0] + byteArray[i - width] * kernel[3] + byteArray[i - width + 4] * kernel[6] 
		  + byteArray[i - 4] * kernel[1] + byteArray[i] * kernel[4] + byteArray[i + 4] * kernel[7] 
		  + byteArray[i + width - 4] * kernel[2] + byteArray[i + width] * kernel[5] + byteArray[i + width + 4] * kernel[8];

		if (newByteVal < 0) newByteVal = 0;
		byteArray[i] = newByteVal /checkSum;
	}
}
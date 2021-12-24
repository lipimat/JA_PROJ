#include "pch.h"
#include "ImageFilteringAlgorithmCPP.h"

void cppProc(ImageInfoStruct* imageInfo)
{ 
	int width = imageInfo->countOfBytesInRow;
	int height = imageInfo->height;
	UINT8* originalArray = imageInfo->originalBytes;
	UINT8* copyArray = imageInfo->resultBytes;
	int* kernel = imageInfo->matrix;


	int checkSum;
	if (imageInfo->checkSum == 0)
		checkSum = 1;
	else checkSum = imageInfo->checkSum;

	int alphaCounter = 1; //if == 4 we hit alpha byte so skip one
	int lineCounter = 5; //current pos in respect to line
	int newByteVal;

	int end = width * (height - 1);
	//loop through all bytes
	//*for now we skip first and last row, and first and last column
	for (int i = width + 4; i < end; i++)
	{
		//we hit last column so skip 8 bytes
		if (lineCounter == width - 4) {
			lineCounter = 5;
			i += 8;
		}
		
		//we hit alpha
		if (alphaCounter == 4) {
			alphaCounter = 1;
			lineCounter++;
			i++;
		}

		lineCounter++;
		alphaCounter++;
		//kernel and *one of pixelvals(r,g,b) nieghborhood are indexed as follows
		//kernel						//neighborhood
		//0 3 6							//i-width-4	i-width	i-width+4
		//1 4 7							//i-4	i(currently filtered)	i+4	
		//2 5 8							//i+width-4	i+width	i+width+4
		newByteVal =
			originalArray[i  - width - 4] * kernel[0] + originalArray[i - width] * kernel[3] + originalArray[i - width + 4] * kernel[6] 
		  + originalArray[i - 4] * kernel[1] + originalArray[i] * kernel[4] + originalArray[i + 4] * kernel[7] 
		  + originalArray[i + width - 4] * kernel[2] + originalArray[i + width] * kernel[5] + originalArray[i + width + 4] * kernel[8];

		if (newByteVal < 0) newByteVal = 0;

		copyArray[i] = newByteVal /checkSum;
	}
}
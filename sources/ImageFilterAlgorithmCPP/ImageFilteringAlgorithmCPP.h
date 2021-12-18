#pragma once

#ifdef IMAGE_FILTERING_ALGORITHM_CPP_EXPORTS
#define ALGORITHM_API __declspec(dllexport)
#else
#define ALGORITHM_API __declspec(dllimport)
#endif

struct ImageInfoStruct
{
	//order here -> blue -> green -> red -> alpha -> ...
	UINT8* bytes;
	int countOfBytesInRow;
	int height;
	int* matrix;
	int checkSum;
};

extern "C" ALGORITHM_API void cppProc(ImageInfoStruct* imageInfo);

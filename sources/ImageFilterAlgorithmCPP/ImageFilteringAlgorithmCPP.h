#pragma once

#ifdef IMAGE_FILTERING_ALGORITHM_CPP_EXPORTS
#define ALGORITHM_API __declspec(dllexport)
#else
#define ALGORITHM_API __declspec(dllimport)
#endif

struct ImageInfoStruct
{
	UINT8* pixels;
	int pixelsLen;
	int** matrix;
};

extern "C" ALGORITHM_API void cppProc(ImageInfoStruct* imageInfo);

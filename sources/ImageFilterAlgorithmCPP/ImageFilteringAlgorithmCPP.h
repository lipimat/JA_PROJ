#pragma once

#ifdef IMAGE_FILTERING_ALGORITHM_CPP_EXPORTS
#define ALGORITHM_API __declspec(dllexport)
#else
#define ALGORITHM_API __declspec(dllimport)
#endif

struct Pixel
{
	UINT8 bValue;
	UINT8 gValue;
	UINT8 rValue;

	//not used
	UINT8 alpha;
};

struct ImageInfoStruct
{
	Pixel* pixels;
	int width;
	int height;
	int** matrix;
};

extern "C" ALGORITHM_API void cppProc(ImageInfoStruct* imageInfo);

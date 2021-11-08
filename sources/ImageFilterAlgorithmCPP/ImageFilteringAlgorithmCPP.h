#pragma once

#ifdef IMAGE_FILTERING_ALGORITHM_CPP_EXPORTS
#define ALGORITHM_API __declspec(dllexport)
#else
#define ALGORITHM_API __declspec(dllimport)
#endif

extern "C" ALGORITHM_API void cppProc(UINT8 * pixels, int len);

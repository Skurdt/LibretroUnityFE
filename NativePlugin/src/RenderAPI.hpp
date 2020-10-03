#pragma once

#include <IUnityGraphics.h>

#include <stddef.h>

struct IUnityInterfaces;

class RenderAPI
{
public:
	virtual ~RenderAPI() = default;

	virtual void ProcessDeviceEvent(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces) = 0;

	virtual unsigned int GetCurrentFramebuffer() = 0;
	virtual void* GetHwProcAddress(const char* sym) = 0;

	virtual bool InitFramebuffer(void* textureHandle, void* renderbufferHandle, int width, int height, bool depth, bool stencil) = 0;
	virtual void DeinitFramebuffer() = 0;
};

RenderAPI* CreateRenderAPI(UnityGfxRenderer apiType);

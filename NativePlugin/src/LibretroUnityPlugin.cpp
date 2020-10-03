#include "PlatformBase.hpp"
#include "RenderAPI.hpp"

#include <cassert>
#include <mutex>

typedef void (*retro_hw_context_reset_t)();
typedef void (*retro_run_t)();

static struct
{
	IUnityInterfaces* interfaces = nullptr;
	IUnityGraphics* graphics     = nullptr;
	UnityGfxRenderer deviceType  = UnityGfxRenderer::kUnityGfxRendererNull;
} g_UnityData;

static struct InteropInterface
{
	retro_hw_context_reset_t context_reset   = nullptr;
	retro_hw_context_reset_t context_destroy = nullptr;
	retro_run_t retro_run                    = nullptr;
} *g_InteropInterface = nullptr;

struct InitContextData
{
	void* textureHandle;
	void* renderbufferHandle;
	int width;
	int height;
	bool depth;
	bool stencil;
};

static RenderAPI* g_CurrentAPI = nullptr;
static bool g_FramebufferInitialized      = false;
static std::mutex g_Lock;

extern "C" UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API SetupInteropInterface(InteropInterface* interopInterface)
{
	g_InteropInterface = interopInterface;
}

extern "C" UNITY_INTERFACE_EXPORT unsigned int UNITY_INTERFACE_API GetCurrentFramebuffer()
{
	return g_CurrentAPI->GetCurrentFramebuffer();
}

extern "C" UNITY_INTERFACE_EXPORT void* UNITY_INTERFACE_API GetHwProcAddress(const char* sym)
{
	return g_CurrentAPI->GetHwProcAddress(sym);
}

static void UNITY_INTERFACE_API OnRenderEvent(int eventID, void* data)
{
	if (!g_CurrentAPI)
		return;

	std::lock_guard<std::mutex> guard(g_Lock);

	switch (eventID)
	{
	case 0:
	{
		if (data)
		{
			InitContextData* initData = (InitContextData*)data;
			if (g_CurrentAPI->InitFramebuffer(initData->textureHandle, initData->renderbufferHandle, initData->width, initData->height, initData->depth, initData->stencil))
			{
				if (g_InteropInterface->context_reset)
					g_InteropInterface->context_reset();
				g_FramebufferInitialized = true;
			}
		}
	}
	break;
	case 1:
	{
		if (g_FramebufferInitialized)
		{
			if (g_InteropInterface->context_destroy)
				g_InteropInterface->context_destroy();
			g_CurrentAPI->DeinitFramebuffer();
			g_FramebufferInitialized = false;
		}
	}
	break;
	case 2:
	{
		if (g_FramebufferInitialized && g_InteropInterface->retro_run)
			g_InteropInterface->retro_run();
	}
	break;
	default:
		break;
	}
}

extern "C" UNITY_INTERFACE_EXPORT UnityRenderingEventAndData UNITY_INTERFACE_API GetRenderEventFunc()
{
	return OnRenderEvent;
}

static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType)
{
	if (eventType == kUnityGfxDeviceEventInitialize)
	{
		assert(g_CurrentAPI == nullptr);
		g_UnityData.deviceType = g_UnityData.graphics->GetRenderer();
		g_CurrentAPI           = CreateRenderAPI(g_UnityData.deviceType);
	}

	if (g_CurrentAPI)
		g_CurrentAPI->ProcessDeviceEvent(eventType, g_UnityData.interfaces);

	if (eventType == kUnityGfxDeviceEventShutdown)
	{
		if (g_CurrentAPI)
		{
			delete g_CurrentAPI;
			g_CurrentAPI = nullptr;
		}
		g_UnityData.deviceType = kUnityGfxRendererNull;
	}
}

extern "C" UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
{
	g_UnityData.interfaces = unityInterfaces;
	g_UnityData.graphics   = unityInterfaces->Get<IUnityGraphics>();
	g_UnityData.graphics->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);

#if SUPPORT_VULKAN
	if (g_UnityData.graphics->GetRenderer() == kUnityGfxRendererNull)
	{
		extern void RenderAPI_Vulkan_OnPluginLoad(IUnityInterfaces*);
		RenderAPI_Vulkan_OnPluginLoad(unityInterfaces);
	}
#endif

	OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
}

extern "C" UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API UnityPluginUnload()
{
	g_UnityData.graphics->UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);
}

#include <IUnityGraphics.h>

#include "glad/glad.c"

#include <cassert>
#include <cstdio>

typedef void (*retro_hw_context_reset_t)();
typedef void (*retro_run_t)();

static struct
{
	IUnityInterfaces* interfaces = nullptr;
	IUnityGraphics* graphics     = nullptr;
} g_UnityData;

static struct InteropInterface
{
	retro_hw_context_reset_t context_reset   = nullptr;
	retro_hw_context_reset_t context_destroy = nullptr;
	retro_run_t retro_run                    = nullptr;
} *g_InteropInterface = nullptr;

static struct
{
	void* texture = nullptr;
	int width     = 0;
	int height    = 0;
} g_RenderData;

static bool g_Initialized    = false;
static GLuint g_Texture      = 0;
static GLuint g_RenderBuffer = 0;
static GLuint g_Framebuffer  = 0;

extern "C" __declspec(dllexport) void UNITY_INTERFACE_API SetupInteropInterface(InteropInterface* interopInterface)
{
	g_InteropInterface = interopInterface;
}

extern "C" __declspec(dllexport) void UNITY_INTERFACE_API SendTexture(void* texture, int width, int height)
{
	g_RenderData.texture = texture;
	g_RenderData.width   = width;
	g_RenderData.height  = height;
}

extern "C" __declspec(dllexport) GLuint UNITY_INTERFACE_API GetCurrentFramebuffer()
{
	return g_Framebuffer;
}

extern "C" __declspec(dllexport) void* UNITY_INTERFACE_API GetHwProcAddress(const char* sym)
{
	return static_cast<void*>(gladGetProcAddressPtr(sym));
}

static void InitContext()
{
	assert(g_InteropInterface);
	assert(g_RenderData.texture);
	assert(g_RenderData.width);
	assert(g_RenderData.height);

	if (g_RenderBuffer)
	{
		glDeleteRenderbuffers(1, &g_RenderBuffer);
		g_RenderBuffer = 0;
	}

	if (g_Framebuffer)
	{
		glDeleteFramebuffers(1, &g_Framebuffer);
		g_Framebuffer = 0;
	}

	glGenRenderbuffers(1, &g_RenderBuffer);
	glBindRenderbuffer(GL_RENDERBUFFER, g_RenderBuffer);
	glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, g_RenderData.width, g_RenderData.height);
	glBindRenderbuffer(GL_RENDERBUFFER, 0);

	glGenFramebuffers(1, &g_Framebuffer);
	glBindFramebuffer(GL_FRAMEBUFFER, g_Framebuffer);
	glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, static_cast<GLuint>(reinterpret_cast<size_t>(g_RenderData.texture)), 0);
	glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, g_RenderBuffer);
	assert(glCheckFramebufferStatus(GL_FRAMEBUFFER) == GL_FRAMEBUFFER_COMPLETE);

	glClearColor(1.0f, 0.0f, 0.0f, 1.0f);
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	glViewport(0, 0, g_RenderData.width, g_RenderData.height);

	glBindFramebuffer(GL_FRAMEBUFFER, 0);

	if (g_InteropInterface->context_reset)
		g_InteropInterface->context_reset();
}

static void ShutdownContext()
{
	if (g_InteropInterface->context_destroy)
		g_InteropInterface->context_destroy();

	if (g_Texture)
	{
		glDeleteTextures(1, &g_Texture);
		g_Texture = 0;
	}

	if (g_RenderBuffer)
	{
		glDeleteRenderbuffers(1, &g_RenderBuffer);
		g_RenderBuffer = 0;
	}

	if (g_Framebuffer)
	{
		glDeleteFramebuffers(1, &g_Framebuffer);
		g_Framebuffer = 0;
	}
}

static void RetroRun()
{
	g_InteropInterface->retro_run();
}

static void UNITY_INTERFACE_API OnRenderEvent(int eventID)
{
	switch (eventID)
	{
	case 0:
		InitContext();
		break;
	case 1:
		ShutdownContext();
		break;
	case 2:
		RetroRun();
		break;
	default:
		break;
	}
}

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetRenderEventFunc()
{
	return OnRenderEvent;
}

static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType)
{
	// Create graphics API implementation upon initialization
	if (eventType == kUnityGfxDeviceEventInitialize)
	{
		//assert(s_CurrentAPI == NULL);
		//s_DeviceType = s_Graphics->GetRenderer();
		//s_CurrentAPI = CreateRenderAPI(s_DeviceType);
		if (!g_Initialized)
		{
			gladLoadGL();
			glEnable(GL_DEBUG_OUTPUT);
			glDebugMessageCallback([] (GLenum source, GLenum type, GLuint id, GLenum severity, GLsizei length, const GLchar* message, const void* userParam)
			{
				UNREFERENCED_PARAMETER(id); UNREFERENCED_PARAMETER(length); UNREFERENCED_PARAMETER(source); UNREFERENCED_PARAMETER(userParam);
				char buffer[1024];
				sprintf_s(buffer, "GL CALLBACK: %s type = 0x%x, severity = 0x%x, message = %s\n", (type == GL_DEBUG_TYPE_ERROR ? "** GL ERROR **" : ""), type, severity, message);
				printf(buffer);
			},
			nullptr);
			g_Initialized = true;
		}
	}

	// Let the implementation process the device related events
	//if (s_CurrentAPI)
	//{
	//	s_CurrentAPI->ProcessDeviceEvent(eventType, s_UnityInterfaces);
	//}

	// Cleanup graphics API implementation upon shutdown
	if (eventType == kUnityGfxDeviceEventShutdown)
	{
		//delete s_CurrentAPI;
		//s_CurrentAPI = NULL;
		//s_DeviceType = kUnityGfxRendererNull;
		g_Initialized = false;
	}
}

extern "C" void	UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
{
	g_UnityData.interfaces = unityInterfaces;
	g_UnityData.graphics   = unityInterfaces->Get<IUnityGraphics>();
	g_UnityData.graphics->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);
	OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
	g_UnityData.graphics->UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);
}

#include "RenderAPI.hpp"
#include "PlatformBase.hpp"

// OpenGL Core profile (desktop) or OpenGL ES (mobile) implementation of RenderAPI.
// Supports several flavors: Core, ES2, ES3

#if SUPPORT_OPENGL_UNIFIED
#include <assert.h>
#if UNITY_IOS || UNITY_TVOS
#	include <OpenGLES/ES2/gl.h>
#elif UNITY_ANDROID || UNITY_WEBGL
#	include <GLES2/gl2.h>
#elif UNITY_OSX
#	include <OpenGL/gl3.h>
#elif UNITY_WIN
// On Windows, use a library to initialize and load OpenGL Core functions.
#	include "glad/glad.c"
#elif UNITY_LINUX
#	define GL_GLEXT_PROTOTYPES
#	include <GL/gl.h>
#else
#	error Unknown platform
#endif

class RenderAPI_OpenGLCoreES : public RenderAPI
{
public:
	RenderAPI_OpenGLCoreES(UnityGfxRenderer apiType);
	virtual ~RenderAPI_OpenGLCoreES() override = default;

	virtual void ProcessDeviceEvent(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces) override;

	virtual unsigned int GetCurrentFramebuffer() override;
	virtual void* GetHwProcAddress(const char* sym) override;

	virtual bool InitFramebuffer(void* textureHandle, int width, int height, bool depth, bool stencil) override;
	virtual void DeinitFramebuffer() override;

private:
	void CreateResources();

private:
	UnityGfxRenderer m_APIType = UnityGfxRenderer::kUnityGfxRendererNull;
	GLuint m_Framebuffer       = 0;
	GLuint m_Renderbuffer      = 0;
};

RenderAPI* CreateRenderAPI_OpenGLCoreES(UnityGfxRenderer apiType)
{
	return new RenderAPI_OpenGLCoreES(apiType);
}

RenderAPI_OpenGLCoreES::RenderAPI_OpenGLCoreES(UnityGfxRenderer apiType)
: m_APIType(apiType)
{
}

void RenderAPI_OpenGLCoreES::ProcessDeviceEvent(UnityGfxDeviceEventType type, IUnityInterfaces* /*interfaces*/)
{
	if (type == kUnityGfxDeviceEventInitialize)
	{
		CreateResources();
	}
	else if (type == kUnityGfxDeviceEventShutdown)
	{
		//@TODO: release resources
	}
}

unsigned int RenderAPI_OpenGLCoreES::GetCurrentFramebuffer()
{
	return m_Framebuffer;
}

void* RenderAPI_OpenGLCoreES::GetHwProcAddress(const char* sym)
{
	return gladGetProcAddressPtr(sym);
}

bool RenderAPI_OpenGLCoreES::InitFramebuffer(void* textureHandle, int width, int height, bool depth, bool stencil)
{
	DeinitFramebuffer();

	if (depth)
	{
		glGenRenderbuffers(1, &m_Renderbuffer);
		glBindRenderbuffer(GL_RENDERBUFFER, m_Renderbuffer);
		glRenderbufferStorage(GL_RENDERBUFFER, stencil ? GL_DEPTH24_STENCIL8 : GL_DEPTH_COMPONENT24, width, height);
		glBindRenderbuffer(GL_RENDERBUFFER, 0);
	}

	glGenFramebuffers(1, &m_Framebuffer);
	glBindFramebuffer(GL_FRAMEBUFFER, m_Framebuffer);
	glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, static_cast<GLuint>(reinterpret_cast<size_t>(textureHandle)), 0);
	if (depth)
		glFramebufferRenderbuffer(GL_FRAMEBUFFER, stencil ? GL_DEPTH_STENCIL_ATTACHMENT : GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, m_Renderbuffer);

	if (glCheckFramebufferStatus(GL_FRAMEBUFFER) == GL_FRAMEBUFFER_COMPLETE)
	{
		GLbitfield clearBits = GL_COLOR_BUFFER_BIT;
		if (depth)
			clearBits |= GL_DEPTH_BUFFER_BIT;
		if (stencil)
			clearBits |= GL_STENCIL_BUFFER_BIT;
		glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
		glClear(clearBits);
		glViewport(0, 0, width, height);

		glBindFramebuffer(GL_FRAMEBUFFER, 0);
		return true;
	}

	glBindFramebuffer(GL_FRAMEBUFFER, 0);
	DeinitFramebuffer();
	return false;
}

void RenderAPI_OpenGLCoreES::DeinitFramebuffer()
{
	if (m_Framebuffer)
	{
		glDeleteFramebuffers(1, &m_Framebuffer);
		m_Framebuffer = 0;
	}

	if (m_Renderbuffer)
	{
		glDeleteRenderbuffers(1, &m_Renderbuffer);
		m_Renderbuffer = 0;
	}
}

void RenderAPI_OpenGLCoreES::CreateResources()
{
#if SUPPORT_OPENGL_CORE
	if (m_APIType == kUnityGfxRendererOpenGLCore)
	{
#if UNITY_WIN
		assert(gladLoadGL());
#endif // UNITY_WIN
		glEnable(GL_DEBUG_OUTPUT);
		glDebugMessageCallback([] (GLenum source, GLenum type, GLuint id, GLenum severity, GLsizei length, const GLchar* message, const void* userParam)
		{
			UNREFERENCED_PARAMETER(id); UNREFERENCED_PARAMETER(length); UNREFERENCED_PARAMETER(source); UNREFERENCED_PARAMETER(userParam);
			char buffer[1024];
			sprintf_s(buffer, "GL CALLBACK: %s type = 0x%x, severity = 0x%x, message = %s\n", (type == GL_DEBUG_TYPE_ERROR ? "** GL ERROR **" : ""), type, severity, message);
			printf(buffer);
		},
		nullptr);
	}
#endif // SUPPORT_OPENGL_CORE
}
#endif // SUPPORT_OPENGL_UNIFIED

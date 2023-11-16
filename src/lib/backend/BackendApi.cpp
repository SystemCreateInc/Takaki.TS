// CodeFind.cpp : DLL 用の初期化処理の定義を行います。
//

#include "framework.h"
#include "backendApi.h"

#ifdef BACKENDDLL
#define		BACKENDENTRY	APIENTRY
#define		BACKENDAPI		WINAPI
#endif
#include <backend.h>
#include <syslog.h>
#include <cassert>

extern "C" int APIENTRY
DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved)
{
	// lpReserved を使う場合はここを削除してください
	UNREFERENCED_PARAMETER(lpReserved);

	if (dwReason == DLL_PROCESS_ATTACH)
	{

	}
	else if (dwReason == DLL_PROCESS_DETACH)
	{
	}
	return 1;   // ok
}

static thread_local be_state bes;

int __stdcall BackendInit(be_state** pbes, int clientType, const char* name, const char* capability, const char* user, const char* passwd, const char* backendAddress)
{
	SLPrintf(SV_DEBUG, "BackendInit: %d", ::GetCurrentThreadId());
	WSADATA data;
	WSAStartup(MAKEWORD(1, 0), &data);

	*pbes = new be_state;
	auto bes = *pbes;

	int rc = BE_Init(bes);
	if (rc != BE_ERROR_OK)
		return rc;

	bes->clienttype = clientType;
	bes->name = _strdup(name);
	bes->capability = _strdup(capability);
	bes->user = _strdup(user);
	bes->passwd = _strdup(passwd);
	bes->backend = backendAddress ? _strdup(backendAddress) : nullptr;

	return rc;
}

void __stdcall BackendTerm(be_state* bes)
{
	if (bes->internal)
	{
		BE_Disconnect(bes);
		BE_Term(bes);
	}

	free((void*)bes->name);
	free((void*)bes->capability);
	free((void*)bes->user);
	free((void*)bes->passwd);
	free((void*)bes->backend);
	delete bes;

	WSACleanup();
	SLPrintf(SV_DEBUG, "BackendTerm: %d", ::GetCurrentThreadId());
}


int __stdcall BackendCallByName(
	be_state* bes,
	LPCSTR capability,			/*	送信先のキャパビリティー		*/
	long message,				/*	アプリケーション定義メッセージ	*/
	long sendsize,				/*	送信データサイズ				*/
	LPCSTR sendbuf,			/*	送信データ						*/
	long* recvsize,				/*	受信データサイズ				*/
	LPSTR* recvbuf			/*	受信データ						*/
)
{
	char* rawRecvbuf = nullptr;
	int rc = BE_CallByName(bes,
		capability,
		message,
		sendsize,
		sendbuf,
		recvsize,
		&rawRecvbuf);

	if (*recvsize)
	{
		::CoTaskMemFree(*recvbuf);
		*recvbuf = (LPSTR)::CoTaskMemAlloc(*recvsize + 1);
		if (*recvbuf)
		{
			memcpy(*recvbuf, rawRecvbuf, *recvsize);
			(*recvbuf)[*recvsize] = 0;
		}

		free(rawRecvbuf);
	}

	return rc;
}


static thread_local be_message* bem;

int __stdcall BackendGetMessage(be_state* bes, long* from_clientid, long* message, long* size, LPSTR* data)
{
	*size = 0;

	int rc = BE_GetMessage(bes, &bem);
	if (bem)
	{
		*message = bem->message;
		*from_clientid = bem->src_clientid;
		*size = bem->size - offsetof(struct be_message, data);
		if (*size)
		{
			*data = (LPSTR)::CoTaskMemAlloc(*size);
			memcpy(*data, bem->data, *size);
		}
	}

	return rc;
}

int __stdcall BackendCancel(be_state* bes)
{
	return BE_Cancel(bes);
}

int __stdcall BackendPostData(
	be_state* bes,
	long handle,				/*	送信先のハンドル				*/
	short status,				/*	ステータス						*/
	long message,				/*	アプリケーション定義メッセージ	*/
	long sendsize,				/*	送信データサイズ				*/
	const char* sendbuf)				/*	送信データ						*/
{
	return BE_PostData(bes, handle, status, message, sendsize, sendbuf);
}

int __stdcall BackendPostDataByName(
	be_state* bes,
	const char* capability,		/*	送信先のハンドル				*/
	short status,				/*	ステータス						*/
	long message,				/*	アプリケーション定義メッセージ	*/
	long sendsize,				/*	送信データサイズ				*/
	const char* sendbuf)				/*	送信データ						*/
{
	return BE_PostDataByName(bes, capability, status, message, sendsize, sendbuf);
}

#pragma once

#ifdef BACKEND_EXPORTS
#define	BACKEND_API		__declspec(dllexport)
#else
#define	BACKEND_API		__declspec(dllimport)
#endif

#include <backend.h>

extern "C"
{
	BACKEND_API int __stdcall BackendInit(be_state** bes, int clientType, const char* name, const char* capability, const char* user, const char* passwd, const char* backendAddress);
	BACKEND_API void __stdcall BackendTerm(be_state* bes);


	BACKEND_API int __stdcall BackendCallByName(
		be_state* bes,
		LPCSTR capability,			/*	送信先のキャパビリティー		*/
		long message,				/*	アプリケーション定義メッセージ	*/
		long sendsize,				/*	送信データサイズ				*/
		LPCSTR sendbuf,				/*	送信データ						*/
		long* recvsize,				/*	受信データサイズ				*/
		LPSTR* recvbuf				/*	受信データ						*/
	);

	BACKEND_API int __stdcall BackendGetMessage(be_state* bes, long* from_clientid, long* message, long* size, LPSTR* data);
	BACKEND_API int __stdcall BackendCancel(be_state* bes);
	BACKEND_API int __stdcall BackendPostData(
		be_state* bes,
		long handle,				/*	送信先のハンドル				*/
		short status,				/*	ステータス						*/
		long message,				/*	アプリケーション定義メッセージ	*/
		long sendsize,				/*	送信データサイズ				*/
		const char* sendbuf);		/*	送信データ						*/
	BACKEND_API int __stdcall BackendPostDataByName(
		be_state* bes,
		LPCTSTR capability,			/*	送信先のハンドル				*/
		short status,				/*	ステータス						*/
		long message,				/*	アプリケーション定義メッセージ	*/
		long sendsize,				/*	送信データサイズ				*/
		const char* sendbuf);		/*	送信データ						*/

}


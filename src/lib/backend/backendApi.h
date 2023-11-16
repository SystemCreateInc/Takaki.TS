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
		LPCSTR capability,			/*	���M��̃L���p�r���e�B�[		*/
		long message,				/*	�A�v���P�[�V������`���b�Z�[�W	*/
		long sendsize,				/*	���M�f�[�^�T�C�Y				*/
		LPCSTR sendbuf,				/*	���M�f�[�^						*/
		long* recvsize,				/*	��M�f�[�^�T�C�Y				*/
		LPSTR* recvbuf				/*	��M�f�[�^						*/
	);

	BACKEND_API int __stdcall BackendGetMessage(be_state* bes, long* from_clientid, long* message, long* size, LPSTR* data);
	BACKEND_API int __stdcall BackendCancel(be_state* bes);
	BACKEND_API int __stdcall BackendPostData(
		be_state* bes,
		long handle,				/*	���M��̃n���h��				*/
		short status,				/*	�X�e�[�^�X						*/
		long message,				/*	�A�v���P�[�V������`���b�Z�[�W	*/
		long sendsize,				/*	���M�f�[�^�T�C�Y				*/
		const char* sendbuf);		/*	���M�f�[�^						*/
	BACKEND_API int __stdcall BackendPostDataByName(
		be_state* bes,
		LPCTSTR capability,			/*	���M��̃n���h��				*/
		short status,				/*	�X�e�[�^�X						*/
		long message,				/*	�A�v���P�[�V������`���b�Z�[�W	*/
		long sendsize,				/*	���M�f�[�^�T�C�Y				*/
		const char* sendbuf);		/*	���M�f�[�^						*/

}


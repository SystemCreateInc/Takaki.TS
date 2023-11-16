/*
 * CE support functions
 */

#include "ce.h"
#include <windows.h>
#include <winsock.h>
#include <shlwapi.h>

#pragma comment(lib, "winsock.lib")

#ifdef _WIN32_WCE

int GetPrivateProfileInt(LPCTSTR section, LPCTSTR entry, int defvalue, LPCTSTR path)	
{	
	return defvalue;
}

BOOL GetPrivateProfileString(LPCTSTR section, LPCTSTR entry, LPCTSTR defvalue, LPTSTR buffer, DWORD buffersize, LPCTSTR path)
{
	_tcsncpy(buffer, defvalue, buffersize);
	return FALSE;
}

BOOL MakeSureDirectoryPathExists(LPCTSTR path)
{
	return FALSE;
}

BOOL GetComputerName(LPTSTR cname, LPDWORD size)
{
	char buf[128];
	int len;

	if(gethostname(buf, sizeof buf))
		return FALSE;

	len = MultiByteToWideChar(CP_OEMCP, 0, buf, -1, cname, *size);
	*size = len;
	return TRUE;
}

DWORD SearchPath(LPCTSTR lpPath, LPCTSTR lpFileName, LPCTSTR lpExtension, DWORD nBufferLength, LPTSTR lpBuffer, LPTSTR *lpFilePart)
{
#if 1
	return 0;
#else
	WIN32_FIND_DATA ffd;
	TCHAR path[MAX_PATH];
	HANDLE fh;

	PathCombine(path, lpPath, lpFileName);

	fh = FindFirstFile(path, &ffd);
	if(fh == INVALID_HANDLE_VALUE)
		return 0;

	lstrcpy(lpBuffer, ffd.cFileName);
	*lpFilePart = PathFindFileName(lpBuffer);

	FindClose(fh);

	return 1;
#endif
}

#endif



#if !defined(__CE_H__) && defined(_WIN32_WCE)
#define __CE_H__

#include <tchar.h>

int GetPrivateProfileInt(LPCTSTR section, LPCTSTR entry, int defvalue, LPCTSTR path);
BOOL GetPrivateProfileString(LPCTSTR section, LPCTSTR entry, LPCTSTR defvalue, LPTSTR buffer, DWORD buffersize, LPCTSTR path);
BOOL MakeSureDirectoryPathExists(LPCTSTR path);
BOOL GetComputerName(LPTSTR cname, LPDWORD size);
DWORD SearchPath(LPCTSTR lpPath, LPCTSTR lpFileName, LPCTSTR lpExtension, DWORD nBufferLength, LPTSTR lpBuffer, LPTSTR *lpFilePart);



#endif // __CE_H__

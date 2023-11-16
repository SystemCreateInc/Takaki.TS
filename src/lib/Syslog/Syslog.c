// Syslog.cpp : DLL �A�v���P�[�V�����p�̃G���g�� �|�C���g���`���܂��B
//	$Id: Syslog.c 429 2006-06-01 09:34:24Z hiro $

#include <stdlib.h>
#include <stdarg.h>
#include <stdio.h>
#include <errno.h>
#include <io.h>
#include <fcntl.h>
#include <share.h>
#include <sys/stat.h>

#define WIN32_LEAN_AND_MEAN		// Windows �w�b�_�[����w�ǎg�p����Ȃ��X�^�b�t�����O���܂�
#include <tchar.h>
#include <windows.h>
#include <shellapi.h>
#ifndef _WIN32_WCE
#include <imagehlp.h>
#include <time.h>
#endif

#define	__COMMON_ONLY__
#include <common.h>
#include <inidef.h>
#include <Syslog.h>

#include <shlwapi.h>
#pragma comment(lib, "shlwapi.lib")

#include "ce.h"
#include "socket.h"

#pragma warning(disable:4996)

////////////////////////////////////////////////////////////////////////
//	Global function
//	�G���g���[

typedef struct {
	TCHAR	path[MAX_PATH];
	TCHAR	exename[MAX_PATH];
	TCHAR	version[64];
	TCHAR	creationdate[32];
	TCHAR	lastaccesstime[32];
	TCHAR	lastwritetime[32];
	TCHAR	special[64];
} MODULEINFO;

typedef struct {
	FILE*		logfile;
	TCHAR		logroot[MAX_PATH];					//	���O���[�g
	TCHAR		logpath[MAX_PATH];					//	���ۂ̃��O�������݃p�X
	TCHAR		logfilepath[MAX_PATH];				//	���O�t�@�C���p�X
	SYSTEMTIME	currentdate;						//	���݂̓��t
} LOGFILEINFO;

#define	MAX_LOG_FILES		10

static LOGFILEINFO	lfi[MAX_LOG_FILES];

//static FILE*	logfile;							//	���O�t�@�C���̃n���h��
static TCHAR	inipath[MAX_PATH];					//	�������t�@�C���̃p�X
static TCHAR	sysroot[MAX_PATH];					//	�V�X�e�����[�g
//static TCHAR	logroot[MAX_PATH];					//	���O���[�g
//static TCHAR	logpath[MAX_PATH];					//	���ۂ̃��O�������݃p�X
static TCHAR	logprefix[10];						//	���O�v���t�B�b�N�X
static TCHAR	logfilename[MAX_PATH];				//	���O�t�@�C����
//static TCHAR	logfilepath[MAX_PATH];				//	���O�t�@�C���p�X
static TCHAR	cname[256];							//	�R���s���[�^��
static TCHAR	exename[MAX_PATH];					//	EXE��
static UINT		expdays;							//	�L������
//static SYSTEMTIME	currentdate;					//	���݂̓��t
static int		flush;								//	�t���b�V���t���O
static int		logmask = SV_DEBUG;					//	�}�X�N
static __int64	splitsize;							//	�����T�C�Y
static int		logseq;								//	�t�@�C���V�[�P���X
static HANDLE	mutex;
static DWORD	processid = 0;
static __declspec(thread)	DWORD	threadid = 0;

/*
 *	�f�o�b�O���͍폜���Ȃ��Ă��ݔ��Ɉړ�����
 */
#ifdef _DEBUG
#	ifdef DeleteFile
#		undef DeleteFile
#	endif
#	define	DeleteFile		MoveToDustBox

#	ifdef RemoveDirectory
#		undef RemoveDirectory
#	endif
#	define	RemoveDirectory	MoveToDustBox
#endif

#ifdef _DEBUG
static int MoveToDustBox(LPCTSTR path)
{
	SHFILEOPSTRUCT fo;
	TCHAR from[_MAX_PATH];

	_tcscpy_s(from, sizeof from, path);
	from[lstrlen(path)+1] = '\0';

	fo.hwnd = NULL;
	fo.wFunc = FO_DELETE;
	fo.pFrom = from;
	fo.pTo = NULL;
	//fo.fFlags = FOF_ALLOWUNDO;	// �m�F�_�C�A���O���o���ꍇ
	fo.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;

	return !SHFileOperation(&fo);
}
#endif

LPCTSTR GetIniPath(void)
{
	return inipath;
}

LPCTSTR GetExeName(void)
{
	return exename;
}

LPCTSTR GetHostName(void)
{
	return cname;
}

void str_delete(LPTSTR buf, size_t bufsize, size_t pos, size_t count)
{
	//	NULL�܂Ŋ܂߂�̂�+1����
	size_t strsize = _tcslen(buf) + 1;
	size_t pos2 = pos + count;

	if (pos >= bufsize)
		return;

	//	�o�b�t�@�[���z�������͍̂Ō�܂ō폜����
	if (count == -1 || pos2 >= bufsize)
		pos2 = bufsize;
	else
		pos2;

	memmove(buf + pos, buf + pos2, sizeof (TCHAR) * (strsize - pos2));
}

void str_insert(LPTSTR buf, size_t bufsize, size_t pos, LPCTSTR str)
{
	size_t strsize = _tcslen(buf) + 1;
	size_t istrsize = _tcslen(str);

	if (pos >= bufsize)
		return;

	if (strsize + istrsize >= bufsize)
		return;

	memmove(buf + pos + istrsize, buf + pos, sizeof (TCHAR) * (strsize - pos));
	memcpy(buf + pos, str, sizeof (TCHAR) * istrsize);
}

void str_replace(LPTSTR buf, int bufsize, LPCTSTR src, LPCTSTR dst)
{
	LPTSTR p;

	while (p = _tcsstr(buf, src))
	{
		size_t pos = p - buf;

		str_delete(buf, bufsize, pos, _tcslen(src));
		str_insert(buf, bufsize, pos, dst);
	}
}

//
//	�v���t�@�C�����b�p�[
//
DWORD GetPrivateProfileString2(
    __in_opt LPCSTR lpAppName,
    __in_opt LPCSTR lpKeyName,
    __in_opt LPCSTR lpDefault,
    __out_ecount_part_opt(nSize, return + 1) LPSTR lpReturnedString,
    __in     DWORD nSize,
    __in_opt LPCSTR lpFileName
	)
{
	TCHAR cname[256];
	int size;
	DWORD namesize;

	size = GetPrivateProfileString(lpAppName, lpKeyName, lpDefault, lpReturnedString, nSize, lpFileName);

	namesize = _countof(cname);
	GetComputerName(cname, &namesize);

	str_replace(lpReturnedString, nSize, _T("${m}"), cname);
	str_replace(lpReturnedString, nSize, _T("${e}"), exename);

	return size;
}

////////////////////////////////////////////////////////////////////////
//	�f�B���N�g���폜
//
//	�V�X�e���t�@�C��������΍폜���Ȃ�
//
static int DeleteDirectory(LPCTSTR path)
{
	TCHAR findpath[_MAX_PATH], delpath[_MAX_PATH];
	WIN32_FIND_DATA ffd;
	HANDLE f;

	if(!path) return 0;

	//	�f�B���N�g���[���̃t�@�C��������
	PathCombine(findpath, path, _T("*.*"));
	if((f = FindFirstFile(findpath, &ffd)) == INVALID_HANDLE_VALUE)
		return 0;
	
	do
	{
		if(ffd.dwFileAttributes & FILE_ATTRIBUTE_SYSTEM)
			break;

		if(!lstrcmp(_T("."), ffd.cFileName) || !lstrcmp(_T(".."), ffd.cFileName))
			continue;

		PathCombine(delpath, path, ffd.cFileName);
		if(ffd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
		{
			//	�f�B���N�g���[�̏ꍇ���g���폜
			if(!DeleteDirectory(delpath))
			{
				FindClose(f);
				return 0;
			}
		}
		else if(!DeleteFile(delpath))
		{
			FindClose(f);
			return 0;
		}
	}
	while(FindNextFile(f, &ffd));
	FindClose(f);

	//	�ړI�f�B���N�g���[�폜
	return RemoveDirectory(path);
}

////////////////////////////////////////////////////////////////////////
//	���O�p�X�擾
//
static __inline LPTSTR GetLogPath(LOGFILEINFO* li)
{
	return li->logpath;
}

static void UpdateLogDir(LOGFILEINFO* li)
{
	TCHAR dir[MAX_PATH];

	GetLocalTime(&li->currentdate);

	//	�p�X�쐬
	wsprintf(dir, _T("%s%4d%02d%02d"),
			logprefix,
			li->currentdate.wYear,
			li->currentdate.wMonth,
			li->currentdate.wDay);

	PathCombine(li->logpath, li->logroot, dir);
}

////////////////////////////////////////////////////////////////////////
//	���O�t�@�C���I�[�v��
//
static int OpenLogFile(LOGFILEINFO* li)
{
	UpdateLogDir(li);

	PathCombine(li->logfilepath, GetLogPath(li), logfilename);
#ifndef UNICODE
	MakeSureDirectoryPathExists(li->logfilepath);
#endif

	li->logfile = _tfopen(li->logfilepath, _T("a+b"));

	return li->logfile ? 1 : 0;
}

////////////////////////////////////////////////////////////////////////
//	�t�@�C���o�[�W�����擾
//
static void FiletimeToStr(LPTSTR buf, FILETIME *ft)
{
	SYSTEMTIME st;

	FileTimeToSystemTime(ft, &st);
	wsprintf(buf, _T("%04d-%02d-%02d %02d:%02d:%02d"),
		st.wYear,
		st.wMonth,
		st.wDay,	
		st.wHour,
		st.wMinute,
		st.wSecond);
}

static int GetModuleInfo(HMODULE hm, MODULEINFO *mi)
{
	TCHAR *version, *value;
	DWORD dwHandle;
	HANDLE hf;
	UINT size;
	WIN32_FIND_DATA ffd;
	VS_FIXEDFILEINFO* info;

	//	���̎擾
	GetModuleFileName(hm, mi->path, _countof(mi->path));

	//	�o�[�W�������擾
	size = GetFileVersionInfoSize(mi->path, &dwHandle);
	if((version = malloc(size)) == NULL)
		return 0;

	if(GetFileVersionInfo(mi->path, dwHandle, size, version))
	{
		//	�o�[�W����
		VerQueryValue(version, _T("\\"), &value, &size);
		info = (VS_FIXEDFILEINFO*)value;
		_stprintf_s(mi->version, _countof(mi->version), _T("%d.%d.%d.%d"), 
			HIWORD(info->dwFileVersionMS),
			LOWORD(info->dwFileVersionMS),
			HIWORD(info->dwFileVersionLS),
			LOWORD(info->dwFileVersionLS));

		//	�X�y�V�����r���h
		VerQueryValue(version, _T("\\041104b0"), &value, &size);
		if (size)
			_tcscpy_s(mi->special, min(sizeof mi->special / sizeof (TCHAR), size), value);
	}
	free(version);

	//	���t���擾
	if((hf = FindFirstFile(mi->path, &ffd)) != INVALID_HANDLE_VALUE)
	{
		_tcscpy_s(mi->exename, sizeof mi->exename / sizeof (TCHAR), ffd.cFileName);
		FiletimeToStr(mi->creationdate,		&ffd.ftCreationTime);
		FiletimeToStr(mi->lastaccesstime,	&ffd.ftLastAccessTime);
		FiletimeToStr(mi->lastwritetime,	&ffd.ftLastWriteTime);
		FindClose(hf);
	}

	return 1;
}

////////////////////////////////////////////////////////////////////////
//
//	OS���o��
//
static void OutOSInfo(void)
{
	LPCTSTR platform = _T("UNKNOWN");
	OSVERSIONINFO osv;
	DWORD dwMajor, dwMinor, dwBuild;

	//	�o�[�W�������擾
	osv.dwOSVersionInfoSize = sizeof (osv);
	GetVersionEx(&osv);

	const HMODULE hModule = GetModuleHandle(TEXT("ntdll.dll"));
	if (hModule)
	{
		typedef void (WINAPI* fnRtlGetVersion)(OSVERSIONINFOEXW*);
		fnRtlGetVersion RtlGetVersion = (fnRtlGetVersion)GetProcAddress(hModule, "RtlGetVersion");
		if (RtlGetVersion)
		{
			OSVERSIONINFOEXW osw = { sizeof(OSVERSIONINFOEXW) };
			RtlGetVersion(&osw);
			osv.dwMajorVersion = osw.dwMajorVersion;
			osv.dwMinorVersion = osw.dwMinorVersion;
			osv.dwBuildNumber = osw.dwBuildNumber;
		}
	}


	TCHAR szReleaseID[32] = { 0 };
	TCHAR szUBR[32] = { 0 };
	HKEY hKey;
	if (RegOpenKeyEx(HKEY_LOCAL_MACHINE, TEXT("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion"), 0, KEY_READ | KEY_WOW64_64KEY, &hKey) == ERROR_SUCCESS)
	{
		DWORD dwType = REG_SZ;
		DWORD dwByte = sizeof(szReleaseID);
		if (ERROR_SUCCESS == RegQueryValueEx(hKey, TEXT("ReleaseId"), 0, &dwType, (LPBYTE)&szReleaseID, &dwByte))
		{
			lstrcat(szReleaseID, TEXT(" "));
		}
		dwType = REG_DWORD;
		dwByte = sizeof(DWORD);
		DWORD dwUBR = 0;
		if (ERROR_SUCCESS == RegQueryValueEx(hKey, TEXT("UBR"), 0, &dwType, (LPBYTE)&dwUBR, &dwByte))
		{
			wsprintf(szUBR, TEXT(".%d"), dwUBR);
		}
		RegCloseKey(hKey);
	}

	switch(osv.dwPlatformId)
	{
	case VER_PLATFORM_WIN32_NT:
		platform = _T("WindowsNT");
		dwMajor = osv.dwMajorVersion;
		dwMinor = osv.dwMinorVersion;
		dwBuild = osv.dwBuildNumber;
		break;

	case VER_PLATFORM_WIN32s:	
	case VER_PLATFORM_WIN32_WINDOWS:
		platform = _T("Windows");
		dwBuild = osv.dwBuildNumber & 0x0000ffff;
		dwMajor = (osv.dwBuildNumber & 0x00ff0000) >> 16;
		dwMinor = (osv.dwBuildNumber & 0xff000000) >> 24;
		break;

#ifdef _WIN32_WCE
	case VER_PLATFORM_WIN32_CE:
		platform = _T("WindowsCE");
#endif

	default:
		dwMajor = osv.dwMajorVersion;
		dwMinor = osv.dwMinorVersion;
		dwBuild = osv.dwBuildNumber;
	};

	SLPrintf(SV_INFO, _T("-- %s %d %d %s build:%d%s %s"),
		platform,
		dwMajor,
		dwMinor,
		szReleaseID,
		dwBuild,
		szUBR,
		osv.szCSDVersion);
}

////////////////////////////////////////////////////////////////////////
//
//	���W���[���ꗗ�o��
//
typedef BOOL (WINAPI *DEF_EPM)(HANDLE, HMODULE*, DWORD, LPDWORD);
typedef BOOL (WINAPI *DEF_GMN)(HANDLE, HMODULE, LPTSTR, DWORD);
static DEF_EPM EnumProcessModules;
static DEF_GMN GetModuleFileNameEx;

static void OutModuleList(void)
{
	DWORD i, count, dwNeeded;
	HMODULE mods[1024];
	HINSTANCE hPsapi;
	HANDLE hProc;
	MODULEINFO mi;

	//	PSAPI.DLL�g�p
	if((hPsapi = LoadLibrary(_T("psapi.dll"))) == NULL)
		return;

	EnumProcessModules = (DEF_EPM)GetProcAddress(hPsapi, "EnumProcessModules");
#ifdef UNICODE
	GetModuleFileNameEx = (DEF_GMN)GetProcAddress(hPsapi, "GetModuleFileNameExW");
#else
	GetModuleFileNameEx = (DEF_GMN)GetProcAddress(hPsapi, _T("GetModuleFileNameExA"));
#endif

	//	�ꗗ�擾
	hProc = GetCurrentProcess();
	if(EnumProcessModules && GetModuleFileNameEx &&
		EnumProcessModules(hProc, mods, sizeof mods, &dwNeeded))
	{
		SLPrintf(SV_INFO, _T("--- CreationDate        LastWriteTime       LastAccessTime      ModulePath Version/Special"));
		count = dwNeeded / sizeof (HMODULE);
		for(i = 0; i < count; i++)
		{
			ZeroMemory(&mi, sizeof mi);
			GetModuleInfo(mods[i], &mi);
			SLPrintf(SV_INFO, _T("--- %s %s %s %s    %s/%s"),
				mi.creationdate, mi.lastwritetime, mi.lastaccesstime,
				mi.path,
				mi.version,
				mi.special);
		}
	}

	FreeLibrary(hPsapi);
}

////////////////////////////////////////////////////////////////////////
//
//	���������傫����Ε�������
//	�������@�͊����̃t�@�C�����R�s�[���Ċ����̃t�@�C����0�ɂ���
//
__inline static void SplitCheck(LOGFILEINFO* li)
{
	__int64 size;
//	static int splitting;
	
	if (/*splitting || */!splitsize)
		return;

	size = _filelengthi64(fileno(li->logfile));

	if (size < splitsize)
		return;

//	splitting = 1;

	fflush(li->logfile);

	if (SLCopy(li->logfilepath))
	{
		//	�t�@�C���T�C�Y��0�ɂ���
		//	SLCopy�ł̃��O�o�͂͂��ׂĖ����Ȃ�
		_chsize_s(fileno(li->logfile), 0);
	}

//	splitting = 0;
}

////////////////////////////////////////////////////////////////////////
//
//	���O�t�@�C���`�F�b�N�@�I�[�v���A�����`�F�b�N�Ȃ�
//
static int CheckLogFile(SYSTEMTIME* st, LOGFILEINFO* li)
{
	//	���t���ς���Ă����烍�O�t�@�C����ύX����
	if(li->logfile == NULL 
		|| (st->wDay != li->currentdate.wDay
			|| st->wMonth != li->currentdate.wMonth
			|| st->wYear != li->currentdate.wYear))
	{
		if (li->logfile)
		{
			fclose(li->logfile);
			li->logfile = NULL;
		}

		if(!OpenLogFile(li))
			return 0;
	}

	//	�����`�F�b�N
	if (splitsize)
		SplitCheck(li);

	return 1;
}

////////////////////////////////////////////////////////////////////////
//
//	�X�^�[�g���o��
//
static void OutStartInfo(void)
{
	OutOSInfo();
	OutModuleList();
}

////////////////////////////////////////////////////////////////////////
//	������
//	
static void Initialize(void)
{
	int i;
	LPTSTR fname;
	TCHAR log[MAX_PATH], tmp[MAX_PATH];
	MODULEINFO mi;
	DWORD size;

	//	�R���s���[�^���擾
	size = _countof(cname);
	GetComputerName(cname, &size);

	//	���C����EXE���擾
	ZeroMemory(&mi, sizeof mi);
	GetModuleInfo(GetModuleHandle(NULL), &mi);
	lstrcpy(exename, mi.exename);

	//	ini̧�ق�T��
#ifndef _WIN32_WCE
	if(!SearchPath(NULL, SYSTEMINIFILE,	NULL, _countof(inipath), inipath, &fname))
#else
	lstrcpy(tmp, mi.path);
	PathRemoveFileSpec(tmp);
	if (!SearchPath(tmp, _T(SYSTEMINIFILE), NULL, _countof(inipath), inipath, &fname))
#endif
	{
		ZeroMemory(inipath, sizeof inipath);
	}

	//	�ۑ�����
	expdays = GetPrivateProfileInt(INI_SYSLOG, INI_EXPDAYS, 30, inipath);

	//	�t���b�V���t���O
	flush = GetPrivateProfileInt(INI_SYSLOG, INI_FLUSH, 1, inipath);

	//	�����T�C�Y
	GetPrivateProfileString2(INI_SYSLOG, INI_SPLITSIZE, _T("0"), tmp, _countof(tmp), inipath);
	splitsize = _atoi64(tmp) * 1024 * 1024;

	//	�V�X�e�����[�g
	if(!GetPrivateProfileString2(INI_GLOBAL, INI_ROOT, _T(""), sysroot, _countof(sysroot), inipath))
	{
		MODULEINFO mi;
		GetModuleInfo(GetModuleHandle(NULL), &mi);
		PathRemoveFileSpec(mi.path);
		lstrcpy(sysroot, mi.path);
	}

	//	�v���t�B�b�N�X
	GetPrivateProfileString2(INI_SYSLOG, INI_PREFIX, _T("DL"), logprefix, _countof(logprefix), inipath);

	//	�t�@�C����
	GetPrivateProfileString2(INI_SYSLOG, INI_FILE, _T("log.txt"), logfilename, _countof(logfilename), inipath);

	for (i = 0; i < MAX_LOG_FILES; i++)
	{
		lfi[i].logfile = NULL;

		if (!i)
			_tcscpy_s(tmp, _countof(tmp), INI_ROOT);
		else
			_stprintf_s(tmp, _countof(tmp), "%s%d", INI_ROOT, i);

		//	���O���[�g
		log[0] = 0;
		GetPrivateProfileString2(INI_SYSLOG, tmp, i ? _T("") : _T("log"), log, _countof(log), inipath);

		if (strlen(log))
			PathCombine(lfi[i].logroot, sysroot, log);
		else
			lfi[i].logroot[0] = 0;
	}

	InitializeSocket();

	mutex = CreateMutex(NULL, FALSE, "Systemcreate.Syslog");

	processid = GetCurrentProcessId();
}

////////////////////////////////////////////////////////////////////////
//	�I��
//	
static void Terminate(void)
{
	int i;

	SLPrintf(SV_INFO, _T("------------- EXIT"));

	for (i = 0; i < MAX_LOG_FILES; i++)
	{
		if (lfi[i].logfile)
			fclose(lfi[i].logfile);
	}

	TerminateSocket();

	CloseHandle(mutex);
}

////////////////////////////////////////////////////////////////////////
//	������
//
DWORD WINAPI SLInit(SLINIT *si)
{
	//	�p�����[�^���^�����ĂȂ���΃f�t�H���g���擾����
	if(si) 
	{
		expdays = si->expdays;
		_tcscpy_s(lfi[0].logroot, sizeof lfi[0].logroot / sizeof (TCHAR), si->logroot);
	}

	//	���O�폜
	if(!si || si->trunc) SLTruncExpiredLog();

	SLPrintf(SV_INFO, _T("------------- START"));
	OutStartInfo();

	return 1;
}

////////////////////////////////////////////////////////////////////////
//	���O��������
//
DWORD WINAPI SLWrite(LPCTSTR format, ...)
{
	DWORD rc;
	va_list marker;
	va_start(marker, format);
	rc = SLWriteArg(format, marker);
	va_end(marker);
	return rc;
}

DWORD WINAPI SLWriteArg(LPCTSTR format, va_list marker)
{
	return SLVPrintf(SV_INFO, format, marker);
}

////////////////////////////////////////////////////////////////////////
//	���O�������݃v���C�I���e�B�[�t��
//	

void WINAPI SLSetLoglevel(int severity)
{
	logmask = severity;
}

static LPCTSTR severity_strs[] = {
	_T("EMERG"),
	_T("ALERT"),
	_T("CRIT"),
	_T("ERR"),
	_T("WARN"),
	_T("NOTICE"),
	_T("INFO"),
	_T("DEBUG"),
};

DWORD WINAPI SLPrintf(int severity, LPCTSTR format, ...)
{
	DWORD rc;
	va_list marker;
	va_start(marker, format);
	rc = SLVPrintf(severity, format, marker);
	va_end(marker);
	return rc;
}

DWORD WINAPI SLVPrintf(int severity, LPCTSTR format, va_list marker)
{
	int cnt = 0, i;
	SYSTEMTIME st;
	TCHAR header[128];

	if(severity > logmask)
		return 0;

	WaitForSingleObject(mutex, INFINITE);

	if (!threadid)
		threadid = GetCurrentThreadId();

	//	�w�b�_�[��������
	GetLocalTime(&st);
	cnt = _stprintf_s(header, _countof(header), _T("%04d-%02d-%02d %02d:%02d:%02d.%03d %s %s[%lu-%lu] %s:"),
			st.wYear,
			st.wMonth,
			st.wDay,
			st.wHour,
			st.wMinute,
			st.wSecond,
			st.wMilliseconds,
			cname,
			exename,
			processid,
			threadid,
			severity_strs[severity]);


	SyslogWrite(severity, header, format, marker);

	for (i = 0; i < MAX_LOG_FILES; i++)
	{
		LOGFILEINFO* li = &lfi[i];

		//	�ݒ�̖������̂̓X�L�b�v����
		if (!li->logroot[0])
			break;

		if (!CheckLogFile(&st, li))
			continue;

		//	�ʏ탍�O��������
		_fputts(header, li->logfile);
		int written = _vftprintf(li->logfile, format, marker);
		if (written == EOF)
		{
			fclose(li->logfile);
			li->logfile = NULL;
			continue;
		}

		cnt += written;
		_fputts(_T("\r\n"), li->logfile);

		if (flush)
		{
			if (fflush(li->logfile) == EOF)
			{
				//	�t�@�C�����J���Ȃ���
				fclose(li->logfile);
				li->logfile = NULL;
			}
		}
	}

	ReleaseMutex(mutex);

	return cnt;
}

DWORD WINAPI SLRawWrite(int severity, LPCTSTR str)
{
	size_t cnt = 0, i;
	SYSTEMTIME st;

	if(severity > logmask)
		return 0;

	WaitForSingleObject(mutex, INFINITE);

	//	���t���ς���Ă����烍�O�t�@�C����ύX����
	GetLocalTime(&st);

	for (i = 0; i < MAX_LOG_FILES; i++)
	{
		LOGFILEINFO* li = &lfi[i];

		//	�ݒ�̖������̂̓X�L�b�v����
		if (!li->logroot[0])
			break;

		if (!CheckLogFile(&st, li))
			continue;

		//	���O��������
		cnt = fwrite(str, _tcslen(str), sizeof (TCHAR), li->logfile);

		if(flush)
			fflush(li->logfile);
	}

	ReleaseMutex(mutex);

	return (DWORD)cnt;
}

////////////////////////////////////////////////////////////////////////
//	���O�̈�Ƀt�@�C���R�s�[
//	���Ԃ̊g���q��t�����ăR�s�[����
//
DWORD WINAPI SLCopy(LPCTSTR src)
{
	int i;
	DWORD err;
	TCHAR path[MAX_PATH], fname[MAX_PATH];
	SYSTEMTIME st;

	GetLocalTime(&st);

	PathCombine(path, GetLogPath(&lfi[0]), PathFindFileName(src));
#ifndef UNICODE
	MakeSureDirectoryPathExists(path);
#endif

	for (i = 0; i < 1000; i++)
	{
		wsprintf(fname, _T("%s.%4d%02d%02d-%02d%02d%02d-%d"),
			path,
			st.wYear,
			st.wMonth,
			st.wDay,
			st.wHour,
			st.wMinute,
			st.wSecond,
			i);

		if(CopyFile(src, fname, TRUE))
		{
			SLPrintf(SV_INFO, _T("�t�@�C�����o�b�N�A�b�v���܂����B[%s]->[%s]"),src, path);
			return 1;
		}
	}

	err = GetLastError();
	SLPrintf(SV_WARN, _T("�R�s�[�Ɏ��s���܂����B[%s]->[%s] [%d]"),src, path, err);

	return 1;
}

////////////////////////////////////////////////////////////////////////
//	�����̐؂ꂽ���O���폜
//
DWORD WINAPI  SLTruncExpiredLog(void)
{
#ifndef _WIN32_WCE
	//	�����w�胍�O�폜
	TCHAR findpath[_MAX_PATH], point[_MAX_PATH], path[_MAX_PATH], tmp[_MAX_PATH];
	time_t now;
	struct tm tmnow;
	HANDLE f;
	WIN32_FIND_DATA ffd;

	time(&now);

	localtime_s(&tmnow, &now);
	tmnow.tm_mday -= expdays;
	mktime(&tmnow);

	//	��̃t�@�C�������쐬
	_stprintf_s(point, _countof(point), _T("%s%04d%02d%02d"),
		logprefix,
		tmnow.tm_year + 1900,
		tmnow.tm_mon + 1,
		tmnow.tm_mday);

	//	���O�t�H���_�[�����C���h�J�[�h�Ō���
	_stprintf_s(tmp, _countof(tmp), _T("%s*"), logprefix);

	PathCombine(findpath, lfi[0].logroot, tmp);

	if((f = FindFirstFile(findpath, &ffd)) == INVALID_HANDLE_VALUE)
		return 1;

	do
	{
		if(!(ffd.dwFileAttributes & FILE_ATTRIBUTE_SYSTEM)
			&& ffd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY
			&& lstrcmp(ffd.cFileName, point) < 0)
		{
			PathCombine(path, lfi[0].logroot, ffd.cFileName);
			if(DeleteDirectory(path))
			{
				SLWrite(_T("�����؂ꃍ�O���폜���܂����B [%s]"), path);
			}
			else
			{
				SLWrite(_T("�����؂ꃍ�O���폜�o���܂���ł����B[%d][%s]"), 
					GetLastError(),
					path);
			}
		}
	}
	while(FindNextFile(f, &ffd));
	FindClose(f);
#endif

	return 1;
}

////////////////////////////////////////////////////////////////////////
//	�p�����[�^�擾
//
void WINAPI SLGetParams(SLINIT* si)
{
	lstrcpy(si->logroot, lfi[0].logroot);
	si->expdays = expdays;
}

////////////////////////////////////////////////////////////////////////
//	the main
//

BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
	switch(ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		Initialize();
		break;

	case DLL_PROCESS_DETACH:
		Terminate();
		break;
	}

    return TRUE;
}

// Syslog.cpp : DLL アプリケーション用のエントリ ポイントを定義します。
//	$Id: Syslog.c 429 2006-06-01 09:34:24Z hiro $

#include <stdlib.h>
#include <stdarg.h>
#include <stdio.h>
#include <errno.h>
#include <io.h>
#include <fcntl.h>
#include <share.h>
#include <sys/stat.h>

#define WIN32_LEAN_AND_MEAN		// Windows ヘッダーから殆ど使用されないスタッフを除外します
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
//	エントリー

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
	TCHAR		logroot[MAX_PATH];					//	ログルート
	TCHAR		logpath[MAX_PATH];					//	実際のログ書き込みパス
	TCHAR		logfilepath[MAX_PATH];				//	ログファイルパス
	SYSTEMTIME	currentdate;						//	現在の日付
} LOGFILEINFO;

#define	MAX_LOG_FILES		10

static LOGFILEINFO	lfi[MAX_LOG_FILES];

//static FILE*	logfile;							//	ログファイルのハンドル
static TCHAR	inipath[MAX_PATH];					//	初期化ファイルのパス
static TCHAR	sysroot[MAX_PATH];					//	システムルート
//static TCHAR	logroot[MAX_PATH];					//	ログルート
//static TCHAR	logpath[MAX_PATH];					//	実際のログ書き込みパス
static TCHAR	logprefix[10];						//	ログプレフィックス
static TCHAR	logfilename[MAX_PATH];				//	ログファイル名
//static TCHAR	logfilepath[MAX_PATH];				//	ログファイルパス
static TCHAR	cname[256];							//	コンピュータ名
static TCHAR	exename[MAX_PATH];					//	EXE名
static UINT		expdays;							//	有効期限
//static SYSTEMTIME	currentdate;					//	現在の日付
static int		flush;								//	フラッシュフラグ
static int		logmask = SV_DEBUG;					//	マスク
static __int64	splitsize;							//	分割サイズ
static int		logseq;								//	ファイルシーケンス
static HANDLE	mutex;
static DWORD	processid = 0;
static __declspec(thread)	DWORD	threadid = 0;

/*
 *	デバッグ時は削除しなくてごみ箱に移動する
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
	//fo.fFlags = FOF_ALLOWUNDO;	// 確認ダイアログを出す場合
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
	//	NULLまで含めるので+1する
	size_t strsize = _tcslen(buf) + 1;
	size_t pos2 = pos + count;

	if (pos >= bufsize)
		return;

	//	バッファーを越えたものは最後まで削除する
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
//	プロファイルラッパー
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
//	ディレクトリ削除
//
//	システムファイルがあれば削除しない
//
static int DeleteDirectory(LPCTSTR path)
{
	TCHAR findpath[_MAX_PATH], delpath[_MAX_PATH];
	WIN32_FIND_DATA ffd;
	HANDLE f;

	if(!path) return 0;

	//	ディレクトリー内のファイルを検索
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
			//	ディレクトリーの場合中身を削除
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

	//	目的ディレクトリー削除
	return RemoveDirectory(path);
}

////////////////////////////////////////////////////////////////////////
//	ログパス取得
//
static __inline LPTSTR GetLogPath(LOGFILEINFO* li)
{
	return li->logpath;
}

static void UpdateLogDir(LOGFILEINFO* li)
{
	TCHAR dir[MAX_PATH];

	GetLocalTime(&li->currentdate);

	//	パス作成
	wsprintf(dir, _T("%s%4d%02d%02d"),
			logprefix,
			li->currentdate.wYear,
			li->currentdate.wMonth,
			li->currentdate.wDay);

	PathCombine(li->logpath, li->logroot, dir);
}

////////////////////////////////////////////////////////////////////////
//	ログファイルオープン
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
//	ファイルバージョン取得
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

	//	名称取得
	GetModuleFileName(hm, mi->path, _countof(mi->path));

	//	バージョン情報取得
	size = GetFileVersionInfoSize(mi->path, &dwHandle);
	if((version = malloc(size)) == NULL)
		return 0;

	if(GetFileVersionInfo(mi->path, dwHandle, size, version))
	{
		//	バージョン
		VerQueryValue(version, _T("\\"), &value, &size);
		info = (VS_FIXEDFILEINFO*)value;
		_stprintf_s(mi->version, _countof(mi->version), _T("%d.%d.%d.%d"), 
			HIWORD(info->dwFileVersionMS),
			LOWORD(info->dwFileVersionMS),
			HIWORD(info->dwFileVersionLS),
			LOWORD(info->dwFileVersionLS));

		//	スペシャルビルド
		VerQueryValue(version, _T("\\041104b0"), &value, &size);
		if (size)
			_tcscpy_s(mi->special, min(sizeof mi->special / sizeof (TCHAR), size), value);
	}
	free(version);

	//	日付を取得
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
//	OS情報出力
//
static void OutOSInfo(void)
{
	LPCTSTR platform = _T("UNKNOWN");
	OSVERSIONINFO osv;
	DWORD dwMajor, dwMinor, dwBuild;

	//	バージョン情報取得
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
//	モジュール一覧出力
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

	//	PSAPI.DLL使用
	if((hPsapi = LoadLibrary(_T("psapi.dll"))) == NULL)
		return;

	EnumProcessModules = (DEF_EPM)GetProcAddress(hPsapi, "EnumProcessModules");
#ifdef UNICODE
	GetModuleFileNameEx = (DEF_GMN)GetProcAddress(hPsapi, "GetModuleFileNameExW");
#else
	GetModuleFileNameEx = (DEF_GMN)GetProcAddress(hPsapi, _T("GetModuleFileNameExA"));
#endif

	//	一覧取得
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
//	分割数より大きければ分割する
//	分割方法は既存のファイルをコピーして既存のファイルを0にする
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
		//	ファイルサイズを0にする
		//	SLCopyでのログ出力はすべて無くなる
		_chsize_s(fileno(li->logfile), 0);
	}

//	splitting = 0;
}

////////////////////////////////////////////////////////////////////////
//
//	ログファイルチェック　オープン、分割チェックなど
//
static int CheckLogFile(SYSTEMTIME* st, LOGFILEINFO* li)
{
	//	日付が変わっていたらログファイルを変更する
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

	//	分割チェック
	if (splitsize)
		SplitCheck(li);

	return 1;
}

////////////////////////////////////////////////////////////////////////
//
//	スタート情報出力
//
static void OutStartInfo(void)
{
	OutOSInfo();
	OutModuleList();
}

////////////////////////////////////////////////////////////////////////
//	初期化
//	
static void Initialize(void)
{
	int i;
	LPTSTR fname;
	TCHAR log[MAX_PATH], tmp[MAX_PATH];
	MODULEINFO mi;
	DWORD size;

	//	コンピュータ名取得
	size = _countof(cname);
	GetComputerName(cname, &size);

	//	メインのEXE名取得
	ZeroMemory(&mi, sizeof mi);
	GetModuleInfo(GetModuleHandle(NULL), &mi);
	lstrcpy(exename, mi.exename);

	//	iniﾌｧｲﾙを探す
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

	//	保存期間
	expdays = GetPrivateProfileInt(INI_SYSLOG, INI_EXPDAYS, 30, inipath);

	//	フラッシュフラグ
	flush = GetPrivateProfileInt(INI_SYSLOG, INI_FLUSH, 1, inipath);

	//	分割サイズ
	GetPrivateProfileString2(INI_SYSLOG, INI_SPLITSIZE, _T("0"), tmp, _countof(tmp), inipath);
	splitsize = _atoi64(tmp) * 1024 * 1024;

	//	システムルート
	if(!GetPrivateProfileString2(INI_GLOBAL, INI_ROOT, _T(""), sysroot, _countof(sysroot), inipath))
	{
		MODULEINFO mi;
		GetModuleInfo(GetModuleHandle(NULL), &mi);
		PathRemoveFileSpec(mi.path);
		lstrcpy(sysroot, mi.path);
	}

	//	プレフィックス
	GetPrivateProfileString2(INI_SYSLOG, INI_PREFIX, _T("DL"), logprefix, _countof(logprefix), inipath);

	//	ファイル名
	GetPrivateProfileString2(INI_SYSLOG, INI_FILE, _T("log.txt"), logfilename, _countof(logfilename), inipath);

	for (i = 0; i < MAX_LOG_FILES; i++)
	{
		lfi[i].logfile = NULL;

		if (!i)
			_tcscpy_s(tmp, _countof(tmp), INI_ROOT);
		else
			_stprintf_s(tmp, _countof(tmp), "%s%d", INI_ROOT, i);

		//	ログルート
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
//	終了
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
//	初期化
//
DWORD WINAPI SLInit(SLINIT *si)
{
	//	パラメータが与えられてなければデフォルトを取得する
	if(si) 
	{
		expdays = si->expdays;
		_tcscpy_s(lfi[0].logroot, sizeof lfi[0].logroot / sizeof (TCHAR), si->logroot);
	}

	//	ログ削除
	if(!si || si->trunc) SLTruncExpiredLog();

	SLPrintf(SV_INFO, _T("------------- START"));
	OutStartInfo();

	return 1;
}

////////////////////////////////////////////////////////////////////////
//	ログ書き込み
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
//	ログ書き込みプライオリティー付き
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

	//	ヘッダー書き込み
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

		//	設定の無いものはスキップする
		if (!li->logroot[0])
			break;

		if (!CheckLogFile(&st, li))
			continue;

		//	通常ログ書き込み
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
				//	ファイルを開きなおす
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

	//	日付が変わっていたらログファイルを変更する
	GetLocalTime(&st);

	for (i = 0; i < MAX_LOG_FILES; i++)
	{
		LOGFILEINFO* li = &lfi[i];

		//	設定の無いものはスキップする
		if (!li->logroot[0])
			break;

		if (!CheckLogFile(&st, li))
			continue;

		//	ログ書き込み
		cnt = fwrite(str, _tcslen(str), sizeof (TCHAR), li->logfile);

		if(flush)
			fflush(li->logfile);
	}

	ReleaseMutex(mutex);

	return (DWORD)cnt;
}

////////////////////////////////////////////////////////////////////////
//	ログ領域にファイルコピー
//	時間の拡張子を付加してコピーする
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
			SLPrintf(SV_INFO, _T("ファイルをバックアップしました。[%s]->[%s]"),src, path);
			return 1;
		}
	}

	err = GetLastError();
	SLPrintf(SV_WARN, _T("コピーに失敗しました。[%s]->[%s] [%d]"),src, path, err);

	return 1;
}

////////////////////////////////////////////////////////////////////////
//	期限の切れたログを削除
//
DWORD WINAPI  SLTruncExpiredLog(void)
{
#ifndef _WIN32_WCE
	//	期日指定ログ削除
	TCHAR findpath[_MAX_PATH], point[_MAX_PATH], path[_MAX_PATH], tmp[_MAX_PATH];
	time_t now;
	struct tm tmnow;
	HANDLE f;
	WIN32_FIND_DATA ffd;

	time(&now);

	localtime_s(&tmnow, &now);
	tmnow.tm_mday -= expdays;
	mktime(&tmnow);

	//	基準のファイル名を作成
	_stprintf_s(point, _countof(point), _T("%s%04d%02d%02d"),
		logprefix,
		tmnow.tm_year + 1900,
		tmnow.tm_mon + 1,
		tmnow.tm_mday);

	//	ログフォルダーをワイルドカードで検索
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
				SLWrite(_T("期限切れログを削除しました。 [%s]"), path);
			}
			else
			{
				SLWrite(_T("期限切れログを削除出来ませんでした。[%d][%s]"), 
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
//	パラメータ取得
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

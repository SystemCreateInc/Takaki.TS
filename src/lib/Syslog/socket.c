#include <stdio.h>
#include <tchar.h>
#define	__COMMON_ONLY__
#include <inidef.h>

#if !defined(_WIN32_WCE)
#include <crtdbg.h>
#include <winsock2.h>
#pragma comment(lib, "ws2_32.lib")
#else
#include <winsock.h>
#endif

#include "socket.h"
#include "ce.h"

static int					facility = 1;
static SOCKET				sock = INVALID_SOCKET;				//	ソケット
static struct sockaddr_in	target;
static int					startupped;

extern LPCTSTR GetHostName();
extern LPCTSTR GetExeName();
extern LPCTSTR GetIniPath();

static void cleanup(void)
{
	if(startupped)
		WSACleanup();
}

//
//	ソケット準備
//
void InitializeSocket(void)
{
	int port = 514, usetcp = 0;
	BOOL val;
	TCHAR transmitto[128];
	char ctransmitto[128];
	WSADATA wsaData;
	struct linger linger;
	struct hostent *he;

	GetPrivateProfileString(INI_SYSLOG, INI_TRANSMITTO, _T(""), transmitto, sizeof transmitto / sizeof (TCHAR), GetIniPath());
	port	= GetPrivateProfileInt(INI_SYSLOG, INI_TRANSMITPORT, 514, GetIniPath());
	usetcp	= GetPrivateProfileInt(INI_SYSLOG, INI_USETCP, 1, GetIniPath());

	if(_tcslen(transmitto) == 0)
		return;

	if(WSAStartup(MAKEWORD(1, 0), &wsaData) != 0)
		return;

	startupped = TRUE;

	//	送信先のアドレスを取得する
#ifdef UNICODE
	WideCharToMultiByte(CP_OEMCP, 0, transmitto, -1, ctransmitto, sizeof ctransmitto, 0, 0);
#else
	_tcscpy_s(ctransmitto, sizeof ctransmitto, transmitto);
#endif
	memset(&target, 0, sizeof target);
	target.sin_family = AF_INET;
	target.sin_port = port;

	target.sin_addr.s_addr = inet_addr(ctransmitto);
	if(target.sin_addr.s_addr == INADDR_NONE)
	{
		he = gethostbyname(ctransmitto);
		if(he == NULL)
			return;

		target.sin_addr.s_addr = *(u_long*)he->h_addr_list[0];
	}


	sock = socket(AF_INET, usetcp ? SOCK_STREAM : SOCK_DGRAM, usetcp ? IPPROTO_TCP : IPPROTO_UDP);
	if(sock == INVALID_SOCKET)
		return;

	if(usetcp)
	{
		struct sockaddr_in local;
		memset(&local, 0, sizeof local);
		local.sin_family	= AF_INET;
		local.sin_port		= 0;
		local.sin_addr.s_addr = INADDR_ANY;
		if(bind(sock, (const struct sockaddr*)&local, sizeof local) == SOCKET_ERROR
			|| connect(sock, (const struct sockaddr*)&target, sizeof target) == SOCKET_ERROR)
		{
#ifndef _WIN32_WCE
			_RPT1(_CRT_WARN, "FAIL connect to syslogd by tcp %d", WSAGetLastError());
#endif
			closesocket(sock);
			sock = INVALID_SOCKET;
			return;
		}

		
		val = 1;
//		setsockopt(sock, IPPROTO_TCP, TCP_NODELAY, (const char*)&val, sizeof val);
	}

	linger.l_onoff	= TRUE;
	linger.l_linger = 0;
	setsockopt(sock, SOL_SOCKET, SO_LINGER, (const char*)&linger, sizeof linger);
}

void TerminateSocket(void)
{
	if(sock != INVALID_SOCKET)
	{
		closesocket(sock);
		cleanup();
	}
}

static void SendToSyslog(const char* str, int size)
{
	int rc = sendto(sock, str, size, 0, (const struct sockaddr*)&target, sizeof (struct sockaddr_in));
	if (rc == SOCKET_ERROR)
	{
		_RPT1(_CRT_WARN, "error %d\n", GetLastError());
	}
}

void SyslogWrite(int severity, LPCTSTR header, LPCTSTR format, va_list marker)
{
	TCHAR buf[8192];
	int len;

	if(sock == INVALID_SOCKET)
		return;

	len = _stprintf_s(buf, (sizeof buf / sizeof (TCHAR)), _T("<%d>%s"),
			facility * 8 + severity,
			header);

	len += _vstprintf_s(&buf[len], (sizeof buf / sizeof (TCHAR)) - len, format, marker);

#ifdef UNICODE
	{
		char cbuf[1024];
		len = WideCharToMultiByte(CP_OEMCP, 0, buf, -1, cbuf, sizeof cbuf, 0, 0) -1;
		SendToSyslog(cbuf, len);
	}
#else
	SendToSyslog(buf, len);
#endif
}

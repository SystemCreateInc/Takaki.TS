#ifndef __SOCKET_H__
#define __SOCKET_H__

void InitializeSocket(void);
void TerminateSocket(void);
void SyslogWrite(int severity, LPCTSTR header, LPCTSTR format, va_list marker);

#endif //__SOCKET_H__
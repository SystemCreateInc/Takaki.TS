/*
 *	ログ管理ＤＬＬ
 *		ログ書き込み、期限切れログ削除
 *	
 *	Copyright:	(c) 2001 System Create Inc.
 *
 *	Author:		Hirobumi Shimada
 *
 *	Chainges:	April 25, 2001 新規
 *
 *	CVS Id		$Id: Syslog.h,v 1.1 2005/11/02 02:59:55 hiro Exp $
 *
 */

#ifndef __SYSLOG_H__
#define __SYSLOG_H__

#include <stdlib.h>
#include <windows.h>

#if !defined(__SYSLOG__) && !defined(_WIN32_WCE)
#ifdef _MSC_VER
#	ifdef _DEBUG
#		pragma comment(lib, "Syslog64d.lib")
#	else
#		pragma comment(lib, "Syslog64.lib")
#	endif
#endif
#endif

typedef struct {
	unsigned int	expdays;				/*	有効期限				*/
	TCHAR			logroot[_MAX_PATH];		/*	ログルート				*/
	int				trunc;					/*	ログ削除フラグ			*/
} SLINIT;

#ifdef __cplusplus
extern "C" {
#endif

/*
 *	初期化 NULLの場合はiniファイルの設定を使用する
 *	内部で SLTruncExpiredLog を呼んでいる
 *
 *	InitIncetance の先頭で SLInit(NULL); とすればいい
 */
DWORD WINAPI SLInit(SLINIT *si);

#define		SV_EMERG				0	/*	致命的				*/
#define		SV_ALERT				1	/*	警戒				*/
#define		SV_CRIT					2	/*	危機的				*/
#define		SV_ERR					3	/*	エラー				*/
#define		SV_WARN					4	/*	警告				*/
#define		SV_NOTICE				5	/*	通知				*/
#define		SV_INFO					6	/*	情報				*/
#define		SV_DEBUG				7	/*	デバッグ			*/

/*
 *	ログ書き込み　SEVERITY付き
 */
DWORD WINAPI SLPrintf(int severity, LPCTSTR format, ...);
DWORD WINAPI SLVPrintf(int severity, LPCTSTR format, va_list marker);
DWORD WINAPI SLRawWrite(int severity, LPCTSTR str);

/*
 *	ログフィルター設定
 *	設定したseverity以下がログに出力される
 */
void WINAPI SLSetLoglevel(int severity);


/*
 *	期限切れログ削除
 *	SLInit で指定した有効期限より古い日付のログを削除する
 */
DWORD WINAPI SLTruncExpiredLog(void);	

/*
 *	ログディレクトリに指定ファイルをコピーする拡張子に日付を付加する
 */
DWORD WINAPI SLCopy(LPCTSTR filename);



/*
 *	!!! ATTENTION !!! 
 *
 *		以下は古い関数です。
 *		これからのプログラムにはなるべく使用しないでください｡
 *		古い関数を使用した場合severityは全て INFO　で出力されます｡
 *
 */

/*
 *	ログ書き込み
 */
DWORD WINAPI SLWrite(LPCTSTR format, ...);
DWORD WINAPI SLWriteArg(LPCTSTR format, va_list marker);

/*
 *	パラメータ取得
 */
void WINAPI SLGetParams(SLINIT* si);


#ifdef __cplusplus
};
#endif

#endif //__SYSLOG_H__
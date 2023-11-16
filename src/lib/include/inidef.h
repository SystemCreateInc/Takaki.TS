/*
 *	INIファイル定義
 *	
 *	Copyright:	(c) 2001 System Create Inc.
 *
 *	CVS Id		$Id: inidef.h,v 1.3 2005/12/07 05:52:27 hiro Exp $
 *
 */

#ifndef __INIDEF_H__
#define __INIDEF_H__

/*
 *	共通
 */
#define	INI_GLOBAL				"GLOBAL"				//	共通セクション
#define		INI_ID					"ID"				//	クライアントＩＤ
#define		INI_ROOT				"ROOT"				//	システムルート
#define		INI_CONNECT				"CONNECT"			//	ＤＢ接続
#define		INI_USER				"USER"				//	ユーザー
#define		INI_PASSWD				"PASSWD"			//	パスワード
#define		INI_LABELTYPE			"LABELTYPE"			//	ラベルタイプ
#define		INI_DEFAULT_PRINTERPORT	"DEFAULT_PRINTERPORT"	//	既定のプリンターポート指定無しで選択 0:232C 1:IrDA
#define		INI_DEFAULT_LOCATION	"DEFAULT_LOCATION"		//	既定の作業場所 指定無しで選択
#define		INI_AUTOWAREHOUSE		"AUTOWAREHOUSE"		//	自動倉庫
#define		INI_DEBUGLEVEL			"DEBUGLEVEL"		//	デバッグレベル



/*
 *	システムログ
 *	module:	syslog.dll
 */
#define INI_SYSLOG				"SYSLOG"					//	ログセクション
#define		INI_ROOT				"ROOT"					//	ログルート
#define		INI_EXPDAYS				"EXPDAYS"				//	保存期間
#define		INI_FILE				"FILE"					//	ログファイル名
#define		INI_PREFIX				"PREFIX"				//	ログプレフィックス
#define		INI_FLUSH				"FLUSH"					//	フラッシュする
#define		INI_TRANSMITTO			"TRANSMITTO"			//	送信先
#define		INI_TRANSMITPORT		"TRANSMITPORT"			//	送信先ポート
#define		INI_USETCP				"USETCP"				//	TCP使用
#define		INI_SPLITSIZE			"SPLITSIZE"				//	分割サイズ


#endif //__INIDEF_H__

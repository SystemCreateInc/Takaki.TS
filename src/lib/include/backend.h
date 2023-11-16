#ifndef __BACKEND_H__
#define __BACKEND_H__

#define		BE_VERSION					1
#define		BE_PREAMPLE					0xAAAA8888

#define		BE_LISTEN_EVENT_NAME		"be_listen_event"
#define		BE_LISTEN_LOCK_NAME			"be_listen_lock"
#define		BE_LISTEN_SHMEM_NAME		"be_listen_shmem"

/*
 *	メッセージ
 */
#define		BEM_ECHO							0
#define		BEM_REGIST							1
#define		BEM_REGISTERED						2
#define		BEM_UNREGISTERED					3
#define		BEM_LISTREGIST						5
#define		BEM_LISTCLIENTS						6
#define		BEM_LISTPROCESSORS					7
#define		BEM_NOTIFYMASK						8
#define		BEM_SENDTOCAPABILITY				9
#define		BEM_USER							0x10000000

/*
 *	エラーメッセージ
 */
#define		BE_ERROR_OK							0
#define		BE_ERROR_INVALIDPARAM				1
#define		BE_ERROR_CONNECT					2
#define		BE_ERROR_SEND						3
#define		BE_ERROR_READ						4
#define		BE_ERROR_TIMEOUT					5
#define		BE_ERROR_UNKNOWN					6
#define		BE_ERROR_OUTOFMEMORY				7
#define		BE_ERROR_UNKNOWNCLIENT				8
#define		BE_ERROR_UNKNOWNMESSAGE				9
#define		BE_ERROR_CANCELED					10
#define		BE_ERROR_CHECKTIMEOUT				11

#define		BE_CLIENTTYPE_UNKNOWN				0
#define		BE_CLIENTTYPE_BACKEND				1
#define		BE_CLIENTTYPE_PROCESSOR				2
#define		BE_CLIENTTYPE_FRONTEND				3

#define		BE_PROCESSORID_RESOURCE				0

/*
 *	UIDマスク
 */
#define		BE_UID_PERMBASE						0x00000001
#define		BE_UID_PERMMASK						0x00000fff
#define		BE_UID_TEMPBASE						0x00001000
#define		BE_UID_TEMPMASK						0x00fff000
#define		BE_UID_BIDBASE						0x01000000
#define		BE_UID_BIDMASK						0xff000000

/*
 *	権限
 */
#define		BE_AUTHPRIV_ROOT					0
#define		BE_AUTHPRIV_USER					100
#define		BE_AUTHPRIV_GUEST					1000

/*
 *	メッセージタイプ
 */
#define		BEMS_NONEEDRESULT					(0)
#define		BEMS_NEEDRESULT						(1 << 0)
#define		BEMS_RESULT							(1 << 1)
#define		BEMS_ERROR							(1 << 2)
#define		BEMS_BROADCAST						(1 << 3)


/*
 *	ステータスビット
 */

#pragma pack (push, 1)

/*
 *	メッセージ
 */
struct be_message {
	long	preample;						/*	プリアンプル			*/
	struct be_message_version {
		unsigned version: 4;				/*	バージョン				*/
		unsigned headerlength: 4;			/*	ヘッダー長				*/
	} vh;
	long	size;							/*	全体サイズ				*/
	short	status;							/*	ステータスビット		*/
	long	dst_clientid;					/*	送信先クライアントid	*/
	long	src_clientid;					/*	送信元クライアントid	*/
	long	message;						/*	送信メッセージ			*/
	char	data[1];
};

struct be_regist {
	short		clienttype;					/*	登録クライアントタイプ	*/
	long		uid;						/*	uid						*/
	char		name[32];					/*	名称					*/
	char		capability[64];				/*	能力					*/
	char		user[16];					/*	ユーザー				*/
	char		passwd[16];					/*	パスワード				*/
	char		address[32];				/*	アドレス				*/
};

struct be_notifymask {
	long		acceptable;					/*	許可フラグ				*/
};

struct be_sendtocapability {
	char		capability[64];
	long		message;
	long		datasize;
	char		data[1];
};

/*
 *	クライアント一覧
 */
struct be_clientlist {
	short				count;
	struct be_regist	client[1];
};

#pragma pack (pop)

struct be_state {
	short			clienttype;					/*	クライアントタイプ			*/
	short			timeout_sec;				/*	タイムアウト秒				*/
	short			port;						/*	ポート						*/
	const char		*capability;				/*	能力						*/
	const char		*name;						/*	名称						*/
	const char		*user;						/*	ユーザー					*/
	const char		*passwd;					/*	パスワード					*/
	const char		*backend;					/*	接続先バックエンドアドレス	*/

	void			*internal;
};


#ifdef __cplusplus
extern "C" {
#endif

int BE_Init(struct be_state *bes);
void BE_Term(struct be_state *bes);
int BE_Connect(struct be_state *bes);
int BE_Disconnect(struct be_state *bes);

int BE_Call(struct be_state *bes,
				long handle,				/*	送信先のハンドル				*/
				long message,				/*	アプリケーション定義メッセージ	*/
				long sendsize,				/*	送信データサイズ				*/
				const char *sendbuf,		/*	送信データ						*/
				long *recvsize,				/*	受信データサイズ				*/
				char **recvbuf);			/*	受信データ						*/

int BE_CallByName(struct be_state *bes,
				const char *capability,		/*	送信先のキャパビリティー		*/
				long message,				/*	アプリケーション定義メッセージ	*/
				long sendsize,				/*	送信データサイズ				*/
				const char *sendbuf,		/*	送信データ						*/
				long *recvsize,				/*	受信データサイズ				*/
				char **recvbuf);			/*	受信データ						*/

int BE_PostData(struct be_state *bes, 
				long handle,				/*	送信先のハンドル				*/
				short status,				/*	ステータス						*/
				long message,				/*	アプリケーション定義メッセージ	*/
				long sendsize,				/*	送信データサイズ				*/
				const char *sendbuf);		/*	送信データ						*/

int BE_PostDataByName(struct be_state *bes, 
				const char *capability,		/*	送信先のハンドル				*/
				short status,				/*	ステータス						*/
				long message,				/*	アプリケーション定義メッセージ	*/
				long sendsize,				/*	送信データサイズ				*/
				const char *sendbuf);		/*	送信データ						*/

int BE_GetMessage(struct be_state *bes, 
				struct be_message **bem);	/*	受信メッセージ					*/

int BE_Cancel(struct be_state *bes);

#ifdef __cplusplus
};
#endif

#endif //__BACKEND_H__
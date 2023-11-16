#include <time.h>
#include <stddef.h>

#ifdef _WIN32
#include <winsock2.h>
#pragma comment(lib, "ws2_32.lib")
#define	SYNC_USE_WIN32
#endif

//#define	BE_DEBUG

#include <backend.h>
#include <syslog.h>
#include <common.h>
#include <tchar.h>

/*
 *	内部使用ステータス
 */
struct be_state_internal {
	short			seq;
	UINT_PTR		socket;
	short			stalled;
	char			*recvbuf;
	long			recvbufsize;
	long			recvsize;
	long			backendsize;
	char			*backend;

	CRITICAL_SECTION	lock;

#ifndef SYNC_USE_WIN32
	volatile long	cancel;
#else
	HANDLE			cancel;
	HANDLE			socketevent;
#endif
};

#ifndef SYNC_USE_WIN32

/*
 *	受信可能チェック
 *
 *  for select
 */
static int recv_data_available(struct be_state *bes, int nowait)
{
	int readycnt;
	fd_set fdr;
	struct timeval to;
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

	//	cancel チェックのため500msec 毎に復帰する
	to.tv_sec = 0;
	to.tv_usec = 500000;

	if (nowait)
		to.tv_usec = 0;

	FD_ZERO(&fdr);
	FD_SET(besi->socket, &fdr);

	readycnt = select(1, &fdr, NULL, NULL, &to);
	if (readycnt == 0)
		return besi->cancel ? BE_ERROR_CANCELED : BE_ERROR_CHECKTIMEOUT;

	if (!FD_ISSET(besi->socket, &fdr))
		return BE_ERROR_READ;

	return BE_ERROR_OK;
}

/*
 *	送信可能チェック
 *
 *  for select
 */
static int send_data_available(struct be_state *bes)
{
	int readycnt;
	fd_set fdw;
	struct timeval to;
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

	//	cancel チェックのため500msec 毎に復帰する
	to.tv_sec = 0;
	to.tv_usec = 500000;

	FD_ZERO(&fdw);
	FD_SET(besi->socket, &fdw);

	readycnt = select(1, NULL, &fdw, NULL, &to);
	if (readycnt == 0)
		return besi->cancel ? BE_ERROR_CANCELED : BE_ERROR_CHECKTIMEOUT;

	if (!FD_ISSET(besi->socket, &fdw))
		return BE_ERROR_SEND;

	return BE_ERROR_OK;
}

#else

/*
 *	受信可能チェック
 *
 *  for win32
 */
static int recv_data_available(struct be_state *bes, int nowait)
{
	int index, rc = BE_ERROR_READ, timeout;
	HANDLE handles[2];
	WSANETWORKEVENTS events;
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

	//	待ち受けイベント設定
	handles[0] = besi->cancel;
	handles[1] = besi->socketevent;

	if (WSAEventSelect(besi->socket, handles[1], FD_READ | FD_CLOSE) == SOCKET_ERROR)
	{
		SLPrintf(SV_WARN, _T("BE: FAIL!!! WSAEventSelect %d"), WSAGetLastError());
		return BE_ERROR_READ;
	}

	if (nowait)
		timeout = 0;
	else
		timeout = bes->timeout_sec == 0 ? INFINITE : bes->timeout_sec * 1000;

	index = WaitForMultipleObjects(sizeof handles / sizeof (HANDLE), handles, FALSE, timeout);
	if (index == WAIT_FAILED)
	{
		SLPrintf(SV_WARN, _T("BE: FAIL!!! wait error is %d"), WSAGetLastError());
		return BE_ERROR_READ;
	}

	if (index == WAIT_TIMEOUT)
		return BE_ERROR_TIMEOUT;

	index -= WAIT_OBJECT_0;

	//	終了イベント
	if (index == 0)
		return BE_ERROR_CANCELED;

	if (WSAEnumNetworkEvents(besi->socket, besi->socketevent, &events) == SOCKET_ERROR)
	{
		SLPrintf(SV_WARN, _T("BE: FAIL!!! enum events is %d"), WSAGetLastError());
		return BE_ERROR_READ;
	}

	if (events.lNetworkEvents & FD_READ)
		return BE_ERROR_OK;

	if (events.lNetworkEvents & FD_CLOSE)
	{
		SLPrintf(SV_INFO, _T("BE: Closed by peer"));
		return BE_ERROR_READ;
	}

	return BE_ERROR_CHECKTIMEOUT;
}

/*
 *	送信可能チェック
 *
 *  for win32
 */
static int send_data_available(struct be_state *bes)
{
	int index, timeout, rc = BE_ERROR_SEND;
	HANDLE handles[2];
	WSANETWORKEVENTS events;
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

	//	待ち受けイベント設定
	handles[0] = besi->cancel;
	handles[1] = besi->socketevent;

	WSAEventSelect(besi->socket, handles[1], FD_WRITE | FD_CLOSE);

	timeout = bes->timeout_sec == 0 ? INFINITE : bes->timeout_sec * 1000;
	index = WaitForMultipleObjects(sizeof handles / sizeof (HANDLE), handles, FALSE, timeout);
	if (index == WAIT_FAILED)
	{
		SLPrintf(SV_WARN, _T("BE: FAIL!!! wait error is %d"), WSAGetLastError());
		return BE_ERROR_SEND;
	}

	if (index == WAIT_TIMEOUT)
		return BE_ERROR_TIMEOUT;

	index -= WAIT_OBJECT_0;

	//	終了イベント
	if (index == 0)
		return BE_ERROR_CANCELED;

	if (WSAEnumNetworkEvents(besi->socket, besi->socketevent, &events) == SOCKET_ERROR)
	{
		SLPrintf(SV_WARN, _T("BE: FAIL!!! enum events is %d"), WSAGetLastError());
		return BE_ERROR_SEND;
	}

	if (events.lNetworkEvents & FD_WRITE)
		return BE_ERROR_OK;

	return BE_ERROR_SEND;
}

#endif

/*	
 * 不要データ削除	
 */
static int purge_recvbuffer(struct be_state *bes)
{
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

	//	バッファークリア
	besi->recvsize = 0;

	//	未読データ破棄
	while(recv_data_available(bes, 1) == BE_ERROR_OK)
	{
		if (recv(besi->socket, besi->recvbuf, besi->recvbufsize, 0) == SOCKET_ERROR)
		{
			SLPrintf(SV_WARN, _T("BE: FAIL!!! purge recv %d"), WSAGetLastError());
			return BE_ERROR_READ;
		}
	}

	return BE_ERROR_OK;
}

static int send_message(struct be_state *bes, struct be_message *bem)
{
	int byte, bemsize = bem->size;
	char *p = (char*)bem;
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

	/*	データ送信	*/
	while(bemsize > 0)
	{
		if ((byte = send(besi->socket, p, bemsize, 0)) == SOCKET_ERROR)
		{
			int err = WSAGetLastError();
			if (err ==  WSAEWOULDBLOCK)
			{
				int rc;

				SLPrintf(SV_DEBUG, _T("BE: wait send..."));
				rc = send_data_available(bes);
				if (rc == BE_ERROR_OK)
				{
					continue;
				}

				if (rc == BE_ERROR_TIMEOUT)
				{
					SLPrintf(SV_WARN, _T("BE: send wait timed out, retry"));
					continue;
				}

				SLPrintf(SV_WARN, _T("BE: send wait error %d"), rc);
			}

			SLPrintf(SV_WARN, _T("BE: FAIL!!! send %d"), err);
			return BE_ERROR_SEND;
		}

		bemsize -= byte;
		p += byte;
	}

	return BE_ERROR_OK;
}

/*
 *	受信 select 使用
 */
static int recv_message(struct be_state *bes, struct be_message **bem)
{
	int rc, i;
	long packetsize, packetrecvd, byte;
	time_t limittime;
	char *msgbuf = NULL;
	struct be_message *tbem;
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

	/*	受信	*/
	limittime = time(NULL) + bes->timeout_sec;

	packetsize = 0;
	packetrecvd = 0;

	//	デフォルトの戻り値はタイムアウト
	rc = BE_ERROR_TIMEOUT;
	
	for(;;)
	{
		if (packetsize == 0)
		{
			//	プリアンプル検出
			if (besi->recvsize > offsetof(struct be_message, preample) + sizeof ((struct be_message*)0)->preample)
			{
				for (i = 0; i < besi->recvsize; i++)
				{
					if (((struct be_message*)besi->recvbuf + i)->preample == BE_PREAMPLE)
						break;
				}

				besi->recvsize -= i;
				memmove_s(besi->recvbuf, besi->recvbufsize, besi->recvbuf + i, besi->recvsize);

				//	バージョンチェック
				tbem = (struct be_message*)besi->recvbuf;
				if (besi->recvsize >= offsetof(struct be_message, vh) + sizeof ((struct be_message*)0)->vh
					&& tbem->vh.version != BE_VERSION)
				{
					SLPrintf(SV_WARN, _T("BE: MESSAGE VERSION MISMATCH msg:%d cur:%d"), 
							tbem->vh.version,
							BE_VERSION);

					besi->recvsize += tbem->vh.headerlength;
					memmove_s(besi->recvbuf, besi->recvbufsize, besi->recvbuf + tbem->vh.headerlength, tbem->vh.headerlength);
					packetsize = 0;
				}
				else
				{
					//	パケット長を求める
					if (besi->recvsize >= offsetof(struct be_message, size) + sizeof ((struct be_message*)0)->size)
					{
						packetsize = tbem->size;
						packetrecvd = 0;
						msgbuf = malloc(packetsize);
						if (msgbuf == NULL)
						{
							rc = BE_ERROR_OUTOFMEMORY;
							break;
						}
					}
				}
			}
		}

		//	パケット長以上のデータを受信したら終わり
		if (packetsize > 0 && besi->recvsize > 0)
		{
			byte = min(packetsize - packetrecvd, besi->recvsize);
			memcpy_s(msgbuf + packetrecvd, packetsize, besi->recvbuf, byte);
			packetrecvd += byte;
			besi->recvsize -= byte;
			memmove_s(besi->recvbuf, besi->recvbufsize, besi->recvbuf + byte, besi->recvsize);

			if (packetrecvd == packetsize)
			{
				*bem = (struct be_message*)msgbuf;
				return BE_ERROR_OK; 
			}
		}

		//	タイムアウトチェック
		if (bes->timeout_sec != 0 && time(NULL) > limittime)
		{
			rc = BE_ERROR_TIMEOUT;
			break;
		}

		//	受信データチェック
		rc = recv_data_available(bes, 0);
		if (rc == BE_ERROR_CHECKTIMEOUT)
			continue;

		if (rc != BE_ERROR_OK)
			break;

		//	受信
		if ((byte = recv(besi->socket, besi->recvbuf + besi->recvsize, besi->recvbufsize - besi->recvsize, 0)) == SOCKET_ERROR)
		{
			SLPrintf(SV_WARN, _T("BE: FAIL!!! recv %d"), WSAGetLastError());
			rc = BE_ERROR_READ;
			break;
		}

		//SLPrintf(SV_DEBUG, _T("BE: read %d %d bytes"), byte, besi->recvsize);
		besi->recvsize += byte;
	}

	free(msgbuf);

	return rc;
}

/*
 *	ステータス初期化
 */
int BE_Init(struct be_state *bes)
{
	struct be_state_internal *besi;

	memset(bes, 0, sizeof *bes);

	/* 内部ステータス領域確保 */
	besi = malloc(sizeof (struct be_state_internal));
	bes->internal = besi;
	if (bes->internal == NULL)
		return BE_ERROR_OUTOFMEMORY;

	bes->timeout_sec = 30;
	besi->socket = INVALID_SOCKET;
	besi->stalled = 1;
	besi->recvbufsize = 8192;
	besi->recvbuf = malloc(besi->recvbufsize);
	besi->recvsize = 0;
	besi->backendsize = _MAX_PATH;
	besi->backend = malloc(besi->backendsize);

	InitializeCriticalSection(&besi->lock);

#ifdef SYNC_USE_WIN32
	besi->cancel = CreateEvent(NULL, TRUE, FALSE, NULL);
	besi->socketevent = WSACreateEvent();
#else
	besi->cancel = 0;
#endif

	return BE_ERROR_OK;
}

/*
 *	ステータス終了
 */
void BE_Term(struct be_state *bes)
{
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

	DeleteCriticalSection(&besi->lock);

#ifdef SYNC_USE_WIN32
	CloseHandle(besi->cancel);
	WSACloseEvent(besi->socketevent);
#endif

	free(besi->recvbuf);
	free(besi->backend);
	free(bes->internal);

	bes->internal = NULL;
}

/*
 *	通知マスクをセットする
 */
int BE_Notifymask(struct be_state *bes, int acceptable)
{
	struct be_notifymask mask;

	memset(&mask, 0, sizeof mask);

	mask.acceptable = acceptable;

	return BE_PostData(bes, 0, 0, BEM_NOTIFYMASK, sizeof mask, (char*)&mask);
}

/*
 *	バックエンドに登録する
 */
int BE_Regist(struct be_state *bes)
{
	struct be_regist regist;
	long bytes, rc;
	char *recvbuf = NULL;

	memset(&regist, 0, sizeof regist);

	regist.clienttype = bes->clienttype;

	if (bes->name)
		strcpy_s(regist.name, sizeof regist.name, bes->name);

	if (bes->capability)
		strcpy_s(regist.capability, sizeof regist.capability, bes->capability);

	if (bes->user)
		strcpy_s(regist.user, sizeof regist.user, bes->user);

	if (bes->passwd)
		strcpy_s(regist.passwd, sizeof regist.passwd, bes->passwd);

	rc = BE_Call(bes, 0, BEM_REGIST, sizeof regist, (char*)&regist, &bytes, &recvbuf);

	if (rc != BE_ERROR_OK)
		return rc;

	free(recvbuf);

	SLPrintf(SV_DEBUG, _T("BE: Registered to backend"));

	/* 通知許可にする */
	return BE_Notifymask(bes, 1);
}

int BE_Connect(struct be_state *bes)
{
	int rc, on = 1;
	struct hostent *he;
	struct sockaddr_in addr;
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

	/*	ロック	
	 *
	 *	ATTENTION: クリティカルセクションでロックしているので必ずこの関数からはexitから抜けること
	 *
	 */
	EnterCriticalSection(&besi->lock);

	if (WaitForSingleObject(besi->cancel, 0) == WAIT_OBJECT_0)
	{
		rc = BE_ERROR_CANCELED;
		goto exit;
	}

	if (!besi->stalled)
	{
		SLPrintf(SV_DEBUG, _T("BE: Connecting backend, but already connected"));
		rc = BE_ERROR_OK;
		goto exit;
	}

	/* バックエンドの指定がないときはローカルに接続 */
	if (bes->backend == NULL)
	{
		char *fname, inipath[_MAX_PATH];
		SearchPathA(NULL, SYSTEMINIFILE, NULL, _MAX_PATH, inipath, &fname);
		GetPrivateProfileStringA("GLOBAL", "TARGETBACKEND", "", besi->backend, besi->backendsize, inipath);
		if (strlen(besi->backend) == 0)
		{
			SLPrintf(SV_INFO, _T("backend not specified use localhost"));
			strcpy_s(besi->backend, besi->backendsize, "localhost");
		}

		bes->backend = _strdup(besi->backend);
		SLPrintf(SV_INFO, "target backend is %s", bes->backend);
	}

	if (bes->port == 0)
		bes->port = 5501;


	//	名前解決
	memset(&addr, 0, sizeof addr);

	addr.sin_family = AF_INET;
	addr.sin_port = htons(bes->port);
	addr.sin_addr.s_addr = inet_addr(bes->backend);
	
	if (addr.sin_addr.s_addr == INADDR_NONE)
	{
		he = gethostbyname(bes->backend);
		if (he == NULL)
		{
			rc = BE_ERROR_INVALIDPARAM;
			goto exit;
		}

		addr.sin_addr.s_addr = *(long*)he->h_addr_list[0];
	}

	//	接続
	if ((besi->socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)) == INVALID_SOCKET)
	{
		rc = BE_ERROR_CONNECT;
		goto exit;
	}

	if (connect(besi->socket, (struct sockaddr*)&addr, sizeof addr) == SOCKET_ERROR)
	{
		closesocket(besi->socket);
		besi->socket = INVALID_SOCKET;
		rc = BE_ERROR_CONNECT;
		goto exit;
	}

	setsockopt(besi->socket, IPPROTO_TCP, TCP_NODELAY, (const char*)&on, sizeof on);

	//	リソースマネージャーに登録する
	besi->stalled = 0;
	rc = BE_Regist(bes);

	//	ストールフラグをリセット
	besi->stalled = rc == BE_ERROR_OK ? 0 : 1;

	SLPrintf(SV_DEBUG, _T("BE: Connected to backend"));

exit:
	/*	ロック解除	*/
	LeaveCriticalSection(&besi->lock);

	return rc;
}

int BE_Disconnect(struct be_state *bes)
{
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

	if (besi->socket != INVALID_SOCKET)
	{
		closesocket(besi->socket);
		besi->socket = INVALID_SOCKET;
	}

	besi->stalled = 1;

	SLPrintf(SV_DEBUG, _T("BE: Disconnected from backend"));

	return BE_ERROR_OK;
}

/*
 *	受信中のスレッドをキャンセルする
 */
int BE_Cancel(struct be_state *bes)
{
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

	if (besi == NULL)
		return BE_ERROR_INVALIDPARAM;

	SLPrintf(SV_DEBUG, _T("BE: Cancel request"));

#ifndef SYNC_USE_WIN32
	besi->cancel = 1;
#else
	SetEvent(besi->cancel);
#endif
	return BE_ERROR_OK;
}

int BE_GetMessage(struct be_state *bes, struct be_message **bem)
{
	int rc;
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

#ifdef BE_DEBUG
	SLPrintf(SV_DEBUG, _T("BE: Get message..."));
#endif

	//	もしストールしていたら再接続する
	if (besi->stalled)
	{
		if ((rc = BE_Connect(bes)) != BE_ERROR_OK)
			return rc;
	}

	if ((rc = recv_message(bes, bem)) != BE_ERROR_OK)
	{
		if (rc != BE_ERROR_TIMEOUT)
		{
			SLPrintf(SV_WARN, _T("BE: FAIL!!! GetMessage %d"), rc);
			BE_Disconnect(bes);
		}

		return rc;
	}

	return BE_ERROR_OK;
}

int BE_PostData(struct be_state *bes, 
				long handle,				/*	送信先のハンドル				*/
				short status,				/*	ステータス						*/
				long message,				/*	アプリケーション定義メッセージ	*/
				long sendsize,				/*	送信データサイズ				*/
				const char *sendbuf)		/*	送信データ						*/
{
	int rc;
	long bemsize;
	struct be_message *sendbem;
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

#ifdef BE_DEBUG
	SLPrintf(SV_DEBUG, _T("BE: Post message"));
#endif

	//	もしストールしていたら再接続する
	if (besi->stalled)
	{
		if ((rc = BE_Connect(bes)) != BE_ERROR_OK)
			return rc;
	}

	/* 送信データ作成　*/
	bemsize = sizeof (struct be_message) + sendsize -1;
	sendbem = (struct be_message*)malloc(bemsize);

	if (sendbem == NULL)
		return BE_ERROR_OUTOFMEMORY;

	memset(sendbem, 0, bemsize);
	sendbem->preample		= BE_PREAMPLE;
	sendbem->vh.version		= BE_VERSION;
	sendbem->vh.headerlength = sizeof (struct be_message);
	sendbem->size			= bemsize;
	sendbem->dst_clientid	= handle;
	sendbem->message		= message;
	sendbem->status			= status;

	memcpy(sendbem->data, sendbuf, sendsize);

	if ((rc = send_message(bes, sendbem)) != BE_ERROR_OK)
	{
		free(sendbem);
		SLPrintf(SV_WARN, _T("BE: FAIL!!! PostData %d"), rc);
		BE_Disconnect(bes);
		return rc;
	}

	free(sendbem);

	return BE_ERROR_OK;
}

int BE_PostDataByName(struct be_state *bes, 
				const char *capability,		/*	送信先のハンドル				*/
				short status,				/*	ステータス						*/
				long message,				/*	アプリケーション定義メッセージ	*/
				long sendsize,				/*	送信データサイズ				*/
				const char *sendbuf)		/*	送信データ						*/
{
	int rc;
	long bemsize;
	struct be_message *sendbem;
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;
	struct be_sendtocapability *stc = NULL;

#ifdef BE_DEBUG
	SLPrintf(SV_DEBUG, _T("BE: Post message"));
#endif

	//	もしストールしていたら再接続する
	if (besi->stalled)
	{
		if ((rc = BE_Connect(bes)) != BE_ERROR_OK)
			return rc;
	}

	/* 送信データ作成　*/
	bemsize = offsetof(struct be_message, data) 
			+ offsetof(struct be_sendtocapability, data) 
			+ sendsize;
	sendbem = (struct be_message*)malloc(bemsize);

	if (sendbem == NULL)
		return BE_ERROR_OUTOFMEMORY;

	memset(sendbem, 0, bemsize);
	sendbem->preample		= BE_PREAMPLE;
	sendbem->vh.version		= BE_VERSION;
	sendbem->vh.headerlength = sizeof (struct be_message);
	sendbem->size			= bemsize;
	sendbem->dst_clientid	= 0;
	sendbem->message		= BEM_SENDTOCAPABILITY;
	sendbem->status			= status;
	stc = (struct be_sendtocapability*)sendbem->data;
	strcpy_s(stc->capability, sizeof stc->capability, capability);
	stc->message			= message;
	stc->datasize			= sendsize;
	memcpy(stc->data, sendbuf, sendsize);

	if ((rc = send_message(bes, sendbem)) != BE_ERROR_OK)
	{
		free(sendbem);
		SLPrintf(SV_WARN, _T("BE: FAIL!!! PostDataByName %d"), rc);
		BE_Disconnect(bes);
		return rc;
	}

	free(sendbem);

	return BE_ERROR_OK;
}

int BE_Call(struct be_state *bes,
			long handle,			/*	送信先のハンドル				*/
			long message,			/*	アプリケーション定義メッセージ	*/
			long sendsize,			/*	送信データサイズ				*/
			const char *sendbuf,	/*	送信データ						*/
			long *recvsize,			/*	受信データサイズ				*/
			char **recvbuf)			/*	受信データ						*/
{
	int rc;
	struct be_message *recvbem;
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;

#ifdef BE_DEBUG
	SLPrintf(SV_DEBUG, _T("BE: Call..."));
#endif

	*recvsize = 0;
	*recvbuf = NULL;

	purge_recvbuffer(bes);

	rc = BE_PostData(bes, handle, BEMS_NEEDRESULT, message, sendsize, sendbuf);

	if (rc != BE_ERROR_OK)
		return rc;

	//	同じメッセージが帰ってくるまで受信する
	do
	{
		rc = BE_GetMessage(bes, &recvbem);
		if (rc != BE_ERROR_OK)
			return rc;
	}
	while (recvbem->message != message);

	//	メッセージステータスが BEMS_ERROR ならステータスのみ
	if (recvbem->status & BEMS_ERROR)
	{
		rc = *(long*)recvbem->data;
		free(recvbem);
		return rc;
	}

	//	受信データを領域の先頭に移動する
	*recvsize = recvbem->size - offsetof(struct be_message, data);
	*recvbuf = (char*)recvbem;
	memmove(*recvbuf, recvbem->data, *recvsize);

	return rc;
}

int BE_CallByName(struct be_state *bes,
			const char *capability,	/*	名称							*/
			long message,			/*	アプリケーション定義メッセージ	*/
			long sendsize,			/*	送信データサイズ				*/
			const char *sendbuf,	/*	送信データ						*/
			long *recvsize,			/*	受信データサイズ				*/
			char **recvbuf)			/*	受信データ						*/
{
	int rc;
	long stcsize;
	struct be_message *recvbem;
	struct be_state_internal *besi = (struct be_state_internal*)bes->internal;
	struct be_sendtocapability *stc = NULL;

#ifdef BE_DEBUG
	SLPrintf(SV_DEBUG, _T("BE: Call..."));
#endif

	//	送信データ作成
	stcsize = offsetof(struct be_sendtocapability, data) + sendsize;
	stc = (struct be_sendtocapability*)malloc(stcsize);
	if (!stc)
	{
		return BE_ERROR_UNKNOWN;
	}

	memset(stc, 0, stcsize);
	strcpy_s(stc->capability, sizeof stc->capability, capability);
	stc->message = message;
	stc->datasize = sendsize;
	memcpy(stc->data, sendbuf, sendsize);

	*recvsize = 0;
	*recvbuf = NULL;

	purge_recvbuffer(bes);

	rc = BE_PostData(bes, 0, BEMS_NEEDRESULT, BEM_SENDTOCAPABILITY, stcsize, (const char*)stc);

	free(stc);

	if (rc != BE_ERROR_OK)
		return rc;

	//	同じメッセージが帰ってくるまで受信する
	do
	{
		rc = BE_GetMessage(bes, &recvbem);
		if (rc != BE_ERROR_OK)
			return rc;
	}
	while (recvbem->message != message && recvbem->message != BEM_SENDTOCAPABILITY);

	//	メッセージステータスが BEMS_ERROR ならステータスのみ
	if (recvbem->status & BEMS_ERROR)
	{
		rc = *(long*)recvbem->data;
		free(recvbem);
		return rc;
	}

	//	受信データを領域の先頭に移動する
	*recvsize = recvbem->size - offsetof(struct be_message, data);
	*recvbuf = (char*)recvbem;
	memmove(*recvbuf, recvbem->data, *recvsize);

	return rc;
}


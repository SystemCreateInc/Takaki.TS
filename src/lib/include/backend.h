#ifndef __BACKEND_H__
#define __BACKEND_H__

#define		BE_VERSION					1
#define		BE_PREAMPLE					0xAAAA8888

#define		BE_LISTEN_EVENT_NAME		"be_listen_event"
#define		BE_LISTEN_LOCK_NAME			"be_listen_lock"
#define		BE_LISTEN_SHMEM_NAME		"be_listen_shmem"

/*
 *	���b�Z�[�W
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
 *	�G���[���b�Z�[�W
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
 *	UID�}�X�N
 */
#define		BE_UID_PERMBASE						0x00000001
#define		BE_UID_PERMMASK						0x00000fff
#define		BE_UID_TEMPBASE						0x00001000
#define		BE_UID_TEMPMASK						0x00fff000
#define		BE_UID_BIDBASE						0x01000000
#define		BE_UID_BIDMASK						0xff000000

/*
 *	����
 */
#define		BE_AUTHPRIV_ROOT					0
#define		BE_AUTHPRIV_USER					100
#define		BE_AUTHPRIV_GUEST					1000

/*
 *	���b�Z�[�W�^�C�v
 */
#define		BEMS_NONEEDRESULT					(0)
#define		BEMS_NEEDRESULT						(1 << 0)
#define		BEMS_RESULT							(1 << 1)
#define		BEMS_ERROR							(1 << 2)
#define		BEMS_BROADCAST						(1 << 3)


/*
 *	�X�e�[�^�X�r�b�g
 */

#pragma pack (push, 1)

/*
 *	���b�Z�[�W
 */
struct be_message {
	long	preample;						/*	�v���A���v��			*/
	struct be_message_version {
		unsigned version: 4;				/*	�o�[�W����				*/
		unsigned headerlength: 4;			/*	�w�b�_�[��				*/
	} vh;
	long	size;							/*	�S�̃T�C�Y				*/
	short	status;							/*	�X�e�[�^�X�r�b�g		*/
	long	dst_clientid;					/*	���M��N���C�A���gid	*/
	long	src_clientid;					/*	���M���N���C�A���gid	*/
	long	message;						/*	���M���b�Z�[�W			*/
	char	data[1];
};

struct be_regist {
	short		clienttype;					/*	�o�^�N���C�A���g�^�C�v	*/
	long		uid;						/*	uid						*/
	char		name[32];					/*	����					*/
	char		capability[64];				/*	�\��					*/
	char		user[16];					/*	���[�U�[				*/
	char		passwd[16];					/*	�p�X���[�h				*/
	char		address[32];				/*	�A�h���X				*/
};

struct be_notifymask {
	long		acceptable;					/*	���t���O				*/
};

struct be_sendtocapability {
	char		capability[64];
	long		message;
	long		datasize;
	char		data[1];
};

/*
 *	�N���C�A���g�ꗗ
 */
struct be_clientlist {
	short				count;
	struct be_regist	client[1];
};

#pragma pack (pop)

struct be_state {
	short			clienttype;					/*	�N���C�A���g�^�C�v			*/
	short			timeout_sec;				/*	�^�C���A�E�g�b				*/
	short			port;						/*	�|�[�g						*/
	const char		*capability;				/*	�\��						*/
	const char		*name;						/*	����						*/
	const char		*user;						/*	���[�U�[					*/
	const char		*passwd;					/*	�p�X���[�h					*/
	const char		*backend;					/*	�ڑ���o�b�N�G���h�A�h���X	*/

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
				long handle,				/*	���M��̃n���h��				*/
				long message,				/*	�A�v���P�[�V������`���b�Z�[�W	*/
				long sendsize,				/*	���M�f�[�^�T�C�Y				*/
				const char *sendbuf,		/*	���M�f�[�^						*/
				long *recvsize,				/*	��M�f�[�^�T�C�Y				*/
				char **recvbuf);			/*	��M�f�[�^						*/

int BE_CallByName(struct be_state *bes,
				const char *capability,		/*	���M��̃L���p�r���e�B�[		*/
				long message,				/*	�A�v���P�[�V������`���b�Z�[�W	*/
				long sendsize,				/*	���M�f�[�^�T�C�Y				*/
				const char *sendbuf,		/*	���M�f�[�^						*/
				long *recvsize,				/*	��M�f�[�^�T�C�Y				*/
				char **recvbuf);			/*	��M�f�[�^						*/

int BE_PostData(struct be_state *bes, 
				long handle,				/*	���M��̃n���h��				*/
				short status,				/*	�X�e�[�^�X						*/
				long message,				/*	�A�v���P�[�V������`���b�Z�[�W	*/
				long sendsize,				/*	���M�f�[�^�T�C�Y				*/
				const char *sendbuf);		/*	���M�f�[�^						*/

int BE_PostDataByName(struct be_state *bes, 
				const char *capability,		/*	���M��̃n���h��				*/
				short status,				/*	�X�e�[�^�X						*/
				long message,				/*	�A�v���P�[�V������`���b�Z�[�W	*/
				long sendsize,				/*	���M�f�[�^�T�C�Y				*/
				const char *sendbuf);		/*	���M�f�[�^						*/

int BE_GetMessage(struct be_state *bes, 
				struct be_message **bem);	/*	��M���b�Z�[�W					*/

int BE_Cancel(struct be_state *bes);

#ifdef __cplusplus
};
#endif

#endif //__BACKEND_H__
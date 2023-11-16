/*
 *	���O�Ǘ��c�k�k
 *		���O�������݁A�����؂ꃍ�O�폜
 *	
 *	Copyright:	(c) 2001 System Create Inc.
 *
 *	Author:		Hirobumi Shimada
 *
 *	Chainges:	April 25, 2001 �V�K
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
	unsigned int	expdays;				/*	�L������				*/
	TCHAR			logroot[_MAX_PATH];		/*	���O���[�g				*/
	int				trunc;					/*	���O�폜�t���O			*/
} SLINIT;

#ifdef __cplusplus
extern "C" {
#endif

/*
 *	������ NULL�̏ꍇ��ini�t�@�C���̐ݒ���g�p����
 *	������ SLTruncExpiredLog ���Ă�ł���
 *
 *	InitIncetance �̐擪�� SLInit(NULL); �Ƃ���΂���
 */
DWORD WINAPI SLInit(SLINIT *si);

#define		SV_EMERG				0	/*	�v���I				*/
#define		SV_ALERT				1	/*	�x��				*/
#define		SV_CRIT					2	/*	��@�I				*/
#define		SV_ERR					3	/*	�G���[				*/
#define		SV_WARN					4	/*	�x��				*/
#define		SV_NOTICE				5	/*	�ʒm				*/
#define		SV_INFO					6	/*	���				*/
#define		SV_DEBUG				7	/*	�f�o�b�O			*/

/*
 *	���O�������݁@SEVERITY�t��
 */
DWORD WINAPI SLPrintf(int severity, LPCTSTR format, ...);
DWORD WINAPI SLVPrintf(int severity, LPCTSTR format, va_list marker);
DWORD WINAPI SLRawWrite(int severity, LPCTSTR str);

/*
 *	���O�t�B���^�[�ݒ�
 *	�ݒ肵��severity�ȉ������O�ɏo�͂����
 */
void WINAPI SLSetLoglevel(int severity);


/*
 *	�����؂ꃍ�O�폜
 *	SLInit �Ŏw�肵���L���������Â����t�̃��O���폜����
 */
DWORD WINAPI SLTruncExpiredLog(void);	

/*
 *	���O�f�B���N�g���Ɏw��t�@�C�����R�s�[����g���q�ɓ��t��t������
 */
DWORD WINAPI SLCopy(LPCTSTR filename);



/*
 *	!!! ATTENTION !!! 
 *
 *		�ȉ��͌Â��֐��ł��B
 *		���ꂩ��̃v���O�����ɂ͂Ȃ�ׂ��g�p���Ȃ��ł��������
 *		�Â��֐����g�p�����ꍇseverity�͑S�� INFO�@�ŏo�͂���܂��
 *
 */

/*
 *	���O��������
 */
DWORD WINAPI SLWrite(LPCTSTR format, ...);
DWORD WINAPI SLWriteArg(LPCTSTR format, va_list marker);

/*
 *	�p�����[�^�擾
 */
void WINAPI SLGetParams(SLINIT* si);


#ifdef __cplusplus
};
#endif

#endif //__SYSLOG_H__
/*
 *	INI�t�@�C����`
 *	
 *	Copyright:	(c) 2001 System Create Inc.
 *
 *	CVS Id		$Id: inidef.h,v 1.3 2005/12/07 05:52:27 hiro Exp $
 *
 */

#ifndef __INIDEF_H__
#define __INIDEF_H__

/*
 *	����
 */
#define	INI_GLOBAL				"GLOBAL"				//	���ʃZ�N�V����
#define		INI_ID					"ID"				//	�N���C�A���g�h�c
#define		INI_ROOT				"ROOT"				//	�V�X�e�����[�g
#define		INI_CONNECT				"CONNECT"			//	�c�a�ڑ�
#define		INI_USER				"USER"				//	���[�U�[
#define		INI_PASSWD				"PASSWD"			//	�p�X���[�h
#define		INI_LABELTYPE			"LABELTYPE"			//	���x���^�C�v
#define		INI_DEFAULT_PRINTERPORT	"DEFAULT_PRINTERPORT"	//	����̃v�����^�[�|�[�g�w�薳���őI�� 0:232C 1:IrDA
#define		INI_DEFAULT_LOCATION	"DEFAULT_LOCATION"		//	����̍�Əꏊ �w�薳���őI��
#define		INI_AUTOWAREHOUSE		"AUTOWAREHOUSE"		//	�����q��
#define		INI_DEBUGLEVEL			"DEBUGLEVEL"		//	�f�o�b�O���x��



/*
 *	�V�X�e�����O
 *	module:	syslog.dll
 */
#define INI_SYSLOG				"SYSLOG"					//	���O�Z�N�V����
#define		INI_ROOT				"ROOT"					//	���O���[�g
#define		INI_EXPDAYS				"EXPDAYS"				//	�ۑ�����
#define		INI_FILE				"FILE"					//	���O�t�@�C����
#define		INI_PREFIX				"PREFIX"				//	���O�v���t�B�b�N�X
#define		INI_FLUSH				"FLUSH"					//	�t���b�V������
#define		INI_TRANSMITTO			"TRANSMITTO"			//	���M��
#define		INI_TRANSMITPORT		"TRANSMITPORT"			//	���M��|�[�g
#define		INI_USETCP				"USETCP"				//	TCP�g�p
#define		INI_SPLITSIZE			"SPLITSIZE"				//	�����T�C�Y


#endif //__INIDEF_H__

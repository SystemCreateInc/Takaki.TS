/*
 *
 *	�Œ薼�̃}�X�^
 *
 *	$Id: TB_MKOTEI_MEISHO.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_MKOTEI_MEISHO (

	CD_MEISHO_SHIKIBETSU			char			(5)			not null,	/* 	���̎���						*/
	CD_MEISHO						nvarchar		(8)			not null,	/* 	���̃R�[�h						*/
	DT_TOROKU_NICHIJI				char			(14)		not null,	/* 	�o�^����						*/
	DT_KOSHIN_NICHIJI				char			(14)		not null,	/* 	�X�V����						*/
	CD_HENKOSHA						char            (10)		not null,	/* 	�ύX�҃R�[�h					*/
	NM								nvarchar        (40)		not null,	/* 	����							*/
	NM_YOMI							nvarchar        (20)		not null,	/* 	���̓ǂ�						*/
	NM_RYAKU						nvarchar        (20)		not null,	/* 	����							*/
	NM_RYAKU_YOMI					nvarchar        (5)			not null,	/* 	���̓ǂ�						*/
	CD_KYU_MEISHO_SHIKIBETSU		char            (5)			not null,	/* 	�����̎���						*/
	CD_KYU_MEISHO					char            (8)			not null,	/* 	�����̃R�[�h					*/
	CD_EX_1							char            (10)		not null,	/* 	�g���R�[�h�P					*/
	CD_EX_2							char            (10)		not null,	/* 	�g���R�[�h�Q					*/
	CD_EX_3							char            (10)		not null,	/* 	�g���R�[�h�R					*/
	CD_EX_4							char            (10)		not null,	/* 	�g���R�[�h�S					*/
	CD_EX_5							char            (10)		not null,	/* 	�g���R�[�h�T					*/
	FG_EX_1							char            (1)			not null,	/* 	�g���t���O�P					*/
	FG_EX_2							char            (1)			not null,	/* 	�g���t���O�Q					*/
	FG_EX_3							char            (1)			not null,	/* 	�g���t���O�R					*/
	FG_EX_4							char            (1)			not null,	/* 	�g���t���O�S					*/
	FG_EX_5							char            (1)			not null,	/* 	�g���t���O�T					*/
	NU_HAITA						numeric         (5)			not null,	/* 	�r���J�E���^					*/
	DT_RENKEI						char            (14)		not null,	/* 	�A�g����						*/
	
	createdAt datetime not null,		/*	�f�[�^�}����				*/
	updatedAt datetime not null,		/*	�f�[�^�X�V��				*/

	constraint pk_TB_MKOTEI_MEISHO primary key (CD_MEISHO_SHIKIBETSU,CD_MEISHO)
);

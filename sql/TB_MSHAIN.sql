/*
 *
 *	�Ј��}�X�^
 *
 *	$Id: TB_MSHAIN.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_MSHAIN (
	CD_SHAIN						nchar			(7)			not null,	/* 	�Ј��R�[�h						*/
	DT_TEKIYOKAISHI					nchar           (8)			not null,	/* 	�K�p�J�n��						*/
	DT_TEKIYOMUKO					nchar           (8)			not null,	/* 	�K�p������						*/
	DT_TOROKU_NICHIJI				nchar           (14)		not null,	/* 	�o�^����						*/
	DT_KOSHIN_NICHIJI				nchar           (14)		not null,	/* 	�X�V����						*/
	CD_HENKOSHA						nchar           (10)		not null,	/* 	�ύX�҃R�[�h					*/
	NM_SHAIN						nvarchar        (40)		not null,	/* 	�Ј�����						*/
	NM_SHAIN_YOMI					nvarchar        (60)		not null,	/* 	�����ǂ�						*/
	NM_SHAIN_YOMI_KANA				nvarchar        (30)		not null,	/* 	�����ǂ݁i�J�i�j				*/
	CD_BUMON						nchar           (6)			not null,	/* 	����R�[�h						*/
	FG_TAISHOKUSHA					nchar           (1)			not null,	/* 	�ސE�҃R�[�h					*/
	ST_KOYO							nchar           (2)			not null,	/* 	�ٗp�敪						*/
	IF_USER_MAIL_ADDRESS			nvarchar        (128)		not null,	/* 	���[�U�[���[���A�h���X			*/
	CD_YAKUSHOKU					nchar           (2)			not null,	/* 	��E�R�[�h						*/
	CD_SOTO_YAKUSHOKU				nchar           (2)			not null,	/* 	������E						*/
	CD_SHOKUMU						nchar           (3)			not null,	/* 	�E���R�[�h						*/
	CD_SHOZOKU_KAISHA				nchar           (2)			not null,	/* 	������ЃR�[�h					*/
	CD_SHOZOKU_KANPANI				nchar           (6)			not null,	/* 	�����J���p�j�[�R�[�h			*/
	NU_HAITA						numeric         (5)			not null,	/* 	�r���J�E���^					*/
	DT_RENKEI						nchar           (14)		not null,	/* 	�A�g����						*/
	
	createdAt datetime not null,		/*	�f�[�^�}����				*/
	updatedAt datetime not null,		/*	�f�[�^�X�V��				*/

	constraint pk_TB_MSHAIN primary key (CD_SHAIN,DT_TEKIYOKAISHI)
);


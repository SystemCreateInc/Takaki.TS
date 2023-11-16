/*
 *
 *	���_�}�X�^
 *
 *	$Id: TB_MKYOTEN.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_MKYOTEN (
	CD_KYOTEN						char			(4)			not null,	/* 	���_�R�[�h						*/
	DT_TEKIYOKAISHI					char			(8)			not null,	/* 	�K�p�J�n��						*/
	DT_TEKIYOMUKO					char			(8)			not null,	/* 	�K�p������						*/
	DT_TOROKU_NICHIJI				char			(14)		not null,	/* 	�o�^����						*/
	DT_KOSHIN_NICHIJI				char			(14)		not null,	/* 	�X�V����						*/
	CD_HENKOSHA						char			(10)		not null,	/* 	�ύX�҃R�[�h					*/
	NM_KYOTEN						nvarchar		(40)		not null,	/* 	���_����						*/
	CD_KYOTEN_SHUBETSU				char			(3)			not null,	/* 	���_���						*/
	CD_TORIHIKISAKI					char			(6)			not null,	/* 	�����R�[�h					*/
	CD_ZAIKO_HIKIATE_BUMON			char			(6)			not null,	/* 	�݌Ɉ�������					*/
	NM_KYOTEN_RYAKUSHO				nvarchar		(20)		not null,	/* 	���_����						*/
	CD_KYOTEN_BUMON					char			(6)			not null,	/* 	���_����R�[�h					*/
	ST_SESANKANRI_NIPPAIHIN			char			(1)			not null,	/* 	���Y�Ǘ����z�i�敪				*/
	ST_SEISANKANRI_ZAIKOHIN			char			(1)			not null,	/* 	���Y�Ǘ��݌ɕi�敪				*/
	ST_SHIKIBETSU					char			(1)			not null,	/* 	���ʋ敪						*/
	CD_TENPO_BRAND					char			(2)			not null,	/* 	�X�܃u�����h�R�[�h				*/
	CD_BASHO						char			(4)			not null,	/* 	�ꏊ�R�[�h						*/
	CD_KYOTEN_ZOKUSEI				char			(3)			not null,	/* 	���_�����R�[�h					*/
	NU_HAITA						numeric			(5)			not null,	/* 	�r���J�E���^					*/
	DT_RENKEI						char			(14)		not null,	/* 	�A�g����						*/
	
	createdAt datetime not null,		/*	�f�[�^�}����				*/
	updatedAt datetime not null,		/*	�f�[�^�X�V��				*/

	constraint pk_TB_MKYOTEN primary key (CD_KYOTEN,DT_TEKIYOKAISHI)
);



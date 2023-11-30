/*
 *
 *	�o�׃f�[�^
 *
 *	$Id: TB_DIST.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST (
	ID_DIST				bigint identity(1,1) 	not null,	/* id							*/
	DT_DELIVERY			nchar(8)					not null,	/* �[�i��						*/
	
	CD_SHUKKA_BATCH		nchar(5)				not null,	/* �o�׃o�b�`					*/
	CD_KYOTEN			nchar(4)				not null,	/* �d�������_					*/
	CD_BIN				nchar(3)				not null,	/* �z����						*/
	CD_COURSE			nchar(3)				not null,	/* �R�[�X						*/
	CD_ROUTE			integer					not null,	/* �z����						*/
	CD_TOKUISAKI		nchar(6)				not null,	/* ���Ӑ�R�[�h					*/
	CD_HIMBAN			nchar(9)				not null,	/* 	�i��						*/
	CD_GTIN13			nvarchar(13)			not null,	/* 	JAN����(GTIN-13)			*/
	CD_GTIN14			nvarchar(14)			not null,	/* 	GTIN-14						*/
	ST_BOXTYPE			smallint				not null,	/* �^���e����
															0:���̑� 1:���� 2:���� 3:��	*/
	NU_BOXUNIT			integer					not null,	/* �^���e����萔				*/

	CD_HENKOSHA			nchar(10)				not null,	/* �ύX�҃R�[�h					*/
	DT_TOROKU_NICHIJI	nchar(14)				not null,	/* �o�^����						*/
	DT_KOSHIN_NICHIJI	nchar(14)				not null,	/* �X�V����						*/

	NU_OPS				integer					not null,	/* �o�ח\�萔					*/
	NU_LOPS				integer					not null,	/* ��d�����\�萔				*/
	NU_LRPS				integer					not null,	/* ��d�������ѐ�				*/
	NU_DOPS				integer					not null,	/* �z���\�萔					*/
	NU_DRPS				integer					not null,	/* �z�����ѐ�					*/

	FG_MAPSTATUS		smallint				not null,	/*	���ȃ}�b�s���O�X�e�[�^�X
																0:���ݒ�
																1:���ӂ�
																2:�}�b�s���O����			*/

	FG_LSTATUS			smallint				not null,	/*	��d�����X�e�[�^�X
																0:������
																1:���i
																2:����						*/

	FG_DSTATUS			smallint				not null,	/*	�d���X�e�[�^�X
																0:������
																1:���i
																2:����						*/

	CD_SHAIN_LARGE		nchar(7),							/* ��d������Ǝ�				*/
	NM_SHAIN_LARGE		nvarchar(40),						/* ��d������ƎҖ���			*/
	DT_WORKDT_LARGE		datetime,							/* ��d������Ɠ�				*/
	CD_SHAIN_DIST		nchar(7),							/* �z����Ǝ�					*/
	NM_SHAIN_DIST		nvarchar(40),						/* �z����ƎҖ���				*/
	DT_WORKDT_DIST		datetime,							/* �z����Ɠ�					*/

	DT_SENDDT_DIST		datetime,							/* ���ё��M����					*/

	createdAt 			datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 			datetime 				not null,	/*	�f�[�^�X�V��				*/

	constraint pk_TB_DIST primary key (ID_DIST)
);


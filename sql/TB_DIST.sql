/*
 *
 *	�o�׃f�[�^
 *
 *	$Id: dist.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST (
	ID_DIST				bigint identity(1,1) 	not null,	/* id							*/
	DT_DELIVERY			char(10)				not null,	/* �[�i��						*/
	
	CD_SHUKKA_BATCH		char(5)					not null,	/* �o�׃o�b�`					*/
	CD_KYOTEN			char(4)					not null,	/* �d�������_					*/
	CD_BIN				char(3)					not null,	/* �z����						*/
	CD_COURSE			char(3)					not null,	/* �R�[�X						*/
	CD_ROUTE			integer					not null,	/* �z����						*/
	CD_TOKUISAKI		char(6)					not null,	/* ���Ӑ�R�[�h					*/
	CD_HIMBAN			char(9)					not null,	/* 	�i��						*/
	CD_GTIN13			nvarchar(13)			not null,	/* 	JAN����(GTIN-13)			*/
	CD_GTIN14			nvarchar(14)			not null,	/* 	GTIN-14						*/
	ST_BOXTYPE			smallint				not null,	/* �^���e����
															0:���̑� 1:���� 2:���� 3:��	*/
	NM_BOXUNIT			integer					not null,	/* �^���e����萔				*/

	CD_SUM_TOKUISAKI	char(6)					not null,	/* �W�񓾈Ӑ�R�[�h				*/

	NM_OPS				integer					not null,	/* �o�ח\�萔					*/
	NM_LOPS				integer					not null,	/* ��d�����\�萔				*/
	NM_LRPS				integer					not null,	/* ��d�������ѐ�				*/
	NM_DOPS				integer					not null,	/* �z���\�萔					*/
	NM_DRPS				integer					not null,	/* �z�����ѐ�					*/

	NM_LSTATUS			smallint				not null,	/*	��d�����X�e�[�^�X
																0:������
																1:���i
																2:����						*/

	NM_DSTATUS			smallint				not null,	/*	�d���X�e�[�^�X
																0:������
																1:���i
																2:����						*/

	CD_BLOCK			char(2),							/* �u���b�N�R�[�h				*/
	CD_DIST_GROUP		char(5),							/* �d���O���[�v					*/
	CD_LARGE_GROUP		char(3),							/* ��d���O���[�v				*/

	tdunitaddrcode		nvarchar(10),						/* �A�h���XCD(�_���A�h���X)		*/

	CD_SHAIN_LARGE		char(7),							/* ��d������Ǝ�				*/
	DT_WORKDT_LARGE		datetime,							/* ��d������Ɠ�				*/
	CD_SHAIN_DIST		char(7),							/* �z����Ǝ�					*/
	DT_WORKDT_DIST		datetime,							/* �z����Ɠ�					*/

	DT_SENDDT_DIST		datetime,							/* ���ё��M����					*/

	createdAt 			datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 			datetime 				not null,	/*	�f�[�^�X�V��				*/

	constraint pk_TB_DIST primary key (ID_DIST)
);


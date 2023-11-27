/*
 *
 *	�d���O���[�v�i��
 *
 *	$Id: TB_DIST_GROUP_PROGRESS.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_GROUP_PROGRESS (
	ID_DIST_GROUP_PROGRESS	bigint identity(1,1) 	not null,	/* id ���_�A�d�����O���[�v�A�K�p�J�n���Ń��j�[�N	*/
	DT_DELIVERY				nchar(8)				not null,	/* �[�i��						*/
	CD_KYOTEN				nchar(4)				not null,	/* ���_�R�[�h					*/
	NM_KYOTEN				nvarchar(40)			not null,	/* ���_����						*/
	CD_DIST_GROUP			nchar(5)				not null,	/* �d���O���[�v					*/
	NM_DIST_GROUP			nvarchar(40)			not null,	/* �d���O���[�v����				*/

	ID_PC					integer,							/* PCID							*/
	CD_BLOCK				nchar(2),							/* �u���b�N�R�[�h				*/

	CD_SHAIN				nchar(7),							/* 	�Ј��R�[�h					*/
	NM_SHAIN				nvarchar(40),						/* 	�Ј�����					*/

	DT_START				datetime,							/* 	�J�n����					*/
	DT_END					datetime,							/* 	�I������					*/

	NU_OITEMCNT				datetime,							/* 	�\��A�C�e����				*/
	NU_RITEMCNT				datetime,							/*  �ς݃A�C�e����				*/

	NU_OPS					datetime,							/* 	�\��d������				*/
	NU_RPS					datetime,							/*  �ς݃A�C�e����				*/

	FG_DSTATUS				smallint				not null,	/*	�d����Ə��
																	0:������
																	1:���i
																	2:����						*/

	FG_WORKING				smallint				not null,	/*	��Ə��
																	0:�����
																	1:��ƒ�					*/


	createdAt 				datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 				datetime 				not null,	/*	�f�[�^�X�V��				*/

	constraint pk_TB_DIST_GROUP_PROGRESS primary key (ID_DIST_GROUP_PROGRESS)
);


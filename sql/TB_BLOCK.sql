/*
 *
 *	�u���b�N���i臒l�j
 *
 *	$Id: TB_BLOCK.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_BLOCK (
	ID_BLOCK				bigint identity(1,1) 	not null,	/* id ���_�A�u���b�N�R�[�h�A�K�p�J�n���Ń��j�[�N	*/
	CD_KYOTEN				nchar(4)				not null,	/* ���_							*/
	CD_BLOCK				nchar(2)				not null,	/* �u���b�N�R�[�h				*/
	ST_TDUNIT_TYPE			integer					not null,	/* �u���b�N���
																		5:�V�݁@6�F�I			*/

	NU_TDUNIT_CNT			integer					not null,	/* �\���퐔						*/
	NU_THRESHOLD			integer					not null,	/* 臒l							*/

	DT_TEKIYOKAISHI			nchar(8)				not null,	/* �K�p�J�n��					*/
	DT_TEKIYOMUKO			nchar(8)				not null,	/* �K�p������					*/

	CD_HENKOSHA				nchar(7)				not null,	/* �ύX�҃R�[�h					*/
	NM_HENKOSHA				nvarchar(40)			not null,	/* �ύX�Җ���					*/

	createdAt 				datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 				datetime 				not null,	/*	�f�[�^�X�V��				*/

	constraint pk_TB_BLOCK primary key (ID_BLOCK)
);


/*
 *
 *	��d�����i���b�N���
 *
 */

create table TB_LARGE_LOCK (
  CD_LARGE_GROUP	nchar(3)				not null,	/* ��d�����O���[�v				*/
	CD_SHUKKA_BATCH		nchar(5)				not null,	/* �o�׃o�b�`					*/
	CD_JUCHU_BIN		nchar(3)				not null,	/* �󒍕�						*/
	CD_HIMBAN			  nchar(9)				not null,	/* 	�i��						    */
	ID_PROCESS		  nvarchar(255)		not null,	/* �v���Z�XID						*/
	
	createdAt 		  datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 		  datetime 				not null,	/*	�f�[�^�X�V��				*/

	constraint pk_TB_LARGE_LOCK primary key (CD_LARGE_GROUP, CD_SHUKKA_BATCH, CD_JUCHU_BIN, CD_HIMBAN)
);

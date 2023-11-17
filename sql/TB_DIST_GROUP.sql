/*
 *
 *	�d���O���[�v
 *
 *	$Id: TB_DIST_GROUP.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_GROUP (
	ID_DIST_GROUP			bigint identity(1,1) 	not null,	/* id ���_�A�d�����O���[�v�A�K�p�J�n���Ń��j�[�N	*/
	CD_KYOTEN				char(4)					not null,	/* ���_�R�[�h					*/
	CD_DIST_GROUP			char(5)					not null,	/* �d���O���[�v					*/
	NM_DIST_GROUP			nvarchar(40)			not null,	/* �d���O���[�v����				*/
	CD_BIN_SUM				smallint				not null,	/* �z���֏W�v
																	1:�֏W�v���� 2:�֏W�v���Ȃ�	*/

	DT_TEKIYOKAISHI			char(8)					not null,	/* �K�p�J�n��					*/
	DT_TEKIYOMUKO			char(8)					not null,	/* �K�p������					*/

	CD_HENKOSHA				char(7)					not null,	/* 	�ύX�҃R�[�h				*/

	createdAt 				datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 				datetime 				not null,	/*	�f�[�^�X�V��				*/
	
	constraint pk_TB_DIST_GROUP primary key (ID_DIST_GROUP)
);


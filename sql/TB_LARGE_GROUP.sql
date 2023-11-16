/*
 *
 *	��d�����O���[�v
 *
 *	$Id: TB_LARGE_GROUP.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_LARGE_GROUP (
	ID_LARGE_GROUP			bigint identity(1,1) 	not null,	/* id ���_�A��d�����O���[�v�A�K�p�J�n���Ń��j�[�N	*/
	CD_KYOTEN				char(4)					not null,	/* ���_�R�[�h					*/
	CD_LARGE_GROUP			char(3)					not null,	/* ��d�����O���[�v				*/
	CD_LARGE_GROUP_NAME		nvarchar(40)			not null,	/* ��d�����O���[�v����			*/

	DT_TEKIYOKAISHI			char(8)					not null,	/* �K�p�J�n��					*/
	DT_TEKIYOMUKO			char(8)					not null,	/* �K�p������					*/

	CD_HENKOSHA				char(7)					not null,	/* 	�ύX�҃R�[�h				*/

	createdAt 				datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 				datetime 				not null,	/*	�f�[�^�X�V��				*/
	
	constraint pk_TB_LARGE_GROUP primary key (ID_LARGE_GROUP)
);


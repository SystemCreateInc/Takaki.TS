/*
 *
 *	�W�񓾈Ӑ���
 *
 *	$Id: TB_SUM_TOKUISAKI.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_SUM_TOKUISAKI (
	ID_SUM_TOKUISAKI		bigint identity(1,1) 	not null,	/* id ���_�A�W�񓾈Ӑ�R�[�h�A�K�p�J�n���Ń��j�[�N	*/
	CD_KYOTEN				nchar(4)				not null,	/* ���_�R�[�h					*/
	CD_SUM_TOKUISAKI		nchar(6)				not null,	/* �W�񓾈Ӑ�R�[�h(�e)			*/

	DT_TEKIYOKAISHI			nchar(8)				not null,	/* �K�p�J�n��					*/
	DT_TEKIYOMUKO			nchar(8)				not null,	/* �K�p������					*/

	CD_HENKOSHA				nchar(7)				not null,	/* �ύX�҃R�[�h					*/
	NM_HENKOSHA				nvarchar(40)			not null,	/* �ύX�Җ���					*/

	createdAt 				datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 				datetime 				not null,	/*	�f�[�^�X�V��				*/
	
	constraint pk_TB_SUM_TOKUISAKI primary key (ID_SUM_TOKUISAKI)
);


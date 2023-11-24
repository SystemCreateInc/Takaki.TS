/*
 *
 *	�o�b�u���b�N���
 *
 *	$Id: TB_PC.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_PC (
	ID_PC					integer					not null,	/* PCID							*/
	CD_BLOCK				char(2)					not null,	/* �u���b�N�R�[�h				*/

	CD_HENKOSHA				char(7),							/* �ύX�҃R�[�h					*/
	NM_HENKOSHA				nvarchar(40),						/* �ύX�Җ���					*/
	
	createdAt 				datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 				datetime 				not null,	/*	�f�[�^�X�V��				*/

	constraint pk_TB_PC primary key (ID_PC)
);


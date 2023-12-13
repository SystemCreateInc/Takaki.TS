/*
 *
 *	�d���u���b�N��
 *
 *	$Id: TB_DIST_BLOCK_SEQ.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_BLOCK_SEQ (
	ID_DIST_BLOCK			bigint 			 		not null,	/* id							*/
	
	NU_BLOCK_SEQ			integer					not null,	/* �u���b�N��					*/
	CD_BLOCK				nchar(3),							/* �u���b�N						*/
	CD_ADDR_FROM			nchar(4),							/* �J�n�A�h���X					*/
	CD_ADDR_TO				nchar(3),							/* �I���A�h���X					*/

	createdAt 				datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 				datetime 				not null,	/*	�f�[�^�X�V��				*/
	
	constraint fk_TB_DIST_BLOCK_SEQ foreign key (ID_DIST_BLOCK) references TB_DIST_BLOCK(ID_DIST_BLOCK) on delete cascade,
	
	constraint pk_TB_DIST_BLOCK_SEQ primary key (ID_DIST_BLOCK,NU_SEQ)
);


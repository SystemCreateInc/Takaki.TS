/*
 *
 *	�d���u���b�N�R�[�X��
 *
 *	$Id: TB_DIST_BLOCK_COURSE_SEQ.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_BLOCK_COURSE_SEQ (
	ID_DIST_BLOCK			bigint 			 		not null,	/* id							*/
	
	NM_COURSE_SEQ			integer					not null,	/* �R�[�X��						*/
	CD_COURSE				char(3),							/* �R�[�X						*/
	CD_ADDR_FROM			char(4),							/* �J�n�A�h���X					*/
	CD_ADDR_TO				char(3),							/* �I���A�h���X					*/

	createdAt 				datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 				datetime 				not null,	/*	�f�[�^�X�V��				*/
	
	constraint fk_TB_DIST_BLOCK_COURSE_SEQ foreign key (ID_DIST_BLOCK) references TB_DIST_BLOCK(ID_DIST_BLOCK) on delete cascade,
	
	constraint pk_TB_DIST_BLOCK_COURSE_SEQ primary key (ID_DIST_BLOCK,NM_COURSE_SEQ)
);


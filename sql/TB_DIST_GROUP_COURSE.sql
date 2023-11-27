/*
 *
 *	�d���O���[�v�P�ʂ̃R�[�X��
 *
 *	$Id: TB_DIST_GROUP_COURSE.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_GROUP_COURSE (
	ID_DIST_GROUP			bigint 			 		not null,	/* id							*/
	
	NU_COURSE_SEQ			integer					not null,	/* �R�[�X�\����					*/
	CD_COURSE				nchar(3)				not null,	/* �R�[�X						*/

	createdAt 				datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 				datetime 				not null,	/*	�f�[�^�X�V��				*/
	
	constraint fk_TB_DIST_GROUP_COURSE foreign key (ID_DIST_GROUP) references TB_DIST_GROUP(ID_DIST_GROUP) on delete cascade,
	
	constraint pk_TB_DIST_GROUP_COURSE primary key (ID_DIST_GROUP,NU_COURSE_SEQ)
);


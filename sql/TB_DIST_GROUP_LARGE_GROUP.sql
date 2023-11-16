/*
 *
 *	�d���O���[�v�P�ʂ̑�d���O���[�v
 *
 *	$Id: TB_DIST_GROUP_LARGE_GROUP.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_GROUP_LARGE_GROUP (
	ID_DIST_GROUP			bigint 			 		not null,	/* id							*/
	
	NM_LARGE_GROUP_SEQ		integer					not null,	/* ��d���O���[�v�\����			*/
	CD_LARGE_GROUP			char(3)					not null,	/* ��d���O���[�v				*/

	createdAt 				datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 				datetime 				not null,	/*	�f�[�^�X�V��				*/
	
	constraint fk_TB_DIST_GROUP_LARGE_GROUP foreign key (ID_DIST_GROUP) references TB_DIST_GROUP(ID_DIST_GROUP) on delete cascade,
	
	constraint pk_TB_DIST_GROUP_LARGE_GROUP primary key (ID_DIST_GROUP,NM_LARGE_GROUP_SEQ)
);


/*
 *
 *	�d���O���[�v�P�ʂ̏o�׃o�b�`
 *
 *	$Id: TB_DIST_GROUP_SHUKKA_BATCH.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_GROUP_SHUKKA_BATCH (
	ID_DIST_GROUP			bigint 			 		not null,	/* id							*/
	
	NM_SHUKKA_BATCH_SEQ		integer					not null,	/* �o�׃o�b�`�\����				*/
	CD_SHUKKA_BATCH			char(5)					not null,	/* �o�׃o�b�`					*/

	createdAt 				datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 				datetime 				not null,	/*	�f�[�^�X�V��				*/

	constraint fk_TB_DIST_GROUP_SHUKKA_BATCH foreign key (ID_DIST_GROUP) references TB_DIST_GROUP(ID_DIST_GROUP) on delete cascade,
	
	constraint pk_TB_DIST_GROUP_SHUKKA_BATCH primary key (ID_DIST_GROUP,NM_SHUKKA_BATCH_SEQ)
);


/*
 *
 *	仕分グループ単位の出荷バッチ
 *
 *	$Id: TB_DIST_GROUP_SHUKKA_BATCH.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_GROUP_SHUKKA_BATCH (
	ID_DIST_GROUP			bigint 			 		not null,	/* id							*/
	
	NM_SHUKKA_BATCH_SEQ		integer					not null,	/* 出荷バッチ表示順				*/
	CD_SHUKKA_BATCH			char(5)					not null,	/* 出荷バッチ					*/

	createdAt 				datetime 				not null,	/*	データ挿入日				*/
	updatedAt 				datetime 				not null,	/*	データ更新日				*/

	constraint fk_TB_DIST_GROUP_SHUKKA_BATCH foreign key (ID_DIST_GROUP) references TB_DIST_GROUP(ID_DIST_GROUP) on delete cascade,
	
	constraint pk_TB_DIST_GROUP_SHUKKA_BATCH primary key (ID_DIST_GROUP,NM_SHUKKA_BATCH_SEQ)
);


/*
 *
 *	仕分ブロックコース順
 *
 *	$Id: TB_DIST_BLOCK_COURSE_SEQ.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_BLOCK_COURSE_SEQ (
	ID_DIST_BLOCK			bigint 			 		not null,	/* id							*/
	
	NM_COURSE_SEQ			integer					not null,	/* コース順						*/
	CD_COURSE				char(3),							/* コース						*/
	CD_ADDR_FROM			char(4),							/* 開始アドレス					*/
	CD_ADDR_TO				char(3),							/* 終了アドレス					*/

	createdAt 				datetime 				not null,	/*	データ挿入日				*/
	updatedAt 				datetime 				not null,	/*	データ更新日				*/
	
	constraint fk_TB_DIST_BLOCK_COURSE_SEQ foreign key (ID_DIST_BLOCK) references TB_DIST_BLOCK(ID_DIST_BLOCK) on delete cascade,
	
	constraint pk_TB_DIST_BLOCK_COURSE_SEQ primary key (ID_DIST_BLOCK,NM_COURSE_SEQ)
);


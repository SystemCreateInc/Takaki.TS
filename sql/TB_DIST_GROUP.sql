/*
 *
 *	仕分グループ
 *
 *	$Id: TB_DIST_GROUP.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_GROUP (
	ID_DIST_GROUP			bigint identity(1,1) 	not null,	/* id 拠点、仕分けグループ、適用開始日でユニーク	*/
	CD_KYOTEN				char(4)					not null,	/* 拠点コード					*/
	CD_DIST_GROUP			char(5)					not null,	/* 仕分グループ					*/
	NM_DIST_GROUP			nvarchar(40)			not null,	/* 仕分グループ名称				*/
	CD_BIN_SUM				smallint				not null,	/* 配送便集計
																	1:便集計する 2:便集計しない	*/

	DT_TEKIYOKAISHI			char(8)					not null,	/* 適用開始日					*/
	DT_TEKIYOMUKO			char(8)					not null,	/* 適用無効日					*/

	CD_HENKOSHA				char(7)					not null,	/* 	変更者コード				*/

	createdAt 				datetime 				not null,	/*	データ挿入日				*/
	updatedAt 				datetime 				not null,	/*	データ更新日				*/
	
	constraint pk_TB_DIST_GROUP primary key (ID_DIST_GROUP)
);


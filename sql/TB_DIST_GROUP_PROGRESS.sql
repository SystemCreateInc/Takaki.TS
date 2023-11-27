/*
 *
 *	仕分グループ進捗
 *
 *	$Id: TB_DIST_GROUP_PROGRESS.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_GROUP_PROGRESS (
	ID_DIST_GROUP_PROGRESS	bigint identity(1,1) 	not null,	/* id 拠点、仕分けグループ、適用開始日でユニーク	*/
	DT_DELIVERY				nchar(8)				not null,	/* 納品日						*/
	CD_KYOTEN				nchar(4)				not null,	/* 拠点コード					*/
	NM_KYOTEN				nvarchar(40)			not null,	/* 拠点名称						*/
	CD_DIST_GROUP			nchar(5)				not null,	/* 仕分グループ					*/
	NM_DIST_GROUP			nvarchar(40)			not null,	/* 仕分グループ名称				*/

	ID_PC					integer,							/* PCID							*/
	CD_BLOCK				nchar(2),							/* ブロックコード				*/

	CD_SHAIN				nchar(7),							/* 	社員コード					*/
	NM_SHAIN				nvarchar(40),						/* 	社員名称					*/

	DT_START				datetime,							/* 	開始日時					*/
	DT_END					datetime,							/* 	終了日時					*/

	NU_OITEMCNT				datetime,							/* 	予定アイテム数				*/
	NU_RITEMCNT				datetime,							/*  済みアイテム数				*/

	NU_OPS					datetime,							/* 	予定仕分け個数				*/
	NU_RPS					datetime,							/*  済みアイテム数				*/

	FG_DSTATUS				smallint				not null,	/*	仕分作業状態
																	0:未処理
																	1:欠品
																	2:完了						*/

	FG_WORKING				smallint				not null,	/*	作業状態
																	0:未作業
																	1:作業中					*/


	createdAt 				datetime 				not null,	/*	データ挿入日				*/
	updatedAt 				datetime 				not null,	/*	データ更新日				*/

	constraint pk_TB_DIST_GROUP_PROGRESS primary key (ID_DIST_GROUP_PROGRESS)
);


/*
 *
 *	大仕分商品ロック情報
 *
 */

create table TB_LARGE_LOCK (
  CD_LARGE_GROUP	nchar(3)				not null,	/* 大仕分けグループ				*/
	CD_SHUKKA_BATCH		nchar(5)				not null,	/* 出荷バッチ					*/
	CD_JUCHU_BIN		nchar(3)				not null,	/* 受注便						*/
	CD_HIMBAN			  nchar(9)				not null,	/* 	品番						    */
	ID_PROCESS		  nvarchar(255)		not null,	/* プロセスID						*/
	
	createdAt 		  datetime 				not null,	/*	データ挿入日				*/
	updatedAt 		  datetime 				not null,	/*	データ更新日				*/

	constraint pk_TB_LARGE_LOCK primary key (CD_LARGE_GROUP, CD_SHUKKA_BATCH, CD_JUCHU_BIN, CD_HIMBAN)
);

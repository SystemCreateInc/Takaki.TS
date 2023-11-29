/*
 *
 *	積み付け
 *
 *	$Id: TB_STOWAGE.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_STOWAGE (
	ID_STOWAGE			bigint identity(1,1) 	not null,	/* id							*/
	DT_DELIVERY			nchar(8)				not null,	/* 納品日						*/
	
	CD_SHUKKA_BATCH		nchar(5)				not null,	/* 出荷バッチ					*/
	CD_KYOTEN			nchar(4)				not null,	/* 仕分け拠点					*/
	CD_BIN				nchar(3)				not null,	/* 配送便						*/
	CD_COURSE			nchar(3)				not null,	/* コース						*/
	CD_ROUTE			integer					not null,	/* 配送順						*/
	CD_TOKUISAKI		nchar(6)				not null,	/* 得意先コード					*/

	FG_SSTATUS			smallint				not null,	/*	積み付け作業状態
																	0:未処理
																	2:完了					*/


	createdAt 			datetime 				not null,	/*	データ挿入日				*/
	updatedAt 			datetime 				not null,	/*	データ更新日				*/

	constraint pk_TB_STOWAGE primary key (ID_STOWAGE)
);


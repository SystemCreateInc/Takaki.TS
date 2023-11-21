/*
 *
 *	積み付け
 *
 *	$Id: TB_STOWAGE.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_STOWAGE (
	ID_STOWAGE			bigint identity(1,1) 	not null,	/* id							*/
	DT_DELIVERY			char(8)					not null,	/* 納品日						*/
	
	CD_SHUKKA_BATCH		char(5)					not null,	/* 出荷バッチ					*/
	CD_KYOTEN			char(4)					not null,	/* 仕分け拠点					*/
	CD_BIN				char(3)					not null,	/* 配送便						*/
	CD_COURSE			char(3)					not null,	/* コース						*/
	CD_ROUTE			integer					not null,	/* 配送順						*/
	CD_TOKUISAKI		char(6)					not null,	/* 得意先コード					*/


	CD_BLOCK			char(2)					not null,	/* ブロックコード				*/
	CD_DIST_GROUP		char(5)					not null,	/* 仕分グループ					*/

	CD_SUM_TOKUISAKI	char(6)					not null,	/* 集約得意先コード				*/

	tdunitaddrcode		nvarchar(10)			not null,	/* アドレスCD(論理アドレス)		*/

	FG_SSTATUS			smallint				not null,	/*	積み付け作業状態
																	0:未処理
																	2:完了					*/


	createdAt 			datetime 				not null,	/*	データ挿入日				*/
	updatedAt 			datetime 				not null,	/*	データ更新日				*/

	constraint pk_TB_STOWAGE primary key (ID_STOWAGE)
);


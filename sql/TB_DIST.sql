/*
 *
 *	出荷データ
 *
 *	$Id: dist.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST (
	ID_DIST				bigint identity(1,1) 	not null,	/* id							*/
	DT_DELIVERY			char(10)				not null,	/* 納品日						*/
	
	CD_SHUKKA_BATCH		char(5)					not null,	/* 出荷バッチ					*/
	CD_KYOTEN			char(4)					not null,	/* 仕分け拠点					*/
	CD_BIN				char(3)					not null,	/* 配送便						*/
	CD_COURSE			char(3)					not null,	/* コース						*/
	CD_ROUTE			integer					not null,	/* 配送順						*/
	CD_TOKUISAKI		char(6)					not null,	/* 得意先コード					*/
	CD_HIMBAN			char(9)					not null,	/* 	品番						*/
	CD_GTIN13			nvarchar(13)			not null,	/* 	JANｺｰﾄﾞ(GTIN-13)			*/
	CD_GTIN14			nvarchar(14)			not null,	/* 	GTIN-14						*/
	ST_BOXTYPE			smallint				not null,	/* 運搬容器種別
															0:その他 1:薄箱 2:圧箱 3:青箱	*/
	NM_BOXUNIT			integer					not null,	/* 運搬容器入り数				*/

	CD_SUM_TOKUISAKI	char(6)					not null,	/* 集約得意先コード				*/

	NM_OPS				integer					not null,	/* 出荷予定数					*/
	NM_LOPS				integer					not null,	/* 大仕分け予定数				*/
	NM_LRPS				integer					not null,	/* 大仕分け実績数				*/
	NM_DOPS				integer					not null,	/* 配分予定数					*/
	NM_DRPS				integer					not null,	/* 配分実績数					*/

	NM_LSTATUS			smallint				not null,	/*	大仕分けステータス
																0:未処理
																1:欠品
																2:完了						*/

	NM_DSTATUS			smallint				not null,	/*	仕分ステータス
																0:未処理
																1:欠品
																2:完了						*/

	CD_BLOCK			char(2),							/* ブロックコード				*/
	CD_DIST_GROUP		char(5),							/* 仕分グループ					*/
	CD_LARGE_GROUP		char(3),							/* 大仕分グループ				*/

	tdunitaddrcode		nvarchar(10),						/* アドレスCD(論理アドレス)		*/

	CD_SHAIN_LARGE		char(7),							/* 大仕分け作業者				*/
	DT_WORKDT_LARGE		datetime,							/* 大仕分け作業日				*/
	CD_SHAIN_DIST		char(7),							/* 配分作業者					*/
	DT_WORKDT_DIST		datetime,							/* 配分作業日					*/

	DT_SENDDT_DIST		datetime,							/* 実績送信日時					*/

	createdAt 			datetime 				not null,	/*	データ挿入日				*/
	updatedAt 			datetime 				not null,	/*	データ更新日				*/

	constraint pk_TB_DIST primary key (ID_DIST)
);


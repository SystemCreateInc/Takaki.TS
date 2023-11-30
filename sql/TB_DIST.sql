/*
 *
 *	出荷データ
 *
 *	$Id: TB_DIST.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST (
	ID_DIST				bigint identity(1,1) 	not null,	/* id							*/
	DT_DELIVERY			nchar(8)					not null,	/* 納品日						*/
	
	CD_SHUKKA_BATCH		nchar(5)				not null,	/* 出荷バッチ					*/
	CD_KYOTEN			nchar(4)				not null,	/* 仕分け拠点					*/
	CD_BIN				nchar(3)				not null,	/* 配送便						*/
	CD_COURSE			nchar(3)				not null,	/* コース						*/
	CD_ROUTE			integer					not null,	/* 配送順						*/
	CD_TOKUISAKI		nchar(6)				not null,	/* 得意先コード					*/
	CD_HIMBAN			nchar(9)				not null,	/* 	品番						*/
	CD_GTIN13			nvarchar(13)			not null,	/* 	JANｺｰﾄﾞ(GTIN-13)			*/
	CD_GTIN14			nvarchar(14)			not null,	/* 	GTIN-14						*/
	ST_BOXTYPE			smallint				not null,	/* 運搬容器種別
															0:その他 1:薄箱 2:圧箱 3:青箱	*/
	NU_BOXUNIT			integer					not null,	/* 運搬容器入り数				*/

	CD_HENKOSHA			nchar(10)				not null,	/* 変更者コード					*/
	DT_TOROKU_NICHIJI	nchar(14)				not null,	/* 登録日時						*/
	DT_KOSHIN_NICHIJI	nchar(14)				not null,	/* 更新日時						*/

	NU_OPS				integer					not null,	/* 出荷予定数					*/
	NU_LOPS				integer					not null,	/* 大仕分け予定数				*/
	NU_LRPS				integer					not null,	/* 大仕分け実績数				*/
	NU_DOPS				integer					not null,	/* 配分予定数					*/
	NU_DRPS				integer					not null,	/* 配分実績数					*/

	FG_MAPSTATUS		smallint				not null,	/*	座席マッピングステータス
																0:未設定
																1:あふれ
																2:マッピング完了			*/

	FG_LSTATUS			smallint				not null,	/*	大仕分けステータス
																0:未処理
																1:欠品
																2:完了						*/

	FG_DSTATUS			smallint				not null,	/*	仕分ステータス
																0:未処理
																1:欠品
																2:完了						*/

	CD_SHAIN_LARGE		nchar(7),							/* 大仕分け作業者				*/
	NM_SHAIN_LARGE		nvarchar(40),						/* 大仕分け作業者名称			*/
	DT_WORKDT_LARGE		datetime,							/* 大仕分け作業日				*/
	CD_SHAIN_DIST		nchar(7),							/* 配分作業者					*/
	NM_SHAIN_DIST		nvarchar(40),						/* 配分作業者名称				*/
	DT_WORKDT_DIST		datetime,							/* 配分作業日					*/

	DT_SENDDT_DIST		datetime,							/* 実績送信日時					*/

	createdAt 			datetime 				not null,	/*	データ挿入日				*/
	updatedAt 			datetime 				not null,	/*	データ更新日				*/

	constraint pk_TB_DIST primary key (ID_DIST)
);


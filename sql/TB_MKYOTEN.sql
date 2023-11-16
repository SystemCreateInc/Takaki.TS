/*
 *
 *	拠点マスタ
 *
 *	$Id: TB_MKYOTEN.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_MKYOTEN (
	CD_KYOTEN						char			(4)			not null,	/* 	拠点コード						*/
	DT_TEKIYOKAISHI					char			(8)			not null,	/* 	適用開始日						*/
	DT_TEKIYOMUKO					char			(8)			not null,	/* 	適用無効日						*/
	DT_TOROKU_NICHIJI				char			(14)		not null,	/* 	登録日時						*/
	DT_KOSHIN_NICHIJI				char			(14)		not null,	/* 	更新日時						*/
	CD_HENKOSHA						char			(10)		not null,	/* 	変更者コード					*/
	NM_KYOTEN						nvarchar		(40)		not null,	/* 	拠点名称						*/
	CD_KYOTEN_SHUBETSU				char			(3)			not null,	/* 	拠点種別						*/
	CD_TORIHIKISAKI					char			(6)			not null,	/* 	取引先コード					*/
	CD_ZAIKO_HIKIATE_BUMON			char			(6)			not null,	/* 	在庫引当部門					*/
	NM_KYOTEN_RYAKUSHO				nvarchar		(20)		not null,	/* 	拠点略称						*/
	CD_KYOTEN_BUMON					char			(6)			not null,	/* 	拠点部門コード					*/
	ST_SESANKANRI_NIPPAIHIN			char			(1)			not null,	/* 	生産管理日配品区分				*/
	ST_SEISANKANRI_ZAIKOHIN			char			(1)			not null,	/* 	生産管理在庫品区分				*/
	ST_SHIKIBETSU					char			(1)			not null,	/* 	識別区分						*/
	CD_TENPO_BRAND					char			(2)			not null,	/* 	店舗ブランドコード				*/
	CD_BASHO						char			(4)			not null,	/* 	場所コード						*/
	CD_KYOTEN_ZOKUSEI				char			(3)			not null,	/* 	拠点属性コード					*/
	NU_HAITA						numeric			(5)			not null,	/* 	排他カウンタ					*/
	DT_RENKEI						char			(14)		not null,	/* 	連携日時						*/
	
	createdAt datetime not null,		/*	データ挿入日				*/
	updatedAt datetime not null,		/*	データ更新日				*/

	constraint pk_TB_MKYOTEN primary key (CD_KYOTEN,DT_TEKIYOKAISHI)
);



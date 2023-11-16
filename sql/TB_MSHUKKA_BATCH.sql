/*
 *
 *	出荷バッチマスタ
 *
 *	$Id: TB_MSHUKKA_BATCH.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_MSHUKKA_BATCH (
	
	CD_SHUKKA_BATCH					char			(5)			not null,	/* 	出荷バッチコード				*/
	DT_TEKIYOKAISHI					char			(8)			not null,	/* 	適用開始日						*/
	DT_TEKIYOMUKO					char			(8)			not null,	/* 	適用無効日						*/
	DT_TOROKU_NICHIJI				char			(14)		not null,	/* 	登録日時						*/
	DT_KOSHIN_NICHIJI				char			(14)		not null,	/* 	更新日時						*/
	CD_HENKOSHA						char			(10)		not null,	/* 	変更者コード					*/
	NM_SHUKKA_BATCH					nvarchar		(40)		not null,	/* 	出荷バッチ名称					*/
	CD_SHIWAKE_KYOTEN				char			(4)			not null,	/* 	仕分拠点コード					*/
	TM_SHIWAKE_KAISHI				char			(4)			not null,	/* 	仕分開始時刻					*/
	TM_SHIWAKE_SHURYO				char			(4)			not null,	/* 	仕分終了時刻					*/
	CD_SHUKKA_BATCH_GROUP			char			(6)			not null,	/* 	出荷バッチグループ				*/
	ST_OSHIWAKEHYO_SHUBETSU			char			(1)			not null,	/* 	大仕分表種別区分				*/
	ST_CHUSHIWAKEHYO_SHUBETSU		char			(1)			not null,	/* 	中仕分表種別区分				*/
	ST_KOSHIWAKEHYO_SHUBETSU		char			(1)			not null,	/* 	小仕分表種別区分				*/
	FG_TOKUISAKI_SHUKEI_1			char			(1)			not null,	/* 	得意先別集計出力フラグ１		*/
	FG_TOKUISAKI_SHUKEI_2			char			(1)			not null,	/* 	得意先別集計出力フラグ２		*/
	FG_TOKUISAKI_SHUKEI_3			char			(1)			not null,	/* 	得意先別集計出力フラグ３		*/
	FG_TOKUISAKI_SHUKEI_4			char			(1)			not null,	/* 	得意先別集計出力フラグ４		*/
	FG_TOKUISAKI_SHUKEI_5			char            (1)			not null,	/* 	得意先別集計出力フラグ５		*/
	FG_TOKUISAKI_SHUKEI_6			char            (1)			not null,	/* 	得意先別集計出力フラグ６		*/
	FG_HAISOUBIN_BETSU_SHUKEI_UMU	char            (1)			not null,	/* 	配送便別受注集計出力有無		*/
	FG_COURSE_BETSU_SHUKEI_UMU		char            (1)			not null,	/* 	コース別受注集計出力有無		*/
	FG_JUCHUJOKYOHYO				char            (1)			not null,	/* 	受注状況表出力フラグ			*/
	FG_SAND_HIKITORI_HYO			char            (1)			not null,	/* 	サンド引取表出力フラグ			*/
	ST_DPS_SHUBETSU					char            (2)			not null,	/* 	ＤＰＳ種別						*/
	NU_SHIWAKE_LEAD_TIME			numeric         (3)			not null,	/* 	仕分リードタイム				*/
	ST_LEAD_TIME_SEIGYO				char            (1)			not null,	/* 	リードタイム制御区分			*/
	FG_YOBI_1						char            (1)			not null,	/* 	予備フラグ１					*/
	FG_YOBI_2						char            (1)			not null,	/* 	予備フラグ２					*/
	FG_YOBI_3						char            (1)			not null,	/* 	予備フラグ３					*/
	FG_YOBI_4						char            (1)			not null,	/* 	予備フラグ４					*/
	FG_YOBI_5						char            (1)			not null,	/* 	予備フラグ５					*/
	ST_YOBI_1						char            (1)			not null,	/* 	予備区分１						*/
	ST_YOBI_2						char            (1)			not null,	/* 	予備区分２						*/
	ST_YOBI_3						char            (1)			not null,	/* 	予備区分３						*/
	ST_YOBI_4						char            (1)			not null,	/* 	予備区分４						*/
	ST_YOBI_5						char            (1)			not null,	/* 	予備区分５						*/
	NU_HAITA						numeric         (5)			not null,	/* 	排他カウンタ					*/
	DT_RENKEI						char            (14)		not null,	/* 	連携日時						*/
	
	createdAt datetime not null,		/*	データ挿入日				*/
	updatedAt datetime not null,		/*	データ更新日				*/

	constraint pk_TB_MSHUKKA_BATCH primary key (CD_SHUKKA_BATCH	,DT_TEKIYOKAISHI)
);


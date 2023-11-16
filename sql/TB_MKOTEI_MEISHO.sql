/*
 *
 *	固定名称マスタ
 *
 *	$Id: TB_MKOTEI_MEISHO.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_MKOTEI_MEISHO (

	CD_MEISHO_SHIKIBETSU			char			(5)			not null,	/* 	名称識別						*/
	CD_MEISHO						nvarchar		(8)			not null,	/* 	名称コード						*/
	DT_TOROKU_NICHIJI				char			(14)		not null,	/* 	登録日時						*/
	DT_KOSHIN_NICHIJI				char			(14)		not null,	/* 	更新日時						*/
	CD_HENKOSHA						char            (10)		not null,	/* 	変更者コード					*/
	NM								nvarchar        (40)		not null,	/* 	名称							*/
	NM_YOMI							nvarchar        (20)		not null,	/* 	名称読み						*/
	NM_RYAKU						nvarchar        (20)		not null,	/* 	略称							*/
	NM_RYAKU_YOMI					nvarchar        (5)			not null,	/* 	略称読み						*/
	CD_KYU_MEISHO_SHIKIBETSU		char            (5)			not null,	/* 	旧名称識別						*/
	CD_KYU_MEISHO					char            (8)			not null,	/* 	旧名称コード					*/
	CD_EX_1							char            (10)		not null,	/* 	拡張コード１					*/
	CD_EX_2							char            (10)		not null,	/* 	拡張コード２					*/
	CD_EX_3							char            (10)		not null,	/* 	拡張コード３					*/
	CD_EX_4							char            (10)		not null,	/* 	拡張コード４					*/
	CD_EX_5							char            (10)		not null,	/* 	拡張コード５					*/
	FG_EX_1							char            (1)			not null,	/* 	拡張フラグ１					*/
	FG_EX_2							char            (1)			not null,	/* 	拡張フラグ２					*/
	FG_EX_3							char            (1)			not null,	/* 	拡張フラグ３					*/
	FG_EX_4							char            (1)			not null,	/* 	拡張フラグ４					*/
	FG_EX_5							char            (1)			not null,	/* 	拡張フラグ５					*/
	NU_HAITA						numeric         (5)			not null,	/* 	排他カウンタ					*/
	DT_RENKEI						char            (14)		not null,	/* 	連携日時						*/
	
	createdAt datetime not null,		/*	データ挿入日				*/
	updatedAt datetime not null,		/*	データ更新日				*/

	constraint pk_TB_MKOTEI_MEISHO primary key (CD_MEISHO_SHIKIBETSU,CD_MEISHO)
);

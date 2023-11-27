/*
 *
 *	ブロック情報（閾値）
 *
 *	$Id: TB_BLOCK.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_BLOCK (
	ID_BLOCK				bigint identity(1,1) 	not null,	/* id 拠点、ブロックコード、適用開始日でユニーク	*/
	CD_KYOTEN				nchar(4)				not null,	/* 拠点							*/
	CD_BLOCK				nchar(2)				not null,	/* ブロックコード				*/
	ST_TDUNIT_TYPE			integer					not null,	/* ブロック種別
																		5:天吊　6：棚			*/

	NU_TDUNIT_CNT			integer					not null,	/* 表示器数						*/
	NU_THRESHOLD			integer					not null,	/* 閾値							*/

	DT_TEKIYOKAISHI			nchar(8)				not null,	/* 適用開始日					*/
	DT_TEKIYOMUKO			nchar(8)				not null,	/* 適用無効日					*/

	CD_HENKOSHA				nchar(7)				not null,	/* 変更者コード					*/
	NM_HENKOSHA				nvarchar(40)			not null,	/* 変更者名称					*/

	createdAt 				datetime 				not null,	/*	データ挿入日				*/
	updatedAt 				datetime 				not null,	/*	データ更新日				*/

	constraint pk_TB_BLOCK primary key (ID_BLOCK)
);


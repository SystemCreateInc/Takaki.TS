/*
 *
 *	出荷データマッピング結果
 *
 *	$Id: TB_DIST_MAPPING.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_MAPPING (
	ID_DIST				bigint 	not null,					/* id							*/
	
	NM_SHUKKA_BATCH		nvarchar(40),						/* 出荷バッチ名称				*/
	NM_KYOTEN			nvarchar(40),						/* 拠点名称						*/
	NM_TOKUISAKI		nvarchar(40),						/* 得意先名						*/
	NM_HIN_SEISHIKIMEI	nvarchar(60),						/* 品名（正式名）				*/

	CD_SUM_TOKUISAKI	nchar(6),							/* 集約得意先コード(親)			*/
	NM_SUM_TOKUISAKI	nvarchar(40),						/* 集約得意先名					*/

	CD_BLOCK			nchar(2),							/* ブロックコード				*/
	CD_DIST_GROUP		nchar(5),							/* 仕分グループ					*/
	NM_DIST_GROUP		nvarchar(40),						/* 仕分グループ名称				*/
	CD_LARGE_GROUP		nchar(3),							/* 大仕分グループ				*/
	NM_LARGE_GROUP		nvarchar(40),						/* 大仕分グループ名称			*/

	tdunitaddrcode		nvarchar(10),						/* アドレスCD(論理アドレス)		*/

	createdAt 			datetime 				not null,	/*	データ挿入日				*/
	updatedAt 			datetime 				not null,	/*	データ更新日				*/

	constraint fk_TB_DIST_MAPPING foreign key (ID_DIST) references TB_DIST(ID_DIST) on delete cascade,
	
	constraint pk_TB_DIST_MAPPING primary key (ID_DIST)
);


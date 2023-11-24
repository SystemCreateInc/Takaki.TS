/*
 *
 *	集約得意先情報
 *
 *	$Id: TB_SUM_TOKUISAKI.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_SUM_TOKUISAKI (
	ID_SUM_TOKUISAKI		bigint identity(1,1) 	not null,	/* id 拠点、集約得意先コード、適用開始日でユニーク	*/
	CD_KYOTEN				char(4)					not null,	/* 拠点コード					*/
	CD_SUM_TOKUISAKI		char(6)					not null,	/* 集約得意先コード(親)			*/

	DT_TEKIYOKAISHI			char(8)					not null,	/* 適用開始日					*/
	DT_TEKIYOMUKO			char(8)					not null,	/* 適用無効日					*/

	CD_HENKOSHA				char(7)					not null,	/* 変更者コード					*/
	NM_HENKOSHA				nvarchar(40)			not null,	/* 変更者名称					*/

	createdAt 				datetime 				not null,	/*	データ挿入日				*/
	updatedAt 				datetime 				not null,	/*	データ更新日				*/
	
	constraint pk_TB_SUM_TOKUISAKI primary key (ID_SUM_TOKUISAKI)
);


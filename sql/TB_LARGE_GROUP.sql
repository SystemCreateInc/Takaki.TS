/*
 *
 *	大仕分けグループ
 *
 *	$Id: TB_LARGE_GROUP.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_LARGE_GROUP (
	ID_LARGE_GROUP			bigint identity(1,1) 	not null,	/* id 拠点、大仕分けグループ、適用開始日でユニーク	*/
	CD_KYOTEN				nchar(4)				not null,	/* 拠点コード					*/
	CD_LARGE_GROUP			nchar(3)				not null,	/* 大仕分けグループ				*/
	CD_LARGE_GROUP_NAME		nvarchar(40)			not null,	/* 大仕分けグループ名称			*/

	DT_TEKIYOKAISHI			nchar(8)				not null,	/* 適用開始日					*/
	DT_TEKIYOMUKO			nchar(8)				not null,	/* 適用無効日					*/

	CD_HENKOSHA				nchar(7)				not null,	/* 変更者コード					*/
	NM_HENKOSHA				nvarchar(40)			not null,	/* 変更者名称					*/

	createdAt 				datetime 				not null,	/*	データ挿入日				*/
	updatedAt 				datetime 				not null,	/*	データ更新日				*/
	
	constraint pk_TB_LARGE_GROUP primary key (ID_LARGE_GROUP)
);


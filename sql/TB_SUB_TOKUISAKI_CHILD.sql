/*
 *
 *	集約得意先情報
 *
 *	$Id: TB_SUM_TOKUISAKI_CHILD.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_SUM_TOKUISAKI_CHILD (
	ID_SUM_TOKUISAKI		bigint 					not null,	/* id 拠点、集約得意先コード、適用開始日でユニーク	*/
	CD_TOKUISAKI_CHILD		char(6)					not null,	/* 集約得意先コード				*/

	createdAt 				datetime 				not null,	/*	データ挿入日				*/
	updatedAt 				datetime 				not null,	/*	データ更新日				*/
	
	constraint fk_TB_SUM_TOKUISAKI_CHILD foreign key (ID_SUM_TOKUISAKI) references TB_SUM_TOKUISAKI(ID_SUM_TOKUISAKI) on delete cascade,
	
	constraint pk_TB_SUM_TOKUISAKI_CHILD primary key (ID_SUM_TOKUISAKI,	CD_TOKUISAKI_CHILD)
);


/*
 *
 *	積み付け箱単位
 *
 *	$Id: TB_STOWAGE_BOX.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_STOWAGE_BOX (
	ID_STOWAGE			bigint 					not null,	/* id							*/
	ST_BOXTYPE			smallint				not null,	/* 運搬容器種別
															0:その他 1:薄箱 2:圧箱 3:青箱	*/
	NU_OBOXCNT			integer					not null,	/* 予定箱数						*/
	NU_RBOXCNT			integer					not null,	/* 実績箱数						*/

	CD_HENKOSHA			nchar(7)					not null,	/* 変更者コード					*/
	NM_HENKOSHA			nvarchar(40)			not null,	/* 変更者名称					*/

	createdAt 			datetime 				not null,	/*	データ挿入日				*/
	updatedAt 			datetime 				not null,	/*	データ更新日				*/

	constraint fk_TB_STOWAGE_BOX foreign key (ID_STOWAGE) references TB_STOWAGE(ID_STOWAGE) on delete cascade,
	
	constraint pk_TB_STOWAGE_BOX primary key (ID_STOWAGE,ST_BOXTYPE)
);


/*
 *
 *	作業履歴
 *
 *	$Id: TB_REPORT.sql,v 1.1.1.1 2004/05/06 04:02:04 ohki Exp $
 */

create table TB_REPORT (
	ID_REPORT		integer identity not null,	/* id						*/
	CD_DIST_GROUP	char(5)			not null,	/* 仕分グループ				*/
	NM_DIST_GROUP	nvarchar(40)	not null,	/* 仕分グループ名称			*/
	CD_BLOCK		char(2)			not null,	/* ブロックコード			*/
	DT_START		datetime		not null,	/* 作業開始日時				*/
	DT_END			datetime		not null,	/* 作業終了日時				*/
	NM_IDLE			integer			not null,	/* 休憩時間（秒）			*/
	CD_SYAIN		char(7)			not null,	/* 社員コード				*/
	NM_SYAIN		nvarchar(40)	not null,	/* 社員名称					*/
	DT_WORKSTART	datetime		not null,	/* 作業開始時間				*/
	DT_WORKEND		datetime		not null,	/* 作業終了時間				*/
	NM_WORKTIME		integer			not null,	/* 稼動時間（秒）			*/
	NM_ITEMCNT		integer			not null,	/* 商品数					*/
	NM_SHOPCNT		integer			not null,	/* 店舗数					*/
	NM_DISTCNT		integer			not null,	/* 配分数					*/
	NM_CHECKCNT		integer			not null,	/* 検品回数					*/
	NM_CHECKTIME	integer			not null,	/* 検品時間（秒）			*/
	
	createdAt 		datetime 		not null,	/*	データ挿入日			*/
	
	constraint pk_TB_REPORT primary key (ID_REPORT)
);

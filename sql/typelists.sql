/*
 *
 *	タイプ一覧(コンボ表示等に使用)
 *
 *	$Id: typelists.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table typelists (
	typefield		nvarchar(100)	not null,	/* タイプフィールド名		*/
	typecode		nvarchar(100)	not null,	/* コード					*/
	typename		nvarchar(100)	not null,	/* 名称						*/
	typedispseq		integer			not null,	/* 表示順序					*/
	updt			datetime default getdate(),	/*  更新時刻				*/
	indt			datetime default getdate(), /*  挿入時刻				*/

	constraint pk_typelists primary key (typefield,typecode)
);

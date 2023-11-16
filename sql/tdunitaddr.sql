/*
 *
 *	TD表示器アドレス
 *
 *	$Id: tdunitaddr.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table tdunitaddr (
	tdunitaddrcode		nvarchar(10)		not null,	/* アドレスCD(論理アドレス)	*/
	tdunitcode			nvarchar(10)		not null,	/* 表示器CD					*/
	tdunitareacode		integer,						/* エリア表示灯CD			*/
	tdunitzonecode		integer,						/* ゾーンCD					*/
	tdunitseq			integer,						/* 優先順位					*/
	usageid				integer,						/* 用途ID					*/
	
	updt			datetime default getdate(),	/*  更新時刻				*/
	indt			datetime default getdate(), /*  挿入時刻				*/

	constraint fk_tdaddr foreign key (tdunitcode) references tdunitmst(tdunitcode) on delete cascade,
	
	constraint pk_tdaddr primary key (tdunitcode)
);

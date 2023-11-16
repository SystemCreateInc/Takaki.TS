/*
 *
 *	TD表示器No
 *
 *	$Id: tdunitmst.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table tdunitmst (
	tdunitcode		nvarchar(10)	not null,	/* 表示器NO					*/
	tdunitportcode	smallint		not null,	/* ポートCD					*/
	tdunitgroup		integer			not null,	/* 表示器グループ			*/
	tdunitaddr		integer			not null,	/* 表示器アドレス			*/
	tdunitbutton	integer			not null,	/* 表示器ボタン数			*/
	
	updt			datetime default getdate(),	/*  更新時刻				*/
	indt			datetime default getdate(), /*  挿入時刻				*/

	constraint pk_tdunitmst primary key (tdunitcode)
);

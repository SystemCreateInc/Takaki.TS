/*
 *
 *	TD表示器ポート
 *
 *	$Id: tdunitport.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table tdunitport (
	tdunitportcode	smallint		not null,	/* ポートCD					*/
	tdunitportcom	nvarchar(40)		not null,	/* ポート番号				*/
	tdunitporttype	smallint		not null,	/* ポート区分				*/
	updt			datetime default getdate(),	/*  更新時刻				*/
	indt			datetime default getdate(), /*  挿入時刻				*/

	constraint pk_tdunitport primary key (tdunitportcode)
);

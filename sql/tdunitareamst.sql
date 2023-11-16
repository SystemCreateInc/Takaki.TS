/*
 *
 *	TD表示器エリア表示灯マスタ
 *
 *	$Id: tdunitareamst.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table tdunitareamst (
	tdunitareacode		integer			not null,	/* エリア表示灯CD			*/
	tdunitareaname		nvarchar(40)	not null,	/* エリア表示灯CD名称		*/
	
	updt			datetime default getdate(),	/*  更新時刻				*/
	indt			datetime default getdate(), /*  挿入時刻				*/

	constraint pk_tdunitareamst primary key (tdunitareacode)
);

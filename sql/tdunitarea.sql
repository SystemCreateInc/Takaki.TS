/*
 *
 *	TD表示器エリア表示灯
 *
 *	$Id: tdunitarea.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table tdunitarea (
	tdunitcode		nvarchar(10)	not null,	/* 表示器CD					*/
	tdunitareacode	integer			not null,	/* エリア表示灯CD			*/
	
	updt			datetime default getdate(),	/*  更新時刻				*/
	indt			datetime default getdate(), /*  挿入時刻				*/

	constraint fk_tdarea foreign key (tdunitcode) references tdunitmst(tdunitcode) on delete cascade,
	
	constraint pk_tdarea primary key (tdunitcode)
);

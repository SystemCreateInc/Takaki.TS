/*
 *
 *	�Z�b�e�B���O
 *
 *	$Id: settings.sql,v 1.1 2005/11/15 04:28:08 hiro Exp $
 */

create table settings (
	value			nvarchar(50)	not null,	/* ����						*/
	data			nvarchar(255),				/* �l						*/
	id				nvarchar(255)	not null,	/* id						*/
	comment			nvarchar(max),				/* comment					*/
	
	constraint pk_settings primary key (value, id)
);

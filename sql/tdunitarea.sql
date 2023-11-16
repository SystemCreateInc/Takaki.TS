/*
 *
 *	TD�\����G���A�\����
 *
 *	$Id: tdunitarea.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table tdunitarea (
	tdunitcode		nvarchar(10)	not null,	/* �\����CD					*/
	tdunitareacode	integer			not null,	/* �G���A�\����CD			*/
	
	updt			datetime default getdate(),	/*  �X�V����				*/
	indt			datetime default getdate(), /*  �}������				*/

	constraint fk_tdarea foreign key (tdunitcode) references tdunitmst(tdunitcode) on delete cascade,
	
	constraint pk_tdarea primary key (tdunitcode)
);

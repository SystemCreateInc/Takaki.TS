/*
 *
 *	TD�\����G���A�\�����}�X�^
 *
 *	$Id: tdunitareamst.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table tdunitareamst (
	tdunitareacode		integer			not null,	/* �G���A�\����CD			*/
	tdunitareaname		nvarchar(40)	not null,	/* �G���A�\����CD����		*/
	
	updt			datetime default getdate(),	/*  �X�V����				*/
	indt			datetime default getdate(), /*  �}������				*/

	constraint pk_tdunitareamst primary key (tdunitareacode)
);

/*
 *
 *	TD�\����No
 *
 *	$Id: tdunitmst.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table tdunitmst (
	tdunitcode		nvarchar(10)	not null,	/* �\����NO					*/
	tdunitportcode	smallint		not null,	/* �|�[�gCD					*/
	tdunitgroup		integer			not null,	/* �\����O���[�v			*/
	tdunitaddr		integer			not null,	/* �\����A�h���X			*/
	tdunitbutton	integer			not null,	/* �\����{�^����			*/
	
	updt			datetime default getdate(),	/*  �X�V����				*/
	indt			datetime default getdate(), /*  �}������				*/

	constraint pk_tdunitmst primary key (tdunitcode)
);

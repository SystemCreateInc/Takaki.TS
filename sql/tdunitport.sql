/*
 *
 *	TD�\����|�[�g
 *
 *	$Id: tdunitport.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table tdunitport (
	tdunitportcode	smallint		not null,	/* �|�[�gCD					*/
	tdunitportcom	nvarchar(40)		not null,	/* �|�[�g�ԍ�				*/
	tdunitporttype	smallint		not null,	/* �|�[�g�敪				*/
	updt			datetime default getdate(),	/*  �X�V����				*/
	indt			datetime default getdate(), /*  �}������				*/

	constraint pk_tdunitport primary key (tdunitportcode)
);

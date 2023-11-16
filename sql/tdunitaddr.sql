/*
 *
 *	TD�\����A�h���X
 *
 *	$Id: tdunitaddr.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table tdunitaddr (
	tdunitaddrcode		nvarchar(10)		not null,	/* �A�h���XCD(�_���A�h���X)	*/
	tdunitcode			nvarchar(10)		not null,	/* �\����CD					*/
	tdunitareacode		integer,						/* �G���A�\����CD			*/
	tdunitzonecode		integer,						/* �]�[��CD					*/
	tdunitseq			integer,						/* �D�揇��					*/
	usageid				integer,						/* �p�rID					*/
	
	updt			datetime default getdate(),	/*  �X�V����				*/
	indt			datetime default getdate(), /*  �}������				*/

	constraint fk_tdaddr foreign key (tdunitcode) references tdunitmst(tdunitcode) on delete cascade,
	
	constraint pk_tdaddr primary key (tdunitcode)
);

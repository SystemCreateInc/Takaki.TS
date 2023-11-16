/*
 *
 *	�^�C�v�ꗗ(�R���{�\�����Ɏg�p)
 *
 *	$Id: typelists.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table typelists (
	typefield		nvarchar(100)	not null,	/* �^�C�v�t�B�[���h��		*/
	typecode		nvarchar(100)	not null,	/* �R�[�h					*/
	typename		nvarchar(100)	not null,	/* ����						*/
	typedispseq		integer			not null,	/* �\������					*/
	updt			datetime default getdate(),	/*  �X�V����				*/
	indt			datetime default getdate(), /*  �}������				*/

	constraint pk_typelists primary key (typefield,typecode)
);

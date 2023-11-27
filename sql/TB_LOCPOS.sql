/*
 *
 *	���P�[�V�����|�W�V�����ΏۊO�ݒ�
 *
 *	$Id: TB_LOCPOS.sql 4474 2004-05-06 04:02:04Z ohki $
 */

create table TB_LOCPOS (
	CD_BLOCK			nchar(2)				not null,	/* �u���b�N�R�[�h				*/
	
	tdunitaddrcode		nvarchar(10)			not null,	/* �A�h���XCD(�_���A�h���X)		*/
	
	ST_REMOVE			smallint 				not null,	/* ۹���ݑΏۊO
																0�F����۹���ݑΏ�
																1�F����۹���ݑΏۊO			*/
																
	createdAt 			datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 			datetime 				not null,	/*	�f�[�^�X�V��				*/

	constraint pk_TB_LOCPOS primary key (CD_BLOCK, tdunitaddrcode)
);

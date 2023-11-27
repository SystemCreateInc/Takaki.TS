/*
 *
 *	�ςݕt�����P��
 *
 *	$Id: TB_STOWAGE_BOX.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_STOWAGE_BOX (
	ID_STOWAGE			bigint 					not null,	/* id							*/
	ST_BOXTYPE			smallint				not null,	/* �^���e����
															0:���̑� 1:���� 2:���� 3:��	*/
	NU_OBOXCNT			integer					not null,	/* �\�蔠��						*/
	NU_RBOXCNT			integer					not null,	/* ���є���						*/

	CD_HENKOSHA			nchar(7)					not null,	/* �ύX�҃R�[�h					*/
	NM_HENKOSHA			nvarchar(40)			not null,	/* �ύX�Җ���					*/

	createdAt 			datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 			datetime 				not null,	/*	�f�[�^�X�V��				*/

	constraint fk_TB_STOWAGE_BOX foreign key (ID_STOWAGE) references TB_STOWAGE(ID_STOWAGE) on delete cascade,
	
	constraint pk_TB_STOWAGE_BOX primary key (ID_STOWAGE,ST_BOXTYPE)
);


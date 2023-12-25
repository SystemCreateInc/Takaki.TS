/*
 *
 *	�ςݕt���}�b�s���O����
 *
 *	$Id: TB_STOWAGE_MAPPING.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_STOWAGE_MAPPING (
	ID_STOWAGE			bigint				 	not null,	/* id							*/
	NM_SHUKKA_BATCH		nvarchar(40),						/* �o�׃o�b�`����				*/
	NM_KYOTEN			nvarchar(40),						/* ���_����						*/
	NM_TOKUISAKI		nvarchar(40),						/* ���Ӑ於						*/

	CD_BLOCK			nchar(2),							/* �u���b�N�R�[�h				*/
	CD_DIST_GROUP		nchar(5),							/* �d���O���[�v					*/
	NM_DIST_GROUP		nvarchar(40),						/* �d���O���[�v����				*/
	CD_BIN_SUM			smallint,							/* �z���֏W�v
																1:�֏W�v���� 2:�֏W�v���Ȃ�	*/

	CD_SUM_TOKUISAKI	nchar(6),							/* �W�񓾈Ӑ�R�[�h(�e)			*/
	NM_SUM_TOKUISAKI	nvarchar(40),						/* �W�񓾈Ӑ於					*/
	CD_SUM_COURSE		nchar(3),							/* �W�񓾈Ӑ�R�[�X				*/
	CD_SUM_ROUTE		integer,							/* �W�񓾈Ӑ�z����				*/

	tdunitaddrcode		nvarchar(10),						/* �A�h���XCD(�_���A�h���X)		*/
	NU_MAGICHI			integer,							/* �Ԍ���						*/

	createdAt 			datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 			datetime 				not null,	/*	�f�[�^�X�V��				*/

	constraint fk_TB_STOWAGE_MAPPING foreign key (ID_STOWAGE) references TB_STOWAGE(ID_STOWAGE) on delete cascade,
	
	constraint pk_TB_STOWAGE_MAPPING primary key (ID_STOWAGE)
);


/*
 *
 *	�o�׃f�[�^�}�b�s���O����
 *
 *	$Id: TB_DIST_MAPPING.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_MAPPING (
	ID_DIST				bigint 	not null,					/* id							*/
	
	NM_SHUKKA_BATCH		nvarchar(40),						/* �o�׃o�b�`����				*/
	NM_KYOTEN			nvarchar(40),						/* ���_����						*/
	NM_TOKUISAKI		nvarchar(40),						/* ���Ӑ於						*/
	NM_HIN_SEISHIKIMEI	nvarchar(60),						/* �i���i�������j				*/

	CD_SUM_TOKUISAKI	nchar(6),							/* �W�񓾈Ӑ�R�[�h(�e)			*/
	NM_SUM_TOKUISAKI	nvarchar(40),						/* �W�񓾈Ӑ於					*/

	CD_BLOCK			nchar(2),							/* �u���b�N�R�[�h				*/
	CD_DIST_GROUP		nchar(5),							/* �d���O���[�v					*/
	NM_DIST_GROUP		nvarchar(40),						/* �d���O���[�v����				*/
	CD_LARGE_GROUP		nchar(3),							/* ��d���O���[�v				*/
	NM_LARGE_GROUP		nvarchar(40),						/* ��d���O���[�v����			*/

	tdunitaddrcode		nvarchar(10),						/* �A�h���XCD(�_���A�h���X)		*/

	createdAt 			datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 			datetime 				not null,	/*	�f�[�^�X�V��				*/

	constraint fk_TB_DIST_MAPPING foreign key (ID_DIST) references TB_DIST(ID_DIST) on delete cascade,
	
	constraint pk_TB_DIST_MAPPING primary key (ID_DIST)
);


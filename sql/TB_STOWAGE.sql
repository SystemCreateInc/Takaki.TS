/*
 *
 *	�ςݕt��
 *
 *	$Id: TB_STOWAGE.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_STOWAGE (
	ID_STOWAGE			bigint identity(1,1) 	not null,	/* id							*/
	DT_DELIVERY			char(8)					not null,	/* �[�i��						*/
	
	CD_SHUKKA_BATCH		char(5)					not null,	/* �o�׃o�b�`					*/
	CD_KYOTEN			char(4)					not null,	/* �d�������_					*/
	CD_BIN				char(3)					not null,	/* �z����						*/
	CD_COURSE			char(3)					not null,	/* �R�[�X						*/
	CD_ROUTE			integer					not null,	/* �z����						*/
	CD_TOKUISAKI		char(6)					not null,	/* ���Ӑ�R�[�h					*/


	CD_BLOCK			char(2)					not null,	/* �u���b�N�R�[�h				*/
	CD_DIST_GROUP		char(5)					not null,	/* �d���O���[�v					*/

	CD_SUM_TOKUISAKI	char(6)					not null,	/* �W�񓾈Ӑ�R�[�h				*/

	tdunitaddrcode		nvarchar(10)			not null,	/* �A�h���XCD(�_���A�h���X)		*/

	FG_SSTATUS			smallint				not null,	/*	�ςݕt����Ə��
																	0:������
																	2:����					*/


	createdAt 			datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 			datetime 				not null,	/*	�f�[�^�X�V��				*/

	constraint pk_TB_STOWAGE primary key (ID_STOWAGE)
);


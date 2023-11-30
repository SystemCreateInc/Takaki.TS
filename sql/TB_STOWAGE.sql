/*
 *
 *	�ςݕt��
 *
 *	$Id: TB_STOWAGE.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_STOWAGE (
	ID_STOWAGE			bigint identity(1,1) 	not null,	/* id							*/
	DT_DELIVERY			nchar(8)				not null,	/* �[�i��						*/
	
	CD_SHUKKA_BATCH		nchar(5)				not null,	/* �o�׃o�b�`					*/
	CD_KYOTEN			nchar(4)				not null,	/* �d�������_					*/
	CD_HAISHO_BIN		nchar(3)				not null,	/* �z����						*/
	CD_COURSE			nchar(3)				not null,	/* �R�[�X						*/
	CD_ROUTE			integer					not null,	/* �z����						*/
	CD_TOKUISAKI		nchar(6)				not null,	/* ���Ӑ�R�[�h					*/

	CD_HENKOSHA			nchar(10)				not null,	/* �ύX�҃R�[�h					*/
	DT_TOROKU_NICHIJI	nchar(14)				not null,	/* �o�^����						*/
	DT_KOSHIN_NICHIJI	nchar(14)				not null,	/* �X�V����						*/
	
	FG_SSTATUS			smallint				not null,	/*	�ςݕt����Ə��
																	0:������
																	2:����					*/


	createdAt 			datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 			datetime 				not null,	/*	�f�[�^�X�V��				*/

	constraint pk_TB_STOWAGE primary key (ID_STOWAGE)
);


/*
 *
 *	�W�񓾈Ӑ���
 *
 *	$Id: TB_SUM_TOKUISAKI_CHILD.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_SUM_TOKUISAKI_CHILD (
	ID_SUM_TOKUISAKI		bigint 					not null,	/* id ���_�A�W�񓾈Ӑ�R�[�h�A�K�p�J�n���Ń��j�[�N	*/
	CD_TOKUISAKI_CHILD		char(6)					not null,	/* �W�񓾈Ӑ�R�[�h				*/

	createdAt 				datetime 				not null,	/*	�f�[�^�}����				*/
	updatedAt 				datetime 				not null,	/*	�f�[�^�X�V��				*/
	
	constraint fk_TB_SUM_TOKUISAKI_CHILD foreign key (ID_SUM_TOKUISAKI) references TB_SUM_TOKUISAKI(ID_SUM_TOKUISAKI) on delete cascade,
	
	constraint pk_TB_SUM_TOKUISAKI_CHILD primary key (ID_SUM_TOKUISAKI,	CD_TOKUISAKI_CHILD)
);


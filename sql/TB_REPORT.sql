/*
 *
 *	��Ɨ���
 *
 *	$Id: TB_REPORT.sql,v 1.1.1.1 2004/05/06 04:02:04 ohki Exp $
 */

create table TB_REPORT (
	ID_REPORT		integer identity not null,	/* id						*/
	CD_DIST_GROUP	char(5)			not null,	/* �d���O���[�v				*/
	NM_DIST_GROUP	nvarchar(40)	not null,	/* �d���O���[�v����			*/
	CD_BLOCK		char(2)			not null,	/* �u���b�N�R�[�h			*/
	DT_START		datetime		not null,	/* ��ƊJ�n����				*/
	DT_END			datetime		not null,	/* ��ƏI������				*/
	NM_IDLE			integer			not null,	/* �x�e���ԁi�b�j			*/
	CD_SYAIN		char(7)			not null,	/* �Ј��R�[�h				*/
	NM_SYAIN		nvarchar(40)	not null,	/* �Ј�����					*/
	DT_WORKSTART	datetime		not null,	/* ��ƊJ�n����				*/
	DT_WORKEND		datetime		not null,	/* ��ƏI������				*/
	NM_WORKTIME		integer			not null,	/* �ғ����ԁi�b�j			*/
	NM_ITEMCNT		integer			not null,	/* ���i��					*/
	NM_SHOPCNT		integer			not null,	/* �X�ܐ�					*/
	NM_DISTCNT		integer			not null,	/* �z����					*/
	NM_CHECKCNT		integer			not null,	/* ���i��					*/
	NM_CHECKTIME	integer			not null,	/* ���i���ԁi�b�j			*/
	
	createdAt 		datetime 		not null,	/*	�f�[�^�}����			*/
	
	constraint pk_TB_REPORT primary key (ID_REPORT)
);

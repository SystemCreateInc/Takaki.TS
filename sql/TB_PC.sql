/*
 *
 *	ＰＣブロック情報
 *
 *	$Id: TB_PC.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_PC (
	ID_PC					integer					not null,	/* PCID							*/
	CD_KYOTEN				char(4)					not null,	/* 拠点							*/
	CD_BLOCK				char(2)					not null,	/* ブロックコード				*/

	CD_HENKOSHA				char(7),							/* 	変更者コード				*/
	NM_SYAIN				char(7),							/* 	社員名称					*/
	
	createdAt 				datetime 				not null,	/*	データ挿入日				*/
	updatedAt 				datetime 				not null,	/*	データ更新日				*/

	constraint pk_TB_PC primary key (ID_PC)
);


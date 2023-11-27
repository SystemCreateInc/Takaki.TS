/*
 *
 *	ロケーションポジション対象外設定
 *
 *	$Id: TB_LOCPOS.sql 4474 2004-05-06 04:02:04Z ohki $
 */

create table TB_LOCPOS (
	CD_BLOCK			nchar(2)				not null,	/* ブロックコード				*/
	
	tdunitaddrcode		nvarchar(10)			not null,	/* アドレスCD(論理アドレス)		*/
	
	ST_REMOVE			smallint 				not null,	/* ﾛｹｰｼｮﾝ対象外
																0：自動ﾛｹｰｼｮﾝ対象
																1：自動ﾛｹｰｼｮﾝ対象外			*/
																
	createdAt 			datetime 				not null,	/*	データ挿入日				*/
	updatedAt 			datetime 				not null,	/*	データ更新日				*/

	constraint pk_TB_LOCPOS primary key (CD_BLOCK, tdunitaddrcode)
);

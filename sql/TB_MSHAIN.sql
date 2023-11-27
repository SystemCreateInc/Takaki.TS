/*
 *
 *	社員マスタ
 *
 *	$Id: TB_MSHAIN.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_MSHAIN (
	CD_SHAIN						nchar			(7)			not null,	/* 	社員コード						*/
	DT_TEKIYOKAISHI					nchar           (8)			not null,	/* 	適用開始日						*/
	DT_TEKIYOMUKO					nchar           (8)			not null,	/* 	適用無効日						*/
	DT_TOROKU_NICHIJI				nchar           (14)		not null,	/* 	登録日時						*/
	DT_KOSHIN_NICHIJI				nchar           (14)		not null,	/* 	更新日時						*/
	CD_HENKOSHA						nchar           (10)		not null,	/* 	変更者コード					*/
	NM_SHAIN						nvarchar        (40)		not null,	/* 	社員名称						*/
	NM_SHAIN_YOMI					nvarchar        (60)		not null,	/* 	氏名読み						*/
	NM_SHAIN_YOMI_KANA				nvarchar        (30)		not null,	/* 	氏名読み（カナ）				*/
	CD_BUMON						nchar           (6)			not null,	/* 	部門コード						*/
	FG_TAISHOKUSHA					nchar           (1)			not null,	/* 	退職者コード					*/
	ST_KOYO							nchar           (2)			not null,	/* 	雇用区分						*/
	IF_USER_MAIL_ADDRESS			nvarchar        (128)		not null,	/* 	ユーザーメールアドレス			*/
	CD_YAKUSHOKU					nchar           (2)			not null,	/* 	役職コード						*/
	CD_SOTO_YAKUSHOKU				nchar           (2)			not null,	/* 	相当役職						*/
	CD_SHOKUMU						nchar           (3)			not null,	/* 	職務コード						*/
	CD_SHOZOKU_KAISHA				nchar           (2)			not null,	/* 	所属会社コード					*/
	CD_SHOZOKU_KANPANI				nchar           (6)			not null,	/* 	所属カンパニーコード			*/
	NU_HAITA						numeric         (5)			not null,	/* 	排他カウンタ					*/
	DT_RENKEI						nchar           (14)		not null,	/* 	連携日時						*/
	
	createdAt datetime not null,		/*	データ挿入日				*/
	updatedAt datetime not null,		/*	データ更新日				*/

	constraint pk_TB_MSHAIN primary key (CD_SHAIN,DT_TEKIYOKAISHI)
);


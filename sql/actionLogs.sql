/*
 *	アクションログ
 */
create table actionLogs
(
	id 	bigint identity not null,		/* id */
	personid		char(7),			/* 担当者						*/
	personnm		nvarchar(40),		/* 担当者名称					*/
	terminal		nvarchar(255),		/* 実施PC						*/
	work 			nvarchar(100),		/* 作業行程						*/
	operation 		nvarchar(100),		/* 作業内容						*/
	status 			nvarchar(100),		/* 成功、失敗					*/
	description 	nvarchar(max),		/* 処理詳細	jsonで書き込み		*/

	createdAt datetime not null,		/*	データ挿入日				*/
	updatedAt datetime not null,		/*	データ更新日				*/

	constraint pk_actionLogs primary key (id)
);


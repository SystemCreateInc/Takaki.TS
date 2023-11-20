/*
 *	送受信ファイル
 */
create table interfaceFiles
(
	dataType smallint not null,			/* データ種別					*/
	name varchar(100) not null,			/* データ名称					*/
	sortOrder integer,					/* ソート順						*/
	direction smallint not null,		/* 方向 0:入力 or 1:出力		*/
	fileName varchar(1000) not null,	/* ファイル名、パターン			*/
	expdays integer,					/* 保持日数	取込用				*/
	comment varchar(255),				/* コメント						*/
	
	createdAt datetime not null,		/*	データ挿入日				*/
	updatedAt datetime not null,		/*	データ更新日				*/

	constraint pk_interface_files primary key (dataType)
);

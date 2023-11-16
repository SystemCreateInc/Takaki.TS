/*
 *	送受信ログ
 */
create table interface_logs
(
	id integer identity not null,
	/* id */
	work smallint not null,
	/* 方向 0:入力 or 1:出力				*/
	direction smallint not null,
	/* 方向 0:入力 or 1:出力				*/
	data_type smallint not null,
	/* データ種別				*/
	force smallint not null,
	/* 強制送信				*/
	row_count integer not null,
	/* 件数						*/
	file_size integer not null,
	/* ファイルサイズ						*/
	name nvarchar(100) not null,
	/* データ名称				*/
	status nvarchar(40) not null,
	/* ステータス				*/
	src_file nvarchar(255),
	/* 送信元					*/
	dst_file nvarchar(255),
	/* 送信先					*/
	comment nvarchar(1000),
	/* コメント					*/
	terminal nvarchar(255),
	/* 実施PC					*/
	file_date datetime,
	/* ファイル更新日			*/
	createdAt datetime,
	/*	データ挿入日			*/
	updatedAt datetime,
	/*	データ更新日			*/

	constraint pk_interface_logs primary key (id)
);


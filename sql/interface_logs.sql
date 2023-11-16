/*
 *	����M���O
 */
create table interface_logs
(
	id integer identity not null,
	/* id */
	work smallint not null,
	/* ���� 0:���� or 1:�o��				*/
	direction smallint not null,
	/* ���� 0:���� or 1:�o��				*/
	data_type smallint not null,
	/* �f�[�^���				*/
	force smallint not null,
	/* �������M				*/
	row_count integer not null,
	/* ����						*/
	file_size integer not null,
	/* �t�@�C���T�C�Y						*/
	name nvarchar(100) not null,
	/* �f�[�^����				*/
	status nvarchar(40) not null,
	/* �X�e�[�^�X				*/
	src_file nvarchar(255),
	/* ���M��					*/
	dst_file nvarchar(255),
	/* ���M��					*/
	comment nvarchar(1000),
	/* �R�����g					*/
	terminal nvarchar(255),
	/* ���{PC					*/
	file_date datetime,
	/* �t�@�C���X�V��			*/
	createdAt datetime,
	/*	�f�[�^�}����			*/
	updatedAt datetime,
	/*	�f�[�^�X�V��			*/

	constraint pk_interface_logs primary key (id)
);


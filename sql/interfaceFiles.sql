/*
 *	����M�t�@�C��
 */
create table interfaceFiles
(
	dataType smallint not null,			/* �f�[�^���					*/
	name varchar(100) not null,			/* �f�[�^����					*/
	sortOrder integer,					/* �\�[�g��						*/
	direction smallint not null,		/* ���� 0:���� or 1:�o��		*/
	fileName varchar(1000) not null,	/* �t�@�C�����A�p�^�[��			*/
	expdays integer,					/* �ێ�����	�捞�p				*/
	comment varchar(255),				/* �R�����g						*/
	
	createdAt datetime not null,		/*	�f�[�^�}����				*/
	updatedAt datetime not null,		/*	�f�[�^�X�V��				*/

	constraint pk_interface_files primary key (dataType)
);

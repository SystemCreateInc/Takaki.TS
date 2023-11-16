/*
 *	�A�N�V�������O
 */
create table actionLogs
(
	id 	bigint identity not null,		/* id */
	personid		char(7),			/* �S����						*/
	personnm		nvarchar(40),		/* �S���Җ���					*/
	terminal		nvarchar(255),		/* ���{PC						*/
	work 			nvarchar(100),		/* ��ƍs��						*/
	operation 		nvarchar(100),		/* ��Ɠ��e						*/
	status 			nvarchar(100),		/* �����A���s					*/
	description 	nvarchar(max),		/* �����ڍ�	json�ŏ�������		*/

	createdAt datetime not null,		/*	�f�[�^�}����				*/
	updatedAt datetime not null,		/*	�f�[�^�X�V��				*/

	constraint pk_actionLogs primary key (id)
);


delete from interface_logs;

-- ��M�t�@�C��
insert into interfaceFiles (dataType, name, sortOrder, direction, fileName, expdays, comment, createdAt, updatedAt) values(0, '���_�}�X�^', 0, 0, 'TB_MKYOTEN.csv', 0, '', getdate(), getdate());
insert into interfaceFiles (dataType, name, sortOrder, direction, fileName, expdays, comment, createdAt, updatedAt) values(1, '�Ј��}�X�^', 0, 0, 'TB_MSHAIN.csv', 0, '', getdate(), getdate());
insert into interfaceFiles (dataType, name, sortOrder, direction, fileName, expdays, comment, createdAt, updatedAt) values(2, '���Ӑ�}�X�^', 0, 0, 'TB_MTOKUISAKI.csv', 0, '', getdate(), getdate());
insert into interfaceFiles (dataType, name, sortOrder, direction, fileName, expdays, comment, createdAt, updatedAt) values(3, '�i�ڃ}�X�^', 0, 0, 'TB_MHIMMOKU.csv', 0, '', getdate(), getdate());
insert into interfaceFiles (dataType, name, sortOrder, direction, fileName, expdays, comment, createdAt, updatedAt) values(4, '�o�׃o�b�`�}�X�^', 0, 0, 'TB_MSHUKKA_BATCH.csv', 0, '', getdate(), getdate());
insert into interfaceFiles (dataType, name, sortOrder, direction, fileName, expdays, comment, createdAt, updatedAt) values(5, '�Œ薼�̃}�X�^', 0, 0, 'TB_MKOTEI_MEISHO.csv', 0, '', getdate(), getdate());

insert into interfaceFiles (dataType, name, sortOrder, direction, fileName, expdays, comment, createdAt, updatedAt) values(10, '�o�ח\��f�[�^', 0, 0, '�o�׃f�[�^*.csv', 100, '', getdate(), getdate());
insert into interfaceFiles (dataType, name, sortOrder, direction, fileName, expdays, comment, createdAt, updatedAt) values(11, '�����\��f�[�^', 0, 0, '�����\�萔�f�[�^*.csv', 100, '', getdate(), getdate());

-- ���уt�@�C��
insert into interfaceFiles (dataType, name, sortOrder, direction, fileName, expdays, comment, createdAt, updatedAt) values(100, '�o�׎��уf�[�^', 0, 1, 'C:\HULFT\snd\pick_result.txt', 0, '-f S2AOT001 -sync', getdate(), getdate());
insert into interfaceFiles (dataType, name, sortOrder, direction, fileName, expdays, comment, createdAt, updatedAt) values(101, '�������уf�[�^', 0, 1, 'C:\HULFT\snd\hako_result.txt', 0, '-f S2AOT002 -sync', getdate(), getdate());

-- �F
insert into typelists values('distcolor','1','��',1, getdate(), getdate());
insert into typelists values('distcolor','2','��',2, getdate(), getdate());
insert into typelists values('distcolor','3','��',3, getdate(), getdate());
insert into typelists values('distcolor','4','��',4, getdate(), getdate());
insert into typelists values('distcolor','5','��',5, getdate(), getdate());
insert into typelists values('distcolor','6','��',6, getdate(), getdate());

-- �p�r�敪
insert into typelists values('usageid','0','�d��',0, getdate(), getdate());
insert into typelists values('usageid','1','����BOX',1, getdate(), getdate());
insert into typelists values('usageid','2','����BOX',2, getdate(), getdate());
insert into typelists values('usageid','3','�z��',3, getdate(), getdate());
insert into typelists values('usageid','4','�ް�����',4, getdate(), getdate());
insert into typelists values('usageid','5','�V��',5, getdate(), getdate());
insert into typelists values('usageid','6','�I',6, getdate(), getdate());
insert into typelists values('usageid','7','�V�ݽ���BOX',7, getdate(), getdate());
insert into typelists values('usageid','8','�I����BOX',8, getdate(), getdate());

-- TD�|�[�g�敪
insert into typelists values('tdunitporttype','0','WIRELESS',0, getdate(), getdate());
insert into typelists values('tdunitporttype','1','WIRED',1, getdate(), getdate());
insert into typelists values('tdunitporttype','2','WIRELESS2',2, getdate(), getdate());
insert into typelists values('tdunitporttype','3','WIRED2',3, getdate(), getdate());

-- TD�|�[�g
insert into tdunitport values('1','COM1','1', getdate(), getdate());

-- TD�\����}�X�^�i�V�݁j
-- T0001 = �\���탆�j�[�N�L�[ 
-- 1 = TD�|�[�g
-- 1 = ����
-- 1 = ����
-- 5 = �F
insert into tdunitmst values('T0001',1,1, 1,5, getdate(), getdate());
insert into tdunitmst values('T0002',1,1, 2,5, getdate(), getdate());
insert into tdunitmst values('T0003',1,1, 3,5, getdate(), getdate());
insert into tdunitmst values('T0004',1,1, 4,5, getdate(), getdate());
insert into tdunitmst values('T0005',1,1, 5,5, getdate(), getdate());
insert into tdunitmst values('T0006',1,1, 6,5, getdate(), getdate());
insert into tdunitmst values('T0007',1,1, 7,5, getdate(), getdate());
insert into tdunitmst values('T0008',1,1, 8,5, getdate(), getdate());
insert into tdunitmst values('T0009',1,1, 9,5, getdate(), getdate());
insert into tdunitmst values('T0010',1,1,10,5, getdate(), getdate());
insert into tdunitmst values('T0011',1,1,11,5, getdate(), getdate());
insert into tdunitmst values('T0012',1,1,12,5, getdate(), getdate());
insert into tdunitmst values('T0013',1,1,13,5, getdate(), getdate());
insert into tdunitmst values('T0014',1,1,14,5, getdate(), getdate());
insert into tdunitmst values('T0015',1,1,15,5, getdate(), getdate());
insert into tdunitmst values('T0016',1,1,16,5, getdate(), getdate());
insert into tdunitmst values('T0017',1,1,17,5, getdate(), getdate());
insert into tdunitmst values('T0018',1,1,18,5, getdate(), getdate());
insert into tdunitmst values('T0019',1,1,19,5, getdate(), getdate());
insert into tdunitmst values('T0020',1,1,20,5, getdate(), getdate());

-- TD�\����(�V�݂�X�^�[�gBOX�j
-- T0001 = �\���탆�j�[�N�L�[
-- 1 = TD�|�[�g
-- 1 = ����
-- 1 = ����
-- 1 = �F
insert into tdunitmst values('TS0001',1,1,101,1, getdate(), getdate());
insert into tdunitmst values('TS0002',1,1,102,2, getdate(), getdate());
insert into tdunitmst values('TS0003',1,1,103,3, getdate(), getdate());
insert into tdunitmst values('TS0004',1,1,104,4, getdate(), getdate());
insert into tdunitmst values('TS0005',1,1,105,5, getdate(), getdate());
insert into tdunitmst values('TS0006',1,1,106,1, getdate(), getdate());
insert into tdunitmst values('TS0007',1,1,107,2, getdate(), getdate());
insert into tdunitmst values('TS0008',1,1,108,3, getdate(), getdate());
insert into tdunitmst values('TS0009',1,1,109,4, getdate(), getdate());
insert into tdunitmst values('TS0010',1,1,110,5, getdate(), getdate());

-- TD�\����}�X�^(�I)
insert into tdunitmst values('R0001',1,1, 1,5, getdate(), getdate());
insert into tdunitmst values('R0002',1,1, 2,5, getdate(), getdate());
insert into tdunitmst values('R0003',1,1, 3,5, getdate(), getdate());
insert into tdunitmst values('R0004',1,1, 4,5, getdate(), getdate());
insert into tdunitmst values('R0005',1,1, 5,5, getdate(), getdate());
insert into tdunitmst values('R0006',1,1, 6,5, getdate(), getdate());
insert into tdunitmst values('R0007',1,1, 7,5, getdate(), getdate());
insert into tdunitmst values('R0008',1,1, 8,5, getdate(), getdate());
insert into tdunitmst values('R0009',1,1, 9,5, getdate(), getdate());
insert into tdunitmst values('R0010',1,1,10,5, getdate(), getdate());
insert into tdunitmst values('R0011',1,1,11,5, getdate(), getdate());
insert into tdunitmst values('R0012',1,1,12,5, getdate(), getdate());
insert into tdunitmst values('R0013',1,1,13,5, getdate(), getdate());
insert into tdunitmst values('R0014',1,1,14,5, getdate(), getdate());
insert into tdunitmst values('R0015',1,1,15,5, getdate(), getdate());
insert into tdunitmst values('R0016',1,1,16,5, getdate(), getdate());
insert into tdunitmst values('R0017',1,1,17,5, getdate(), getdate());
insert into tdunitmst values('R0018',1,1,18,5, getdate(), getdate());
insert into tdunitmst values('R0019',1,1,19,5, getdate(), getdate());
insert into tdunitmst values('R0020',1,1,20,5, getdate(), getdate());

-- TD�\����(�I�X�^�[�gBOX�j
insert into tdunitmst values('RS0001',1,1,101,1, getdate(), getdate());
insert into tdunitmst values('RS0002',1,1,102,2, getdate(), getdate());
insert into tdunitmst values('RS0003',1,1,103,3, getdate(), getdate());
insert into tdunitmst values('RS0004',1,1,104,4, getdate(), getdate());
insert into tdunitmst values('RS0005',1,1,105,5, getdate(), getdate());
insert into tdunitmst values('RS0006',1,1,106,1, getdate(), getdate());
insert into tdunitmst values('RS0007',1,1,107,2, getdate(), getdate());
insert into tdunitmst values('RS0008',1,1,108,3, getdate(), getdate());
insert into tdunitmst values('RS0009',1,1,109,4, getdate(), getdate());
insert into tdunitmst values('RS0010',1,1,110,5, getdate(), getdate());

-- TD�\����(�V�݁j
-- 0001 = �_���A�h���X
-- T0001 = �\���탆�j�[�N�L�[
-- 0 = �G���A�\���� 0=���g�p
-- 1 = �ʘH
-- 1 = �ʘH���_�����i�ǋ삯�Ŏg�p�j
-- 5 = �V��,�I�敪 5=�V�݂� 6=�I
insert into tdunitaddr values('0001','T0001',0,1, 1,5,getdate(), getdate());
insert into tdunitaddr values('0002','T0002',0,1, 2,5,getdate(), getdate());
insert into tdunitaddr values('0003','T0003',0,1, 3,5,getdate(), getdate());
insert into tdunitaddr values('0004','T0004',0,1, 4,5,getdate(), getdate());
insert into tdunitaddr values('0005','T0005',0,1, 5,5,getdate(), getdate());
insert into tdunitaddr values('0006','T0006',0,1, 1,5,getdate(), getdate());
insert into tdunitaddr values('0007','T0007',0,1, 2,5,getdate(), getdate());
insert into tdunitaddr values('0008','T0008',0,1, 3,5,getdate(), getdate());
insert into tdunitaddr values('0009','T0009',0,1, 4,5,getdate(), getdate());
insert into tdunitaddr values('0010','T0010',0,1, 5,5,getdate(), getdate());
insert into tdunitaddr values('0011','T0011',0,2,10,5,getdate(), getdate());
insert into tdunitaddr values('0012','T0012',0,2, 9,5,getdate(), getdate());
insert into tdunitaddr values('0013','T0013',0,2, 8,5,getdate(), getdate());
insert into tdunitaddr values('0014','T0014',0,2, 7,5,getdate(), getdate());
insert into tdunitaddr values('0015','T0015',0,2, 6,5,getdate(), getdate());
insert into tdunitaddr values('0016','T0016',0,2,10,5,getdate(), getdate());
insert into tdunitaddr values('0017','T0017',0,2, 9,5,getdate(), getdate());
insert into tdunitaddr values('0018','T0018',0,2, 8,5,getdate(), getdate());
insert into tdunitaddr values('0019','T0019',0,2, 7,5,getdate(), getdate());
insert into tdunitaddr values('0020','T0020',0,2, 6,5,getdate(), getdate());

-- TD�\����(�V�݂�X�^�[�gBOX�j
-- S001 = �_���A�h���X
-- TS0001 = �\���탆�j�[�N�L�[
-- 0 = �G���A�\���� 0=���g�p
-- 1 = �ʘH
-- 1 = �ʘH
-- 7 = �V��,�I�敪 7=�V�݂� 8=�I
insert into tdunitaddr values('S001','TS0001',0,1,1,7,getdate(), getdate());
insert into tdunitaddr values('S002','TS0002',0,1,1,7,getdate(), getdate());
insert into tdunitaddr values('S003','TS0003',0,1,1,7,getdate(), getdate());
insert into tdunitaddr values('S004','TS0004',0,1,1,7,getdate(), getdate());
insert into tdunitaddr values('S005','TS0005',0,1,1,7,getdate(), getdate());
insert into tdunitaddr values('S006','TS0006',0,2,2,7,getdate(), getdate());
insert into tdunitaddr values('S007','TS0007',0,2,2,7,getdate(), getdate());
insert into tdunitaddr values('S008','TS0008',0,2,2,7,getdate(), getdate());
insert into tdunitaddr values('S009','TS0009',0,2,2,7,getdate(), getdate());
insert into tdunitaddr values('S010','TS0010',0,2,2,7,getdate(), getdate());

-- TD�\����(�I)
insert into tdunitaddr values('0001','R0001',0,1, 1,6,getdate(), getdate());
insert into tdunitaddr values('0002','R0002',0,1, 2,6,getdate(), getdate());
insert into tdunitaddr values('0003','R0003',0,1, 3,6,getdate(), getdate());
insert into tdunitaddr values('0004','R0004',0,1, 4,6,getdate(), getdate());
insert into tdunitaddr values('0005','R0005',0,1, 5,6,getdate(), getdate());
insert into tdunitaddr values('0006','R0006',0,1, 1,6,getdate(), getdate());
insert into tdunitaddr values('0007','R0007',0,1, 2,6,getdate(), getdate());
insert into tdunitaddr values('0008','R0008',0,1, 3,6,getdate(), getdate());
insert into tdunitaddr values('0009','R0009',0,1, 4,6,getdate(), getdate());
insert into tdunitaddr values('0010','R0010',0,1, 5,6,getdate(), getdate());
insert into tdunitaddr values('0011','R0011',0,2,10,6,getdate(), getdate());
insert into tdunitaddr values('0012','R0012',0,2, 9,6,getdate(), getdate());
insert into tdunitaddr values('0013','R0013',0,2, 8,6,getdate(), getdate());
insert into tdunitaddr values('0014','R0014',0,2, 7,6,getdate(), getdate());
insert into tdunitaddr values('0015','R0015',0,2, 6,6,getdate(), getdate());
insert into tdunitaddr values('0016','R0016',0,2,10,6,getdate(), getdate());
insert into tdunitaddr values('0017','R0017',0,2, 9,6,getdate(), getdate());
insert into tdunitaddr values('0018','R0018',0,2, 8,6,getdate(), getdate());
insert into tdunitaddr values('0019','R0019',0,2, 7,6,getdate(), getdate());
insert into tdunitaddr values('0020','R0020',0,2, 6,6,getdate(), getdate());


-- TD�\����(�I�X�^�[�gBOX�j
insert into tdunitaddr values('S001','RS0001',0,1,1,8,getdate(), getdate());
insert into tdunitaddr values('S002','RS0002',0,1,1,8,getdate(), getdate());
insert into tdunitaddr values('S003','RS0003',0,1,1,8,getdate(), getdate());
insert into tdunitaddr values('S004','RS0004',0,1,1,8,getdate(), getdate());
insert into tdunitaddr values('S005','RS0005',0,1,1,8,getdate(), getdate());
insert into tdunitaddr values('S006','RS0006',0,2,2,8,getdate(), getdate());
insert into tdunitaddr values('S007','RS0007',0,2,2,8,getdate(), getdate());
insert into tdunitaddr values('S008','RS0008',0,2,2,8,getdate(), getdate());
insert into tdunitaddr values('S009','RS0009',0,2,2,8,getdate(), getdate());
insert into tdunitaddr values('S010','RS0010',0,2,2,8,getdate(), getdate());



-- �u���b�N
insert into TB_BLOCK values('4201','01',5,128,13,'20230101','29991231','','',getdate(),getdate());
insert into TB_BLOCK values('4201','02',5,128,13,'20230101','29991231','','',getdate(),getdate());
insert into TB_BLOCK values('4201','03',5,128,13,'20230101','29991231','','',getdate(),getdate());
insert into TB_BLOCK values('4201','04',5,128,13,'20230101','29991231','','',getdate(),getdate());
insert into TB_BLOCK values('4201','05',6,256, 4,'20230101','29991231','','',getdate(),getdate());
insert into TB_BLOCK values('4201','06',6,256, 4,'20230101','29991231','','',getdate(),getdate());
insert into TB_BLOCK values('4201','11',5,128,13,'20230101','29991231','','',getdate(),getdate());
insert into TB_BLOCK values('4201','15',6,256, 4,'20230101','29991231','','',getdate(),getdate());

-- PC���
insert into TB_PC values(0,'00',NULL,NULL,getdate(),getdate());
insert into TB_PC values(1,'01',NULL,NULL,getdate(),getdate());
insert into TB_PC values(2,'02',NULL,NULL,getdate(),getdate());
insert into TB_PC values(3,'03',NULL,NULL,getdate(),getdate());
insert into TB_PC values(4,'04',NULL,NULL,getdate(),getdate());
insert into TB_PC values(5,'05',NULL,NULL,getdate(),getdate());
insert into TB_PC values(6,'06',NULL,NULL,getdate(),getdate());



-- settings
--insert into settings(value,data,id,comment)values('importgroup','0','','�ҏW�O���[�v');
--insert into settings(value,data,id,comment)values('autoimporttime','60','','������M�Ԋu');
insert into settings values('PrintChunkSize','100','','����������w��');

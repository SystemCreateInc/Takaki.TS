delete from interface_logs;

-- 色
insert into typelists values('distcolor','1','赤',1, getdate(), getdate());
insert into typelists values('distcolor','2','黄',2, getdate(), getdate());
insert into typelists values('distcolor','3','緑',3, getdate(), getdate());
insert into typelists values('distcolor','4','白',4, getdate(), getdate());
insert into typelists values('distcolor','5','青',5, getdate(), getdate());
insert into typelists values('distcolor','6','無',6, getdate(), getdate());

-- 用途区分
insert into typelists values('usageid','0','仕分',0, getdate(), getdate());
insert into typelists values('usageid','1','ｽﾀｰﾄBOX',1, getdate(), getdate());
insert into typelists values('usageid','2','ｴﾝﾄﾞBOX',2, getdate(), getdate());
insert into typelists values('usageid','3','配順',3, getdate(), getdate());
insert into typelists values('usageid','4','ｿﾞｰﾝﾎﾞﾀﾝ',4, getdate(), getdate());
insert into typelists values('usageid','5','天吊',5, getdate(), getdate());
insert into typelists values('usageid','6','棚',6, getdate(), getdate());
insert into typelists values('usageid','7','天吊ｽﾀｰﾄBOX',1, getdate(), getdate());

-- TDポート区分
insert into typelists values('tdunitporttype','0','WIRELESS',0, getdate(), getdate());
insert into typelists values('tdunitporttype','1','WIRED',1, getdate(), getdate());
insert into typelists values('tdunitporttype','2','WIRELESS2',2, getdate(), getdate());
insert into typelists values('tdunitporttype','3','WIRED2',3, getdate(), getdate());

-- TDポート
insert into tdunitport values('1','COM1','1', getdate(), getdate());

-- TD表示器マスタ（天吊）
insert into tdunitmst values('10001',1,1, 1,5, getdate(), getdate());
insert into tdunitmst values('10002',1,1, 2,5, getdate(), getdate());
insert into tdunitmst values('10003',1,1, 3,5, getdate(), getdate());
insert into tdunitmst values('10004',1,1, 4,5, getdate(), getdate());
insert into tdunitmst values('10005',1,1, 5,5, getdate(), getdate());
insert into tdunitmst values('10006',1,1, 6,5, getdate(), getdate());
insert into tdunitmst values('10007',1,1, 7,5, getdate(), getdate());
insert into tdunitmst values('10008',1,1, 8,5, getdate(), getdate());
insert into tdunitmst values('10009',1,1, 9,5, getdate(), getdate());
insert into tdunitmst values('10010',1,1,10,5, getdate(), getdate());
insert into tdunitmst values('10011',1,1,11,5, getdate(), getdate());
insert into tdunitmst values('10012',1,1,12,5, getdate(), getdate());
insert into tdunitmst values('10013',1,1,13,5, getdate(), getdate());
insert into tdunitmst values('10014',1,1,14,5, getdate(), getdate());
insert into tdunitmst values('10015',1,1,15,5, getdate(), getdate());
insert into tdunitmst values('10016',1,1,16,5, getdate(), getdate());
insert into tdunitmst values('10017',1,1,17,5, getdate(), getdate());
insert into tdunitmst values('10018',1,1,18,5, getdate(), getdate());
insert into tdunitmst values('10019',1,1,19,5, getdate(), getdate());
insert into tdunitmst values('10020',1,1,20,5, getdate(), getdate());

-- TD表示器(天吊りスタートBOX）
insert into tdunitmst values('TS0001',1,1,101,7, getdate(), getdate());
insert into tdunitmst values('TS0002',1,1,102,7, getdate(), getdate());
insert into tdunitmst values('TS0003',1,1,103,7, getdate(), getdate());
insert into tdunitmst values('TS0004',1,1,104,7, getdate(), getdate());
insert into tdunitmst values('TS0005',1,1,105,7, getdate(), getdate());
insert into tdunitmst values('TS0006',1,1,106,7, getdate(), getdate());
insert into tdunitmst values('TS0007',1,1,107,7, getdate(), getdate());
insert into tdunitmst values('TS0008',1,1,108,7, getdate(), getdate());
insert into tdunitmst values('TS0009',1,1,109,7, getdate(), getdate());
insert into tdunitmst values('TS0010',1,1,110,7, getdate(), getdate());

-- TD表示器マスタ(棚)
insert into tdunitmst values('20001',1,1, 1,5, getdate(), getdate());
insert into tdunitmst values('20002',1,1, 2,5, getdate(), getdate());
insert into tdunitmst values('20003',1,1, 3,5, getdate(), getdate());
insert into tdunitmst values('20004',1,1, 4,5, getdate(), getdate());
insert into tdunitmst values('20005',1,1, 5,5, getdate(), getdate());
insert into tdunitmst values('20006',1,1, 6,5, getdate(), getdate());
insert into tdunitmst values('20007',1,1, 7,5, getdate(), getdate());
insert into tdunitmst values('20008',1,1, 8,5, getdate(), getdate());
insert into tdunitmst values('20009',1,1, 9,5, getdate(), getdate());
insert into tdunitmst values('20010',1,1,10,5, getdate(), getdate());
insert into tdunitmst values('20011',1,1,11,5, getdate(), getdate());
insert into tdunitmst values('20012',1,1,12,5, getdate(), getdate());
insert into tdunitmst values('20013',1,1,13,5, getdate(), getdate());
insert into tdunitmst values('20014',1,1,14,5, getdate(), getdate());
insert into tdunitmst values('20015',1,1,15,5, getdate(), getdate());
insert into tdunitmst values('20016',1,1,16,5, getdate(), getdate());
insert into tdunitmst values('20017',1,1,17,5, getdate(), getdate());
insert into tdunitmst values('20018',1,1,18,5, getdate(), getdate());
insert into tdunitmst values('20019',1,1,19,5, getdate(), getdate());
insert into tdunitmst values('20020',1,1,20,5, getdate(), getdate());

-- TD表示器(棚スタートBOX）
insert into tdunitmst values('RS0001',1,1,101,1, getdate(), getdate());
insert into tdunitmst values('RS0002',1,1,102,1, getdate(), getdate());
insert into tdunitmst values('RS0003',1,1,103,1, getdate(), getdate());
insert into tdunitmst values('RS0004',1,1,104,1, getdate(), getdate());
insert into tdunitmst values('RS0005',1,1,105,1, getdate(), getdate());
insert into tdunitmst values('RS0006',1,1,106,1, getdate(), getdate());
insert into tdunitmst values('RS0007',1,1,107,1, getdate(), getdate());
insert into tdunitmst values('RS0008',1,1,108,1, getdate(), getdate());
insert into tdunitmst values('RS0009',1,1,109,1, getdate(), getdate());
insert into tdunitmst values('RS0010',1,1,110,1, getdate(), getdate());

-- TD表示器(天吊）
insert into tdunitaddr values('0001','10001',0,1, 1,5,getdate(), getdate());
insert into tdunitaddr values('0002','10002',0,1, 2,5,getdate(), getdate());
insert into tdunitaddr values('0003','10003',0,1, 3,5,getdate(), getdate());
insert into tdunitaddr values('0004','10004',0,1, 4,5,getdate(), getdate());
insert into tdunitaddr values('0005','10005',0,1, 5,5,getdate(), getdate());
insert into tdunitaddr values('0006','10006',0,1, 1,5,getdate(), getdate());
insert into tdunitaddr values('0007','10007',0,1, 2,5,getdate(), getdate());
insert into tdunitaddr values('0008','10008',0,1, 3,5,getdate(), getdate());
insert into tdunitaddr values('0009','10009',0,1, 4,5,getdate(), getdate());
insert into tdunitaddr values('0010','10010',0,1, 5,5,getdate(), getdate());
insert into tdunitaddr values('0011','10011',0,2,10,5,getdate(), getdate());
insert into tdunitaddr values('0012','10012',0,2, 9,5,getdate(), getdate());
insert into tdunitaddr values('0013','10013',0,2, 8,5,getdate(), getdate());
insert into tdunitaddr values('0014','10014',0,2, 7,5,getdate(), getdate());
insert into tdunitaddr values('0015','10015',0,2, 6,5,getdate(), getdate());
insert into tdunitaddr values('0016','10016',0,2,10,5,getdate(), getdate());
insert into tdunitaddr values('0017','10017',0,2, 9,5,getdate(), getdate());
insert into tdunitaddr values('0018','10018',0,2, 8,5,getdate(), getdate());
insert into tdunitaddr values('0019','10019',0,2, 7,5,getdate(), getdate());
insert into tdunitaddr values('0020','10020',0,2, 6,5,getdate(), getdate());

-- TD表示器(天吊りスタートBOX）
insert into tdunitaddr values('S001','TS0001',0,1,1,1,getdate(), getdate());
insert into tdunitaddr values('S002','TS0002',0,1,1,1,getdate(), getdate());
insert into tdunitaddr values('S003','TS0003',0,1,1,1,getdate(), getdate());
insert into tdunitaddr values('S004','TS0004',0,1,1,1,getdate(), getdate());
insert into tdunitaddr values('S005','TS0005',0,1,1,1,getdate(), getdate());
insert into tdunitaddr values('S006','TS0006',0,2,2,1,getdate(), getdate());
insert into tdunitaddr values('S007','TS0007',0,2,2,1,getdate(), getdate());
insert into tdunitaddr values('S008','TS0008',0,2,2,1,getdate(), getdate());
insert into tdunitaddr values('S009','TS0009',0,2,2,1,getdate(), getdate());
insert into tdunitaddr values('S010','TS0010',0,2,2,1,getdate(), getdate());

-- TD表示器(棚)
insert into tdunitaddr values('0001','20001',0,1, 1,6,getdate(), getdate());
insert into tdunitaddr values('0002','20002',0,1, 2,6,getdate(), getdate());
insert into tdunitaddr values('0003','20003',0,1, 3,6,getdate(), getdate());
insert into tdunitaddr values('0004','20004',0,1, 4,6,getdate(), getdate());
insert into tdunitaddr values('0005','20005',0,1, 5,6,getdate(), getdate());
insert into tdunitaddr values('0006','20006',0,1, 1,6,getdate(), getdate());
insert into tdunitaddr values('0007','20007',0,1, 2,6,getdate(), getdate());
insert into tdunitaddr values('0008','20008',0,1, 3,6,getdate(), getdate());
insert into tdunitaddr values('0009','20009',0,1, 4,6,getdate(), getdate());
insert into tdunitaddr values('0010','20010',0,1, 5,6,getdate(), getdate());
insert into tdunitaddr values('0001','20011',0,2,10,6,getdate(), getdate());
insert into tdunitaddr values('0002','20012',0,2, 9,6,getdate(), getdate());
insert into tdunitaddr values('0003','20013',0,2, 8,6,getdate(), getdate());
insert into tdunitaddr values('0004','20014',0,2, 7,6,getdate(), getdate());
insert into tdunitaddr values('0005','20015',0,2, 6,6,getdate(), getdate());
insert into tdunitaddr values('0006','20016',0,2,10,6,getdate(), getdate());
insert into tdunitaddr values('0007','20017',0,2, 9,6,getdate(), getdate());
insert into tdunitaddr values('0008','20018',0,2, 8,6,getdate(), getdate());
insert into tdunitaddr values('0009','20019',0,2, 7,6,getdate(), getdate());
insert into tdunitaddr values('0010','20020',0,2, 6,6,getdate(), getdate());


-- TD表示器(棚スタートBOX）
insert into tdunitaddr values('S001','RS0001',0,1,1,1,getdate(), getdate());
insert into tdunitaddr values('S002','RS0002',0,1,1,1,getdate(), getdate());
insert into tdunitaddr values('S003','RS0003',0,1,1,1,getdate(), getdate());
insert into tdunitaddr values('S004','RS0004',0,1,1,1,getdate(), getdate());
insert into tdunitaddr values('S005','RS0005',0,1,1,1,getdate(), getdate());
insert into tdunitaddr values('S006','RS0006',0,2,2,1,getdate(), getdate());
insert into tdunitaddr values('S007','RS0007',0,2,2,1,getdate(), getdate());
insert into tdunitaddr values('S008','RS0008',0,2,2,1,getdate(), getdate());
insert into tdunitaddr values('S009','RS0009',0,2,2,1,getdate(), getdate());
insert into tdunitaddr values('S010','RS0010',0,2,2,1,getdate(), getdate());



-- ブロック
insert into TB_BLOCK values('4201','01',5,128,13,'20230101','29991231','',getdate(),getdate());
insert into TB_BLOCK values('4201','02',5,128,13,'20230101','29991231','',getdate(),getdate());
insert into TB_BLOCK values('4201','03',5,128,13,'20230101','29991231','',getdate(),getdate());
insert into TB_BLOCK values('4201','04',5,128,13,'20230101','29991231','',getdate(),getdate());
insert into TB_BLOCK values('4201','05',6,256, 4,'20230101','29991231','',getdate(),getdate());
insert into TB_BLOCK values('4201','06',6,256, 4,'20230101','29991231','',getdate(),getdate());
insert into TB_BLOCK values('4201','11',5,128,13,'20230101','29991231','',getdate(),getdate());
insert into TB_BLOCK values('4201','15',6,256, 4,'20230101','29991231','',getdate(),getdate());

-- PC情報
insert into TB_PC values(0,'4201','00',NULL,getdate(),getdate());
insert into TB_PC values(1,'4201','01',NULL,getdate(),getdate());
insert into TB_PC values(2,'4201','02',NULL,getdate(),getdate());
insert into TB_PC values(3,'4201','03',NULL,getdate(),getdate());
insert into TB_PC values(4,'4201','04',NULL,getdate(),getdate());
insert into TB_PC values(5,'4201','05',NULL,getdate(),getdate());
insert into TB_PC values(6,'4201','06',NULL,getdate(),getdate());



-- settings
--insert into settings(value,data,id,comment)values('importgroup','0','','編集グループ');
--insert into settings(value,data,id,comment)values('autoimporttime','60','','自動受信間隔');


rem target server

set SERVER=(local)\MSSQL2022ex
rem Project root directory
set ROOT=c:\Projects\Takaki.TS

rem Database name
set DB=Takaki_TS

rem Recovery plan
set RCVP=simple

rem Authentication
rem	set AUTH=-E
set AUTH=-Usa -Psc

rem Creating tables
set TABLES=settings.sql interface_logs.sql interfaceFiles.sql
set TABLES=%TABLES% TB_MHIMMOKU.sql TB_MTOKUISAKI.sql TB_MSHUKKA_BATCH.sql TB_MKYOTEN.sql TB_MKOTEI_MEISHO.sql TB_MSHAIN.sql
set TABLES=%TABLES% TB_STOWAGE.sql TB_STOWAGE_MAPPING.sql TB_LOCPOS.sql TB_BLOCK.sql TB_DIST.sql TB_DIST_MAPPING.sql
set TABLES=%TABLES% TB_DIST_GROUP.sql TB_DIST_GROUP_LARGE_GROUP.sql TB_DIST_GROUP_SHUKKA_BATCH.sql TB_DIST_GROUP_COURSE.sql
set TABLES=%TABLES% TB_DIST_BLOCK.sql TB_DIST_BLOCK_SEQ.sql TB_LARGE_GROUP.sql TB_SUB_TOKUISAKI.sql TB_SUB_TOKUISAKI_CHILD.sql
set TABLES=%TABLES% TB_REPORT.sql TB_PC.sql TB_DIST_GROUP_PROGRESS.sql TB_LARGE_LOCK.sql
set TABLES=%TABLES% tdunitport.sql tdunitmst.sql tdunitareamst.sql tdunitaddr.sql tdunitarea.sql typelists.sql


set DBFILE=%ROOT%\db\%DB%.mdf
set DBNAME=%DB%_dat


call dbdef.bat

call dropdb.bat

osql -S %SERVER% %AUTH% -Q "create database %DB% on (name=%DBNAME%, filename='%DBFILE%')"
osql -S %SERVER% %AUTH% -Q "alter database %DB% set recovery %RCVP%"

for %%i in (%TABLES%) do osql -S %SERVER% %AUTH% -d %DB% -i %%i

call init.bat
pause
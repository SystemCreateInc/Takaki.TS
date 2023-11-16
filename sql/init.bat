echo off

call dbdef.bat

osql -S %SERVER% -d %DB% %AUTH% -i init.sql

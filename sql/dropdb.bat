call dbdef.bat

osql -S %SERVER% %AUTH% -Q "drop database %DB%"

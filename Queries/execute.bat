@ECHO OFF
sqlcmd -S .\SQLEXPRESS -o Log.log -i create.sql
CD Procedures

SETLOCAL enableDelayedExpansion
ECHO. > "..\Execucao.log"

for %%G in (*.sql) do (
    ECHO -------------------------------------------------------- >> "..\Execucao.log"
    ECHO !DATE! !TIME! Executando o script "%%G"... >> "..\Execucao.log"
    ECHO -------------------------------------------------------- >> "..\Execucao.log"
    ECHO. >> "..\Execucao.log"
    
    sqlcmd -S .\SQLEXPRESS -o ..\Log.log -i "%%G" >> "..\Execucao.log"
    
    ECHO. >> "..\Execucao.log"
    ECHO Fim da execucao: !DATE! !TIME! >> "..\Execucao.log"
    ECHO -------------------------------------------------------- >> "..\Execucao.log"
    ECHO. >> "..\Execucao.log"
    ECHO. >> "..\Execucao.log"
)
PAUSE
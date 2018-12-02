@echo off
set EXITCODE=0

echo GPLex compilation
tools\gplex.exe grammars\SLangScanner.lex
if ERRORLEVEL 1 goto Failed

echo GPPG compilation
tools\gppg.exe /gplex /conflicts grammars\SLangParser.y
if ERRORLEVEL 1 goto Failed

goto Success
:Failed
set EXITCODE=1
:Success
EXIT /B %EXITCODE%

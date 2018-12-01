@echo off
set EXITCODE=0

echo GPLex compilation
tools\gplex.exe src\SLangScanner.lex
if ERRORLEVEL 1 goto Failed

echo GPPG compilation
tools\gppg.exe /gplex /conflicts src\SLangParser.y
if ERRORLEVEL 1 goto Failed

echo C# compilation
csc src\Program.cs src\SLangScanner.cs src\SLangParser.cs src\SLangLookaheadScanner.cs src\Testing.cs src\LanguageElements.cs
if ERRORLEVEL 1 goto Failed

goto Success
:Failed
set EXITCODE=1
:Success
EXIT /B %EXITCODE%

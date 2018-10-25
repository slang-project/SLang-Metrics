@echo off
set EXITCODE=0

echo GPLex compilation
tools\gplex.exe /stack /out:SLangScanner.cs SLangScanner.lex
if ERRORLEVEL 1 goto Failed

echo GPPG compilation
tools\gppg.exe /gplex /verbose SLangParser.y > SLangParser.cs
if ERRORLEVEL 1 goto Failed

echo C# compilation
csc Program.cs SLangScanner.cs SLangParser.cs
if ERRORLEVEL 1 goto Failed

goto Success
:Failed
set EXITCODE=1
:Success
EXIT /B %EXITCODE%

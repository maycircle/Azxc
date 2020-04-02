set NET="v4, C:\Windows\Microsoft.NET\Framework\v4.0.30319"
set Output=merged

set ILMerge=%1packages\ILMerge.3.0.29\tools\net452\ILMerge.exe
echo ILMerge: %ILMerge%

set TargetPath=%2
echo TargetPath: %TargetPath%
set TargetFile=%~nx2
echo TargetFile: %TargetFile%
set TargetName=%~n2
echo TargetName: %TargetName%
set TargetDir=%~dp2
echo TargetDir: %TargetDir%
set ModPath=%3
echo ModPath: %ModPath%

set Outdir=%TargetDir%%Output%
echo Outdir: %Outdir%
set Result=%Outdir%\%TargetFile%
echo Result: %Result%

IF EXIST "%Outdir%" rmdir /S /Q "%Outdir%"
md "%Outdir%"

"%ILMerge%" /wildcards /targetplatform:%NET% /out:%Result% %TargetPath% "%TargetDir%0Harmony.dll" /allowDup

set Merged="%TargetDir%merged"
ECHO "%Merged%"
COPY "%Merged%\%TargetFile%" "%ModPath%\%TargetFile%"
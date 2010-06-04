
if "%WindowsSdkDir%" neq "" goto build
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\VC\vcvarsall.bat" goto initialize2k8on64
if exist "%ProgramFiles%\Microsoft Visual Studio 9.0\VC\vcvarsall.bat" goto initialize2k8
echo "Unable to detect suitable environment. Build may not succeed."
goto build


:initialize2k8
call "%ProgramFiles%\Microsoft Visual Studio 9.0\VC\vcvarsall.bat" x86
goto build

:initialize2k8on64
call "%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\VC\vcvarsall.bat" x86
goto build


:build
msbuild /t:Build AzurePackage.proj
pause
goto end


:end

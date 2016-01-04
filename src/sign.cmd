@echo off
::Adds AuthentiCode signatures to all binaries. Assumes "build.cmd Release" has already been executed.
if not "%1" == "" set signing_cert_path=%*

rem Determine VS version
if defined VS140COMNTOOLS (
  ::Visual Studio 2015
  call "%VS140COMNTOOLS%vsvars32.bat"
  goto vs_ok
)
if defined VS120COMNTOOLS (
  ::Visual Studio 2013
  call "%VS120COMNTOOLS%vsvars32.bat"
  goto vs_ok
)
if defined VS110COMNTOOLS (
  ::Visual Studio 2012
  call "%VS110COMNTOOLS%vsvars32.bat"
  goto vs_ok
)
if defined VS100COMNTOOLS (
  ::Visual Studio 2010
  call "%VS100COMNTOOLS%vsvars32.bat"
  goto vs_ok
)
echo ERROR: No Visual Studio installation found. >&2
exit /b 1
:vs_ok



echo Signing binaries with "%signing_cert_path%"...
FOR %%A IN ("%~dp0..\build\Release\NanoByte.Common*.dll") DO signtool sign /t http://timestamp.comodoca.com/authenticode /f "%signing_cert_path%" /p "%signing_cert_pass%" /v "%%A"
if errorlevel 1 exit /b %errorlevel%

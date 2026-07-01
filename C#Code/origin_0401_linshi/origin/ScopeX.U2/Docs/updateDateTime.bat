@echo off

:: 检查是否以管理员身份运行
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo 请以管理员身份运行此脚本！
    pause
    goto :eof
)

REM 修改“Windows Time”服务为自动启动并启动该服务
sc config w32time start= auto
net start w32time

REM 开启系统时间自动同步功能
reg add HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\W32Time\TimeProviders\NtpServer /v Enabled /t REG_DWORD /d 1 /f

REM 设置要同步的时间服务器
set ntpServer=time.windows.com

REM 使用w32tm命令同步时间
w32tm /config /manualpeerlist:%ntpServer% /syncfromflags:manual /reliable:yes /update
w32tm /resync

REM 显示同步结果
w32tm /query /status

REM 修改系统信息中的 “设备名称”
set key="HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\OEMInformation"
set value="Model"
set data="MSO7104X"

reg add %key% /v %value% /t REG_SZ /d %data% /f

pause
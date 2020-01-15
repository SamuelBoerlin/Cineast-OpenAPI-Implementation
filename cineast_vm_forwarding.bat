:START
@ECHO OFF
echo Helper script for setting up port forwarding to the cineast VM so it is accessible outside this machine.
echo Needs to be run with administrator rights.
set /p addOrRemove="Add (1) or remove (2) rules, or exit (3): "

if %addOrRemove%==3 exit /B 0
if not %addOrRemove%==1 (
  if not %addOrRemove%==2 (
    goto START
  )
)

set /p localAddr="Enter local address: "
set /p vmAddr="Enter VM address: "

set /p fileServerPort="Enter cineast file server port: "
if /i %addOrRemove%==1 (
  netsh interface portproxy add v4tov4 listenport=%fileServerPort% listenaddress=%localAddr% connectport=%fileServerPort% connectaddress=%vmAddr%
  netsh advfirewall firewall add rule name="cineast_file_server_vm_forward" protocol=TCP dir=in localip=%localAddr% localport=%fileServerPort% action=allow
) else (
  netsh interface portproxy delete v4tov4 listenport=%fileServerPort% listenaddress=%localAddr%
  netsh advfirewall firewall delete rule name="cineast_file_server_vm_forward"
)

set /p cineastPort="Enter cineast api server port: "
if /i %addOrRemove%==1 (
  netsh interface portproxy add v4tov4 listenport=%cineastPort% listenaddress=%localAddr% connectport=%cineastPort% connectaddress=%vmAddr%
  netsh advfirewall firewall add rule name="cineast_api_server_vm_forward" protocol=TCP dir=in localip=%localAddr% localport=%cineastPort% action=allow
) else (
  netsh interface portproxy delete v4tov4 listenport=%cineastPort% listenaddress=%localAddr%
  netsh advfirewall firewall delete rule name="cineast_api_server_vm_forward"
)

if /i %addOrRemove%==1 (
  echo Forwarded addresses:
  netstat -ano | findstr :%fileServerPort%
  netstat -ano | findstr :%cineastPort%
)

pause
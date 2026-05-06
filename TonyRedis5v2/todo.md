# BDYO Redis: Stage 5 / 115
### A. Psuedo-Steps
0. [**svr**] Start Server: `port 6969` OK
1. [**cli**] Sent Bytes to port `6969` OK
1. [**svr**] Capture Bytes & Print to `Console` OK
1. [**cli**] Sent Bytes: `*2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n`
1. [**svr**] Verify bytes received above
1. [**svr**] Receive Bytes: `*2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n`
1. [**svr**] Parse Bytes: `Parse(bytes_received)`
1. [**svr**] Send Response: `3\r\nhey\r\n`
1. [**cli**] Receive Response: `3\r\nhey\r\n`


### B. Request-Response Summary
|Receive|Response|
|-|-|
|RESP Array:<br>`*2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n`<br><br>RESP Encoding<br>`["ECHO", "hey"]`|**BULK STRING** type:<br>`3\r\nhey\r\n`|








<!-- |`redis-cli` Input|Server Response<br>|
|-|-|
|[**command**] PING |Response: `PONG`<br><br>Type:`RESP bulk string``PING`<BR><br>q`+PONG\r\n`||
[**command**] `ECHO`<br>[**argument (1)**] hey|hey|asdf|asdf| -->

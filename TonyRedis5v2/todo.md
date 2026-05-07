# BDYO Redis: Stage 5 / 115
### A. Psuedo-Steps
0. [**svr**] Start Server: `port 6969` OK
1. [**cli**] Sent Bytes to port `6969` OK
1. [**svr**] Capture Bytes & Print to `Console` OK
1. [**cli**] Send Specific Bytes: `*2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n`
    - **terminal**: redis-cli -p 6969 ECHO hey
    - [**validation required**] is recevied bytes == above?
1. [**svr**] Verify bytes received above
1. [**svr**] Receive Bytes: `*2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n`
1. [**svr**] Parse Bytes: `Parse(bytes_received)`
1. [**svr**] Send Response: `*3\r\nhey\r\n`
1. [**cli**] Receive Response: `*3\r\nhey\r\n`




### B. Request-Response Summary
|`redis-cli`<br>terminal command|[ACTUAL] Server Receives:<br>Raw-Bytes|[EXPECTED] Server Receives:<br>Raw-Bytes|Server Receives:<br>[Expected]`RESP` Encoding| Server Response:<br>[Expected]      |
|-|-|-|-|-|
|`redis-cli -p 6969 ECHO hey`|Decimal Byte Values:<br>`42 50 13 10 36 52 13 10 69 67 72 79 13 10 36 51 13 10 104 101 121 13 10`<br><br>RESP Array:<br>`*2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n`<br><br>RESP Array (formatted)-|:<br>`*2\r\n`<br>`$4\r\n`<br>`ECHO\r\n`<br>`$3\r\n`<br>`hey\r\n`<br><br>Hex Bytes:<>|`["ECHO", "hey"]` |**BULK STRING** type:<br>`*3\r\nhey\r\n` |-|




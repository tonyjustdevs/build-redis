
|client sends|server receive|server responds|client receives|
|-|-|-|-|
|`$ redis-cli SET foo bar PX 100` |hexy:<br>`somecoolhexybytes`<br><br>utf-8:<br>`someutf8string`|hexy:<br>`somecoolhexybytes`<br><br>utf-8:<br>`+OK\r\n` |ascii:<br>`OK`<br><br>type:<br>[RESP simple string](https://redis.io/docs/latest/develop/reference/protocol-spec/#simple-strings)|

# Testing Table
|Test_No|Test_Scenario|Server Received<br>Use in Validation|
|-|-|-|
|**[1]**|**setup**:<br>*cli*: `redis-cli SET foo bar PX 100`<br>*svr*: `OK`<br><br>**Immediately (<100 ms)**:<br>*cli*: `$ redis-cli GET foo`<br>*svr*: `$3\r\nbar\r\n`<br><br>**Anytime after (100+ ms)**:<br>*cli*: `$ redis-cli GET foo`<br>*svr*: `$-1\r\n`|**Received** UTF-8:<br>`$2\r\n...`<br><br>**Received** Hexy:<br>`$2\r\n...`|

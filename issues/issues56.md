# 1. Redis-Client Terminal Table
|Test_Steps|Test_Scenario|
|-|-|
|**[1]**|**Setup**:<br>*cli*: `redis-cli SET foo bar PX 100`<br>*svr*: `OK`|
|**[2]**|**Immediately (<100 ms)**:<br>*cli*: `$ redis-cli GET foo`<br>*svr*: `$3\r\nbar\r\n`|
|**[3]**|**Anytime after (100+ ms)**:<br>*cli*: `$ redis-cli GET foo`<br>*svr*: `$-1\r\n`|

# 2. Server Receives Table
|Encoding|Server Receives|
|-|-|
|UTF-8|`*5\r\n$3\r\nSET\r\n$3\r\nfoo\r\n$3\r\nbar\r\n$2\r\nPX\r\n$3\r\n100\r\n`|
|Hexy|`2A350D0A24330D0A5345540D0A24330D0A666F6F0D0A24330D0A6261720D0A24320D0A50580D0A24330D0A3130300D0A`|



# Z. OldTable
|client sends|server receive|server responds|client receives|
|-|-|-|-|
|`$ redis-cli SET foo bar PX 100` |hexy:<br>`somecoolhexybytes`<br><br>utf-8:<br>`someutf8string`|hexy:<br>`somecoolhexybytes`<br><br>utf-8:<br>`+OK\r\n` |ascii:<br>`OK`<br><br>type:<br>[RESP simple string](https://redis.io/docs/latest/develop/reference/protocol-spec/#simple-strings)|

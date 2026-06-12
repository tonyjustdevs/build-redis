


# `SET` via redis-cli terminal

|client sends|server receive|server responds|client receives|
|-|-|-|-|
|`$ redis-cli SET foo bar` |hexy:<br>`somecoolhexybytes`<br><br>utf-8:<br>`someutf8string`|hexy:<br>`somecoolhexybytes`<br><br>utf-8:<br>`+OK\r\n` |ascii:<br>`OK`<br><br>type:<br>[RESP simple string](https://redis.io/docs/latest/develop/reference/protocol-spec/#simple-strings)|

# Validation Psudo-Steps

|#| Description|Server Received<br>`UTF-8`|Server Received<br>`Hexy`|
|-| -|-|-|
|**[1]**| Validate `SET` 2 arguments<br>- `Key`<br>- `Value`<br><br>redis client:<br>`$ redis-cli -p 6969 SET foo bar`|**Expected/Received** UTF-8:<br>`*3\r\n$3\r\nSET\r\n$3\r\nfoo\r\n$3\r\nbar\r\n`|**Expected** Hexy:<br>`$2\r\n..`<br><br>**Received** Hexy:<br>`$2\r\n...`|

|Test_No|Test_Scenario|Server Received<br>Use in Validation|
|-|-|-|
|**[1]**|`$ redis-cli -p 6969 SET FOO BAR`|**Received** UTF-8:<br>`*3\r\n$3\r\nSET\r\n$3\r\nfoo\r\n$3\r\nbar\r\n`<br><br>**Received** Hexy:<br>`$2\r\n...`|

Misc: Im expecting
- redis-cli: `$ redis-cli -p 6969 SET foo bar` to translate to
- Bytes to UTF-8: `*3\r\n$3\r\nSET\r\n$3\r\nfoo\r\n$3\r\nbar\r\n`
- Actualy Recv'd: `*3\r\n$3\r\nSET\r\n$3\r\nfoo\r\n$3\r\nbar\r\n`
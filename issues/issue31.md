# `GET` via redis-cli terminal

# Scenario 1: Existing Key
|client sends|server receive|server responds|client receives|
|-|-|-|-|
|`$ redis-cli GET foo` |hexy:<br>`somecoolhexybytes`<br><br>utf-8:<br>`$2\r\n$3GET\r\n$3foo\r\n`|hexy:<br>`somecoolhexybytes`<br><br>utf-8:<br>`$1\r\n$3bar\r\` |ascii:<br>`bar`<br><br>type:<br>[RESP bulk string](https://redis.io/docs/latest/develop/reference/protocol-spec/#bulk-strings)|

# Scenario 2: Non-Existent Key
|sent| received| received type|
|-|-|-|
|`$ redis-cli GET donotexist` |`$-1\r\n` |[null bulk string](https://redis.io/docs/latest/develop/reference/protocol-spec/#null-bulk-strings)|

todo
- [ ] `redis-cli -p 6969 GET <ExistingKey>`
- [ ] confirm `GetCmd()` executed with test print
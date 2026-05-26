# ISSUE 27
case-insensitive-cmd-processing #24
- https://github.com/tonyjustdevs/build-redis/issues/27

todo
- RD1 test ping:
  - run `redis-cli PING`: result = OK
  - run `redis-cli ping`: result = NOT-OK

|Status|Result|Test|Request|Expected<br>Response|Actual<br>Response|Notes|
|-|-|-|-|-|-|-|
|✅| **PASS**|all upper-case: `PING`|`redis-cli PING`|`PONG`<br>|`PONG`|ok|
|⏳|tba|1 lower-case: `pING`|`redis-cli ping`|`PONG`<br>||||
|🔄|tba|all lower-case: `ping`|`redis-cli ping`|`PONG`<br>||||
|🔄|tba| mixed-case: `pIng`|`redis-cli pIng`|`PONG`<br>||||

Reference
- ✅ = Completed/PASS
- ❌ = Failed  
- ⏳ = In progress/Testing
- 🔄= To be tested

```c#
// terminal: redis-cli -p 6969 PING sends:
Segt:   
Hexy: "2A31 0D0A 2434 0D0A 50494E47 0D0A" (1 hexy string)
UTF8: " * 1 \r\n  $ 4 \r\n  P I N G \r\n" (1 utf8 string)
```


steps: 
1. receive `raw_bytes`  convert to `hexy_bytes`
2. if `hex[0..17]` matches `2A31 0D0A 2434 0D0A`
    - `* 1 \r\n  $ 4 \r\n` 
    - `1` **command** of `4` **bytes**
3. Split `hexy_bytes` by limiter: `0D0A` to: 
   - `byte[] splited_hexy_bytes`

```C#
Segt:   [0]  [1]      [2]
Hexy: "2A31 2434 50494E470D0A"
UTF8: " * 1  $ 4  P I N G\r\n"
```

4. get `splited_hexy_bytes[2]`: "`50494E470D0A`"
5. read first `4`-bytes `50494E47` and convert to **utf8-string**:
   - `cmd_str`
6. apply `string.Upper()` to `cmd_str`:
   - `cmd_str_upper`
7. compare against `PING` or `ECHO`
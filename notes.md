### `dotnet`
- search the sdk & runtimes
    - `apt search dotnet-runtime` 
    - `apt search dotnet-sdk`
- dl download only dotnet: `sudo apt install --download-only dotnet-sdk-8.0`, stored in `/var/cache/apt/archives/`
    - check size: `du sh /var/cache/apt/archives/`
- dl & install: `sudo apt install dotnet-sdk-8.0`


### Computer Architecture Overview

|OS & CPU| Description|Values|Analogy|
|-|-|-|-|
|32-bit address|addys are max 32-bit|Addy just a number in `bytes`<br><br>**1000** is stored as:<br>- 32-bit pointer: `00000000 00000000 00000011 11101000`<br>- 64-bit pointer: `00000000 00000000 00000000 00000000 00000000 00000000 00000011 11101000`|Digit & Bit Systems for House Numbers:<br>- 2-digit system = 99<br>- 4 digit system: 9999<br><br>therefore:<br>- 4-bit system: 4.29 billion addresses|


### Visual Studio 2026
Configuration Manager
| Option  | CPU| Description|
|-|-|-|
| Platform|CPU architecture target|The **CPU Processor**<br>- the compiled program<br>- will ***run*** on|
|`x86`  | 32-bit Intel/AMD|- 2 gb max addressable memory<br>- 32-bit process cannot load 65-bit DLL|
|`x64`  | 64-bit Intel/AMD|- tb range memory space|
|`arm32`| ARM 32-bit|- low-power devices<br>- embedded systems|
|`arm64`| ARM 64-bit|- modern ARM devices|

### `.NET Core/.Net 5+ Vs `C` Execution Model
|Language|Model|
|-|-|
|`C#`|`C#`<br>-> compile -> `IL`<br>-> `.NET` assembly (`.dll` &/or `.exe`)<br>-> `CLR` loads it (only cares there a `Main()` not `.exe`/`.dll`)<br>-> `JIT` compiles -> `machine code` <br>-> executes|
|`C`|`c`<br>`.c` -> compile -> `machinecode` (`elf` or `.exe`) -> `OS` loads directly


### Executable Types `.exe`:  
#### **native host stub**
|Executable Type `.exe`|Description|Example|
|-|-|-|
|Native Host Stub|<br>- 1. locate dotnet runtime <br>- 2. load dll<br>- 3. call entry point|`tony.exe` -> loads -> `tony.dll` -> runs `Main()`|
|Self-Contained `publish`|Includes:<br>- source code<br>- runtime<br>- dependencies|`dotnet publish -r win-x64 --self-contained true`|

### Installing redis official `cli`
Link: https://redis.io/docs/latest/operate/oss_and_stack/install/install-stack/apt/
`sudo apt-get install lsb-release curl gpg`
`curl -fsSL https://packages.redis.io/gpg | sudo gpg --dearmor -o /usr/share/keyrings/redis-archive-keyring.gpg`
`sudo chmod 644 /usr/share/keyrings/redis-archive-keyring.gpg`
`echo "deb [signed-by=/usr/share/keyrings/redis-archive-keyring.gpg] https://packages.redis.io/deb $(lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/redis.list`
`sudo apt-get update`
`sudo apt-get install redis`

Manual: https://redis.io/docs/latest/develop/tools/cli/ 

### TCP stuff: Wireshark
World's most popular network protocol analyzer

Precision Configuration
|Setting|Value|Description|Example|
|-|-|-|-|
| Choose Interface|`Standard User`:<br>- `vEthernel (WSL)`|The **virtual interface** that is WSL2's *window* to Windows networking<br>- The ONLY interface carrying WSL2 localhost traffic|-`tcp port 63791`<br>- `host 127.0.01`|
|Capture Filter<br>- port number| `port 6379`|Reduce noise<br>- only captures Redis port traffic|`tcp.port == 6379 && tcp.flags.syn == 1`|
|Buffer Size| Default: 2MB|Enough for Redis<br>- prevent drops<br>- Capture filters run in kernel mode - they're CPU-efficient|



#### 3. Generate Traffic
```bash
# Start your Redis server (replace with your actual command)
./your_redis_server --port 6379

# In a NEW terminal window (don't stop capture!)
redis-cli PING PING QUIT
```

Note: `QUIT` command generates a clean TCP teardown (essential for seeing full connection lifecycle).

#### 4. Real-Time Analysis
1. The `TCP` Handhake
2. Redis Protocol Dissection
3. Critical Sequence Numbers:
- TCP: Seq #
- Next Seq #
- Ack #. 

#### 5. Various Techniques
|Techniques|How to|
|-|-|
|1. Follow Conversation|Select Redis package<br>- right-click <br>- follow<br>- tcp stream<br>- to ASCII|
|2. Find Protocol Errors|`tcp.analysis.retransmission \|\| tcp.analysis.out_of_order \|\| redis`|
|3. Measure Performance


Smoke test:
- adapter for loopback traffice capture:  loopback adapter (double click)
- ping 127.0.0.01

# Grok Suggestion 
|Solution|File|File Edit|
|-|-|-|
|[0a] Check whats listening to specific ports |`ss -ltnp \| grep 6379`|Sample output:<br>- `LISTEN 0      511      172.27.49.71:6379       0.0.0.0:*`<br>- `LISTEN 0      511         127.0.0.1:6379       0.0.0.0:*`<br>- `LISTEN 0      511             [::1]:6379          [::]:`|
|[0b] Check process |`ps aux \| grep redis-server`|Sample output:<br>- `/usr/bin/redis-server 127.0.0.1:6379`|
|[0b] Check redis server status |`sudo systemctl status redis-server`|Sample output:<br>- `Active: active (running)`|
| [1] Edit Redis Config| `sudo nano /etc/redis/redis.conf`|[1a] Bind to all interface (or specifically your IP)<br>- `bind 0.0.0.0 ::0` or<br>[1b] Bind only your specific IP:<br>- bind `172.27.49.71`<br>[1c] Both:<br> - `bind 127.0.0.1 ::1 172.27.49.71`<br><br>[3] Disable protected mode (if you're in a trusted network)<br>- `protected-mode no`<br><br>[4] Restart:<br>- `sudo systemctl restart redis-server`<br>- `sudo service redis restart`<br><br>[5] Test again:<br>- `sudo ss -ltnp \| grep 6379` <br>- `redis-cli -h 172.27.49.71 PING`<br><br>[6] Capture traffic in WS:<br>- `ip.addr == 172.27.49.71 && tcp.port == 6379``|
|[2] Test via `TCP`|`telnet 172.27.49.71 6379`|


### TCP from Windows to WSL Redis-Server
[Powershell] Test-NetConnection 172.27.49.71 -Port 6379:
- `34	238.506415600	TCP	172.27.48.1	62763	172.27.49.71	6379	66	62763 → 6379 [SYN] Seq=0 Win=65535 Len=0 MSS=1460 WS=256 SACK_PERM	62763	62763	6379	6379	238.506415600	66`

### Links
pipelines: 
- `https://devblogs.microsoft.com/dotnet/system-io-pipelines-high-performance-io-in-net/?spm=a2ty_o01.29997173.0.0.299e55fbzZzmb2#system.io.pipelines`

sockets:
- `https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/socket-services?spm=a2ty_o01.29997173.0.0.299e55fbzZzmb2#create-a-socket-server`

Networking Programming with C:
- `https://github.com/codeplea/Hands-On-Network-Programming-with-C`

Beej's Guide to Network Programming:
- `https://beej.us/guide/bgnet/html/split/


# `telnet` summary
|Server|Client|Result|
|-|-|-|
|**Windows**<br>- Redis Server|**WSL**<br>- `telnet 127.0.0.1 6969`|`telnet: Unable to connect to remote host: Connection refused`|
|**WSL**<br>- Redis Server|**WSL**<br>- `redis-cli -p 6969 PING`|OK|


# Useful commands
|bash|description|example|
|-|-|-|
|`$(...)`|1. runs ... in child process<br>2. substitutes result back |change dir into dir of current file:<br>- `cd $(dirname $0)`|
|`\r` Carriage Return|move cursor start of line|ascii 13|
|`\n` Line Feed|move down one line|ascii 10|

# `RESP` Protocol
|Command|Description|Examples|
|-|-|-|
|**Integer**:<br>`*[<+\|->]<value>\r\n`|`:` first byte<br><br>`[<+\|->]`: optional choice between `+` & `-`<br><br>`value`: obvious|**0** integer response:<br>`:0\r\n`<br><br>**1000** integer response:<br>`:1000\r\n`|
|**Bulk String**:<br>`$<lengh>\r\n<data>\r\n`|`$`: first byte to say a bulk string is coming<br><br>`length`: length of string in bytes<br><br>`\r\n`: CRLN (carriage return line feed) or reset cursor & move next line<br><br>`data`: self-explanatory|**"hello"**:<br>`$5\r\nhello\r\n`<br><br>empty-string **""**:<br>`$0\r\n`<br><br>null bulk string:<br>`$-1\r\n`|
|Simple **Arrays**:<br>`*<number-of-elements>\r\n<element-1>...<element-n>`<br><br>Note:<br>- Clients send commands to Redis server as RESP arrays|`*`: first byte identifier<br><br>`nbr_of_elemnts_in_arr`: self-explanatory<br><br>`element_1...element_n`:<br>each elements whole resp response sequentially|**empty arrays**: `*0\r\n`<br>**null arrays**: `*-1\r\n`<br><br>**["hello","world!"]**:<br>`*2\r\n$5\r\nhello\r\n$6\r\nworld!\r\n`<br>check:<br>`*2\r\n$5\r\nhello\r\n$5\r\nworld\r\n`<br><br>note:<br>**"hello"**:`$5\r\nhello\r\n`<br>**"world!"**: `$6\r\nworld!\r\n`<br><br>arrays can contain **mixed types**:<br>**[42 ,666,"hello"]:**<br>`*3\r\n`<br>`:42\r\n`<br>`:666\r\n`<br>`$5\r\n`<br>`hello\r\n`|
|Mixed/Nested **Arrays**:<br> as above|as above|mixed **[42 ,666,"hello"]:**<br>`*3\r\n`<br>`:42\r\n`<br>`:666\r\n`<br>`$5\r\n`<br>`hello\r\n`<br><br>nested arrays **[[111,222,333],[gday,mateys]]**<br>`*2\r\n`<br>`*3\r\n`<br>`:111\r\n`<br>`:222\r\n`<br>`:333\r\n`<br>`*2\r\n`<br>`+gday\r\n`<br>`+mateys\r\n`|

# Some Notes
`*2\r\n`: 
- is squence of **raw bytes**
- each byte can be `char`, `hex`, `binary`

|#|char(s)|ascii value<br>`ord("char")`|hex<br>`hex(ascii_int)`|binary<br>`bin(ascii_int)`|
|-|-|-|-|-|
|All|`*2\r\n`|- |-      |-          |`
|1  |`*`     |42|`0x2a` |`0b101010` |
|2  |`2`     |50|`0x32` |`0b110010` |
|3  |`\r`    |13|`0xd`  |`0b1101`   |
|4  |`\n`    |10|`0xa`  |`0b1010`   |

|Python|Example|Description|
|-|-|-|
|`b""`         |`bytes = b"*2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n"`|This is bytes, not normal Unicode text|
|`bytes.hex()` |`bytes.hex()`:<br>= `"2a320d0a24340d0a4543484f0d0a24330d0a6865790d0a"`|*Bytes* to **hex-string**|
|`f"{b:02X}"`  |`print(" ".join(f"{b:02X}" for b in bytes))`:<br>= `2A 32 0D 0A 24 34 0D 0A 45 ..`|Bytes to *prettier* **hex-string**|
|`list(bytes)"`|`print(list(bytes))`:<br>= `[42, 50, 13, 10, 36, 52, ..]`|Bytes to **decimal byte values**|
|`bytes.decode("ascii")"`|`print(msg.decode("ascii"))`:<br>`*2`<br>`$4`<br>`ECHO`<br>`$3`<br>`hey`<br><br>note: `\r\n` $\to$ actual newlines.|Bytes to **ascii**|
|`s = "*2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n"`<br>`ascii_values = [ord(c) for c in s]`|`[42, 50, 13, 10, 36, 52, 13...] `|string to ascii values|
|`hex_str = s.encode('ascii').hex()`|`2a320d0a24340d0a454`|string to hex|
|`binary_str = ''.join(format(ord(c), '08b') for c in s)`|`001010100011001000001101...`|string to Binary representation (8 bits per character)|
|`repr(bytes.fromhex(hex_str).decode('ascii'))`|`*2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n`|bytes to og esaped msg|
|`ascii_list = [42,50,13,10,36,52,13,10,69,67,72,79,13,10,36,51,13,10,104,101,121,13,10]`<br>`data = bytes(ascii_list)`<br>`data.decode('ascii', errors='replace')`(str)<br>`da=ta.hex()`(hex)<br>`' '.join(f'{b:02x}' for b in data)`(hex space)<br>`' '.join(f'{b:08b}' for b in data)`(bin bits)|data:<br>`b'*2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n'`|decimal bytes to raw bytes |
|`int("11000011",2)`|binary value of bytes|

|terminal|redis-cli sends|
|-|-|
|`printf "*2\r\n:666\r\n:420\r\n" \| nc localhost 6969`| `*2:666:420`|



|Useful C# Methods|Output|Namespace|
|-|-|-|
|`Convert.ToHexString(byte[])`|string: `"2A320D0A24340D0A4D41..."`|`System`|
|`BitConverter.ToString(byte[])`|string: `"2A-32-0D-0A-24-34-0D-0A-4D-41-..."`|`System`|
|`foreach(...){WL($"{b:X2} ");}`|string: `"2A 32 0D 0A ..."`|`System.Text.Encoding.ASCII.GetBytes(<RAW_BYTES>)`|
|`foreach(...){WL($"{b} ");}`|int: `"..."`|`System.Text.Encoding.ASCII.GetBytes(<RAW_BYTES>)`|
|`ASCII.GetString(msg)`|string: `"..."`|`System.Text.Encoding.ASCII.GetString(<RAW_BYTES>)`|
|`byte[] bytes = Encoding.ASCII.GetBytes(s);`<br>`Wr("ASCII Decimal: " + string.Join(", ", bytes));`| `42, 50, 13, 10, 36, 52...` |RESP msg to  ASCII values (decimal)|
|`string hex = BitConverter.ToString(bytes).Replace("-", "").ToLower();`<br>`Wr("\nHex: " + hex);`|`2a320d0a24340d0a4543484`|string to hex|
|`byte[] hexBytes = Enumerable.Range(0, hex.Length / 2`<br>`.Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16))`<br>`.ToArray();`<br>`string originalFromHex = Encoding.ASCII.GetString(hexBytes);`<br>`Wr("\nFrom Hex back to string: "`<br>` + originalFromHex`<br>`.Replace("\r", "\\r").Replace("\n", "\\n"));`|`*2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n`|hex to **string**|


# GITHUB WORKFLOW
Create **Development** Branch `dev`(once): 
- `git checkout -b dev` 
- `git push -u origin dev`

Create **Feature** Branch `feature`
- `git checkout -b feature/the-cool-new-feature`
- `git push -u origin feature/the-cool-new-feature`

| Prefix      | Meaning               |
| `feature/`  | new feature           |
| `bugfix/`   | bug fix               |
| `hotfix/`   | urgent production fix |
| `refactor/` | architecture cleanup  |
| `test/`     | experiments/tests     |
| `release/`  | release preparation   |

 
**TCP DUMP**
Goal: 
- Capture traffice inside WSL2 (export to file)
- Open file in Windows Wireshark

**Capture in WSL2**
- Capture traffic loopback `lo` (client & server comms on `127.0.0.1`), filter port `6969`, save file `.pcap`:
  - `sudo tcpdump -i lo port 6969 -w tcp_capture.pcap`
- Run server and send msgs then disconnect

Common intefaces:
- `-i eth0` — First ethernet adapter
- `-i wlan0` — Wi-Fi adapter
- `-i any` — Listen on ALL interfaces at once

Other traffic filter expression
-`port 6969 and tcp` — Only TCP traffic on port 6969
-`port 6969 or port 8080` — Capture both ports
-`host 192.168.1.5 and port 6969` — Only from/to a specific IP

Live
`sudo tcpdump -i lo port 6969`

Flags

- `SYN`:    Connection opening (three-way handshake)
- `PSH`/    `ACK`: Actual data transfer
- `FIN`:    "receive returns 0" moment!
- `[S]`:    SYN flag set
- `[S.]`:   SYN-ACK (both SYN and ACK flags)
- `[.]`:    ACK only
- `[P.]`:   PUSH-ACK (data with ACK)
- `[F.]`:   FIN-ACK (finish with ACK)

# Encoding
Packet has two major parts, they may be encoded differently:
- structural (redis: `ascii`)
- payload (redis: `raw bytes`)


# `.NET string` are stored as `UTF-16` Unicode characters
- A string is a sequence of characters.
- Each character is `UTF-16` (2-bytes or 4 hex-values, 1 byte has 2 hex-values)

# Redis serialization protocol  (`RESP`)
Client sends request to Redis Server as:
- an array of strings (`commands` & `arguments`)

### 1. Network Layer 
Default port: `6379`

### 2. Request-Response model 
- Requests can be `pipedlined`: Allow clients to send **multiple commands** at once and wait for replies later.
- `RESP2` connection subscribed to a `Pub/Sub` Channel, `push` protocol
  - client **not required** to send `commands`
  - server automically sends messages to simps.
- `Monitor` **command**: connection becomes ad-hoc push mode.
- `Protected mode`: `-DENIED` (denied & terminated) reply to `non-loop` address
- `RESP3 Push` type. Not specific to any client **command**. Server sends out-of-band data.

### 3. RESP protocol description 
First `byte` determines `data type`

RESP is a `request-response` protocol:
- Client sends `array` of `bulk-strings` 
  - `Command`: `First` (& sometimes Second) `bulk-string` (of the array)
  - `Arguments`: of the command, are **subsequent** elements of the array 

`Control Sequences` encoded in standard `ASCII`.

3 RESP data types:
- `simple`: 
  - e.g. plain literal values, booleans, integers, string (without carraige return `\r` & line feed `\n`)
- `bulk`: 
  - e.g. bulk-strings contain any binary data (binary or blob)
  - can be encoded/decoded further, e.g. wide mult-byte encoding, by client.
- `aggregate`: e.g. `Arrays` & `Maps`, varying numbers of sub-elements & nesting levels.

#### 4. Data Types
|Type|$1^{st}$ `byte`|Command|Example|Client Parser Pseudo|`C#` sample|
|-|-|-|-|-|-|
|**Simple strings**<br><br>format:<br>`+<actual-data>\r\n`|`+`|Any **string**<br><br>Must not contain:<br>`\r` or `\n`<br><br>Termination:<br>`\r\n`|`+OK\r\n` (wire format) <br><br>`+`: protocol metadata<br>`OK`: payload<br>`\r\n`: termination marker|- See `43` (`+`): a simple string incoming<br>- collect bytes (aka extra payload) until `13 10` (`\r\n`)|`string raw = "+OK\r\n"`<br><br>`string parsed = raw.Substring(1, raw.Length - 3)`|
|**Integers**<br><br>format:<br>`:[<+\|->]<value>\r\n`|`:`|tba|`:0\r\n`<br><br>`:1000\r\n`|tba|tba|tba|
|**Bulk strings**<br><br>format:<br>`$<length>\r\n<data>\r\ns`|`$`|tba|`$5\r\nhello\r\n`: hello<br><br>`$0\r\n\r\n`: empty string<br><br>`$-1\r\n`: null bulk string|tba|tba|tba|
|**Arrays**<br><br>format:<br>`*<number-of-elements>\r\n<element-1>...<element-n>`|`*`|tba|`*0\r\n`: empty array<br><br>`*2\r\n$5\r\n`:array of two bulk strings "hello" & "world!"|tba|tba|tba|


**Q**&A
Q: What is `Encoding.UTF8.GetBytes("+PONG\r\n")` ?
A: `C#` stores strings (a list of `characters` within `""`), such as `"hello"` or `"+PONG\r\n"` as `utf-8` encoding.

What is `utf-8`, it is a set of rules for intepreting to and from `bytes`. Recall, a `byte` is a way to represent a number with `1` & `0`s (e.g. `0000-0001` is `1`, `0000-0010` is `2`, & `0000-0011` is 3 etc). 
Bytes, or numbers, by themselves, have no meaning. 

`UTF-8` is a mapping (aka **character encoding**) between a set of characters (officially **Unicode characters**) which humans can read (e.g. 'h', '🍆', 'ẩ').

# Search for these red flags:  
- `+=` in a loop → Replace with `StringBuilder`  
- `new byte[100000]` → Replace with `ArrayPool<byte>.Shared.Rent()`
- `await Task.Run(() => Thread.Sleep(...))` → Delete and use async I/O instead
- Run a **memory profile** → See the 10-100x reduction in allocations

`RESP` $Array \rightarrow$ [`Bulk-String` $command$, `Bulk-String` $args...$]


Examples
|redis-client:<br>command‑line argument|data structure|Redis Serialization Protocol (`RESP`) |
|-|-|-|
|`PING` | array `["PING"]` | `*1\r\n$4\r\nPING\r\n`|
|`SET mykey Hello` | array `["SET","mykey","Hello"]` | `*3\r\n$3\r\nSET\r\n$5\r\nmykey\r\n$5\r\nHello\r\n`|

|tcpdump args|description|
|-|-|
|`sudo tcpdump -i lo -X -s 0 port 6969`|gotta catch'em all|
|`-i lo`    |  capture on the loopback interface (traffic inside your own machine).|
|`-X`       | display the packet data in both hex and ASCII (so you can see the actual bytes).|
|`-s 0`     | snap length zero -> capture the full packet (the output shows 262144 bytes max).|
|`-nn`      | no name resolution: `127.0.0.1` instead of `localhost`|
|`port 6969`| only show traffic involving TCP port 6969|
|`sudo tcpdump -i lo -s 0 -w redis_capture.pcap port 6969`|Write File:<br>- Capture on `loopback`:<br>- if app and Redis are on **same machine**|
|`sudo tcpdump -i lo -s 0 -w redis_capture.pcap 'port 6969 or port 6379'`|Write File:<br>- Multiple ports|
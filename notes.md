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
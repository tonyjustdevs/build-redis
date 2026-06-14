expected server response: 

|encoding| string|
|-| -|
| utf8| `*1\r\n$3\r\niii\r\n`|
| hexy |`2A310D0A24330D0A6969690D0A`|

```C#
2A31 0D0A 2433 0D0A 696969 0D0A
 * 1 \r\n  $ 3 \r\n  i i i \r\n
```

|utf8|description|acquired|
|-|-|-|
|`*`    |array length|✅|
|`\r\n` |next-line|✅|
|`$6`   |length of key payload|✅|
|`\r\n` |next-line|✅|
|`iiiii`|get payload|✅|
# https://github.com/tonyjustdevs/build-redis/issues/28

https://github.com/tonyjustdevs/build-redis/issues/28

Reference: https://redis.io/docs/latest/commands/echo/

### Test Table
|Status|Result|Test|Request|Expected<br>Response|Actual<br>Response|
|-|-|-|-|-|-|
|🔄| **TBA**|all upper-case:`ECHO`      |`redis-cli ECHO a1B!c3de&`|`a1B2c3d!e&`| TBA |
|🔄| **TBA**|all lower-case:`echo`      |`redis-cli echo a1B2c3d!e&`|`a1B2c3d!e&`| TBA |
|🔄| **TBA**|mixed-case:    `eCHo`      |`redis-cli eCHo a1B2c3d!e&`|`a1B2c3d!e&`| TBA |

### Psuedo-Steps
1. Test with official `redis-cli`
### Test Emoji Reference
- ✅ = Completed/PASS
- ❌ = Failed  
- ⏳ = In progress/Testing
- 🔄 = To be tested


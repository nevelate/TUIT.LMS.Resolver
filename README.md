# TUIT.LMS.Resolver
Library used to get data from [lms.tuit.uz](https://lms.tuit.uz). Currently implemented only student related functions.

## Token / Captcha
To get token & captcha go to [lms.tuit.uz/auth/login](https://lms.tuit.uz/auth/login)
![auth page](auth_page.png)
There are 2 hidden inputs. Value of "_token" is token and value of "g-recaptcha-response" is grecaptcha.

## Usage
Library consists of two main classes:
 - LMSAuthService - responsible to auth/login and communication with server
 - LMSResolver - responsible to parse data

### LMSAuthService
```csharp
event Action? LoginRequested; // fires when unable to get data
async Task<bool> TryLoginAsync(string login, string password, string token, string grecaptcha); // Try to login, requires token and captcha
void LogOut(); // log out from account
CheckIfNeededReLogin(); // fires LoginRequested if needed relogin
```

### LMSResoler
```csharp

```

## Example
You can find basic example in **TUIT.LMS.Resolver.Examples**.

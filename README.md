# TUIT.LMS.Resolver
Library used to get data from [lms.tuit.uz](https://lms.tuit.uz). Currently implemented only student related functions.

## Token / Captcha
To get token & captcha go to [lms.tuit.uz/auth/login](https://lms.tuit.uz/auth/login)
![auth page](auth_page.png)
There are 2 hidden inputs. Value of "_token" is **token** and value of "g-recaptcha-response" is **grecaptcha**.

## Usage
Library consists of two main classes:
 - LMSAuthService - responsible to auth/login and communication with server
 - LMSResolver - responsible to parse data

### LMSAuthService
```csharp
event Action? LoginRequested; // fires when unable to get data
async Task LoginAsync(string login, string password, string token, string grecaptcha);
 // Log in, requires token and captcha. Throws exception when unable to login.

void LogOut(); // log out from account
CheckIfNeededReLogin(); // fires LoginRequested if needed relogin
```

### LMSResolver
```csharp
async Task<Information> GetInformationAsync(); // get student information
async Task<List<News>> GetNewsAsync(int page = 1); // get news from dashboard page
async Task<List<Discipline>> GetDisciplinesAsync(); // get disciplines from Individual study plan page
async Task<List<Lesson>> GetLessonsAsync(int courseId, LessonType lessonType); // get all lessons 

enum LessonType
{
    Lecture,
    Practice,
    Laboratory
}

async Task<AssignmentsPage> GetAssignmentsPageAsync(int courseId); // get assignments (deadlines) page
async Task<Dictionary<int, string>> GetSemesterIdsAsync(); // get semester Ids with their names
async Task<List<T>> GetLMSObjectsAsync<T>(int semesterId); // get Course, Absence, TableLesson or Final
async Task<bool> UploadFileAsync(string filePath, int courseId, int uploadId); // upload deadline
async Task<bool> UploadFileAsync(Stream stream, string fileName, int courseId, int uploadId); // upload file (using stream)
async Task<string?> GetAccountFullName(); // get normalized account full name
async Task<TableLessonType> GetLessonSideAsync(int semesterId, TableLessonType firstWeekTableLessonSide = TableLessonType.left); // get current week lesson side (left or right)

public enum TableLessonType
{
    full = 1,
    left,
    right,
}

async Task<bool> ChangeLanguageAsync(string language); // change LMS language

//You can use constants from Languages class
public static class Languages
{
    public const string English = "en";
    public const string Russian = "ru";
    public const string UzbekCyrilic = "uzc";
    public const string UzbekLatin = "uzl";
    public const string Karakalpak = "kar";
}

async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword); // change account password
```

## Example
You can find basic example in **TUIT.LMS.Resolver.Examples**.

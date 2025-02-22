using AngleSharp.Browser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TUIT.LMS.Resolver.LMSObjects;

namespace TUIT.LMS.Resolver
{
    public class LMSResolver
    {
        private const string MyCoursesUrl = "https://lms.tuit.uz/student/my-courses/data?semester_id=";
        private const string FinalsUrl = "https://lms.tuit.uz/student/finals/data?semester_id=";
        private const string ScheduleUrl = "https://lms.tuit.uz/student/schedule/load/";
        private const string AbsencesUrl = "https://lms.tuit.uz/student/attendance/data?semester_id=";

        private const string ChangeLanguageRequestFormat = "_token={0}&language={1}";
        private const string ChangePasswordRequestFormat = "_token={0}&old_password={1}&password={2}&password_confirmation={3}";

        private LMSAuthService _authService;

        private readonly Dictionary<string, string> uploadRequestHeaders;

        public LMSResolver(LMSAuthService authService)
        {
            _authService = authService;

            uploadRequestHeaders = new()
                {
                    {"Accept", "*/*"},
                    {"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36"},
                    {"Origin", "https://lms.tuit.uz"},
                    {"sec-ch-ua", "\"Not/A)Brand\";v=\"8\", \"Chromium\";v=\"126\", \"Google Chrome\";v=\"126\""},
                    {"sec-ch-ua-mobile", "?0"},
                    {"sec-ch-ua-platform", "\"Windows\""},
                    {"Sec-Fetch-Dest", "document"},
                    {"Sec-Fetch-Mode", "navigate"},
                    {"Sec-Fetch-Site", "same-origin"},
                    {"Sec-Fetch-User", "?1"},
                    {"Upgrade-Insecure-Requests", "1"},
                    {"Host", "lms.tuit.uz"},
                    {"X-Requested-With", "XMLHttpRequest" },
                };
        }

        public async Task<Information> GetInformationAsync()
        {
            await _authService.CheckIfNeededReLogin();
            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/student/info");
            var information = new Information()
            {
                FullName = document.QuerySelectorAll("div.card.relative p.m-b-xs")[0].TextContent.RemoveUpToColonAndTrim(),
                BirthDate = DateOnly.Parse(document.QuerySelectorAll("div.card.relative p.m-b-xs")[1].TextContent.RemoveUpToColonAndTrim(), new CultureInfo("ru-RU")),
                Gender = document.QuerySelectorAll("div.card.relative p.m-b-xs")[2].TextContent.RemoveUpToColonAndTrim(),
                StudentNumber = document.QuerySelectorAll("div.card.relative p.m-b-xs")[3].TextContent.RemoveUpToColonAndTrim(),

                Address = document.QuerySelectorAll("div.card.relative p.m-b-xs")[4].TextContent.RemoveUpToColonAndTrim(),
                TemporaryAddress = document.QuerySelectorAll("div.card.relative p")[6].TextContent.RemoveUpToColonAndTrim(),

                Base64Photo = document.QuerySelector("div.card.relative p.text-center.m-b-md img")?.GetAttribute("src"),

                Specialization = document.QuerySelectorAll("div.card:not(.relative) p.m-b-xs")[0].TextContent.RemoveUpToColonAndTrim(),
                StudyLanguage = document.QuerySelectorAll("div.card:not(.relative) p.m-b-xs")[1].TextContent.RemoveUpToColonAndTrim(),
                Degree = document.QuerySelectorAll("div.card:not(.relative) p.m-b-xs")[2].TextContent.RemoveUpToColonAndTrim(),
                TypeOfStudy = document.QuerySelectorAll("div.card:not(.relative) p.m-b-xs")[3].TextContent.RemoveUpToColonAndTrim(),
                Year = int.Parse(document.QuerySelectorAll("div.card:not(.relative) p.m-b-xs")[4].TextContent.RemoveUpToColonAndTrim()),
                Group = document.QuerySelectorAll("div.card:not(.relative) p.m-b-xs")[5].TextContent.RemoveUpToColonAndTrim(),
                Tutor = document.QuerySelectorAll("div.card:not(.relative) p.m-b-xs")[6].TextContent.RemoveUpToColonAndTrim(),
                Stipend = document.QuerySelectorAll("div.card:not(.relative) p")[7].TextContent.RemoveUpToColonAndTrim(),
            };
            return information;
        }

        public async Task<List<News>> GetNewsAsync(int page = 1)
        {
            await _authService.CheckIfNeededReLogin();
            var newsUrl = page switch
            {
                1 => "https://lms.tuit.uz/dashboard/news",
                _ => "https://lms.tuit.uz/dashboard/news?page=" + page,
            };

            var document = await _authService.GetHTMLAsync(newsUrl);
            List<News> news = new List<News>();
            List<Task<IDocument>> htmlDocumentTasks = [];

            foreach (var column in document.QuerySelectorAll("div.row div.col-md-4 div.card.p"))
            {
                var url = column.QuerySelector("div.d-flex a")?.GetAttribute("href");
                htmlDocumentTasks.Add(_authService.GetHTMLAsync(url));
            }

            var htmlDocuments = await Task.WhenAll(htmlDocumentTasks);

            foreach (var newsPage in htmlDocuments)
            {
                news.Add(new()
                {
                    Title = newsPage.QuerySelector("h4.panel__title")?.TextContent,
                    Description = newsPage.QuerySelector("div.panel-body blockquote div")?.TextContent,
                    NewsDate = DateOnly.Parse(newsPage.QuerySelector("div.panel-body blockquote footer cite").TextContent.Replace('-', '.'))
                }
                );
            }

            return news;
        }

        public async Task<List<Discipline>> GetDisciplinesAsync()
        {
            await _authService.CheckIfNeededReLogin();
            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/student/study-plan");

            List<Discipline> disciplines = new List<Discipline>();

            foreach (var row in document.QuerySelectorAll("div.page-inner > div.row"))
            {
                foreach (var card in row.QuerySelectorAll("div.col-lg-6 > div.card"))
                {
                    foreach (var tr in card.QuerySelectorAll("table > tbody > tr"))
                    {
                        disciplines.Add(new()
                        {
                            Title = tr.QuerySelector("td").TextContent,
                            Semester = card.QuerySelector("div.card-body > p").TextContent,
                            CreditCount = int.Parse(tr.QuerySelector("td.text-center").TextContent),
                            Grade = string.IsNullOrWhiteSpace(tr.QuerySelector("td.text-right").TextContent) ? null : int.Parse(tr.QuerySelector("td.text-right").TextContent),
                        });
                    }
                }
            }

            return disciplines;
        }

        public async Task<List<Lesson>> GetLessonsAsync(int courseId, LessonType lessonType)
        {
            await _authService.CheckIfNeededReLogin();
            var dateRegex = new Regex(@"\(.*\)");

            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/student/calendar/" + courseId);
            List<Lesson> lessons = new List<Lesson>();

            string lessonTypeQuery = lessonType switch
            {
                LessonType.Lecture => "div#lecture tbody > tr",
                LessonType.Practice => "div#practice tbody > tr",
                LessonType.Laboratory => "div#lab tbody > tr"
            };

            foreach (var tr in document.QuerySelectorAll(lessonTypeQuery))
            {
                var lesson = new Lesson
                {
                    ThemeTitle = tr.QuerySelector("td p").TextContent,
                    ThemeNumber = int.Parse(tr.QuerySelectorAll("td")[0].TextContent),
                    LessonDate = DateOnly.Parse(dateRegex.Replace(tr.QuerySelectorAll("td")[2].TextContent, (m) => string.Empty)),
                    LessonType = lessonType,
                };

                var attachments = new List<LMSFile>();

                foreach (var a in tr.QuerySelectorAll("td a"))
                {
                    attachments.Add(new(a.TextContent.Trim('\n', ' ', '\t'), a.GetAttribute("href")));
                }

                lesson.Attachments = attachments;

                lessons.Add(lesson);
            }

            return lessons;
        }

        public async Task<AssignmentsPage?> GetAssignmentsPageAsync(int courseId)
        {
            await _authService.CheckIfNeededReLogin();
            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/student/my-courses/show/" + courseId);

            if (document.QuerySelector("div.page-inner > div.panel") == null) return null;

            AssignmentsPage assignmentsPage = new AssignmentsPage()
            {
                AchievedPoints = float.Parse(document.QuerySelectorAll("tbody tr td h4")[0].TextContent.Replace('.', ',')),
                MaxPoints = float.Parse(document.QuerySelectorAll("tbody tr td h4")[1].TextContent),
                Rating = float.Parse(document.QuerySelectorAll("tbody tr td h4")[2].TextContent.Replace("%", "").Replace('.', ',')),
                Grade = int.Parse(document.QuerySelectorAll("tbody tr td h4")[3].TextContent),
            };

            List<Assignment> assignments = new List<Assignment>();

            foreach (var tr in document.QuerySelectorAll("table#simple-table1 tbody tr"))
            {
                var assignment = new Assignment()
                {
                    Teacher = tr.QuerySelectorAll("td")[0].TextContent,
                    TaskName = tr.QuerySelector("td div p").TextContent,
                    Deadline = DateTime.Parse(tr.QuerySelectorAll("td")[2].TextContent, new CultureInfo("ru-RU")),
                    CurrentGrade = tr.QuerySelectorAll("td.text-center div button")[0].TextContent.ParseOrReturnNull(),
                    MaxGrade = float.Parse(tr.QuerySelectorAll("td.text-center div button")[1].TextContent),
                };

                if (!string.IsNullOrWhiteSpace(tr.QuerySelector("td div a").TextContent))
                {
                    assignment.TaskFile = new(
                        fileName: tr.QuerySelector("td div a").TextContent.Trim('\n', ' ', '\t').RemoveFileExtension(),
                        fileUrl: tr.QuerySelector("td div a").GetAttribute("href")
                        );
                }

                if (tr.QuerySelector("td > a") != null)
                {
                    if (tr.QuerySelector("td > a").GetAttribute("href") == "#")
                    {
                        assignment.UploadId = int.Parse(tr.QuerySelector("td > a").GetAttribute("data-id"));
                    }
                    else
                    {
                        assignment.UploadId = (int?)tr.QuerySelector("td div button.js-btn-upload")?.GetAttribute("data-id").ParseOrReturnNull();

                        assignment.UploadedFile = new(
                            fileName: tr.QuerySelector("td > a").TextContent.Trim('\n', ' ', '\t').RemoveFileExtension(),
                            fileUrl: tr.QuerySelector("td > a").GetAttribute("href")
                            );
                    }
                }

                assignment.IsFailed = tr.QuerySelector("td > span") != null;

                assignments.Add(assignment);
            }
            assignmentsPage.Assignments = assignments;

            return assignmentsPage;
        }

        public async Task<Dictionary<int, string>> GetSemesterIdsAsync()
        {
            await _authService.CheckIfNeededReLogin();
            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/student/my-courses");
            Dictionary<int, string> dictionary = new Dictionary<int, string>();

            foreach (var option in document.QuerySelectorAll("select.js-semester option"))
            {
                string semester = option.TextContent.Trim('\n', ' ', '\t');
                dictionary.Add(int.Parse(option.GetAttribute("value")), Regex.Replace(semester, @"\s\s+", " "));
            }

            return dictionary;
        }

        /// <summary>
        /// Get <see cref="Course">Course</see>,
        /// <see cref="Absence">Absence</see>,
        /// <see cref="TableLesson">TableLesson</see> or
        /// <see cref="Final">Final</see> 
        /// </summary>
        /// <param name="semesterId">Semester Id</param>
        /// <returns>LMSObject List</returns>
        public async Task<List<T>> GetLMSObjectsAsync<T>(int semesterId)
        {
            await _authService.CheckIfNeededReLogin();
            string url = typeof(T).Name switch
            {
                "Course" => MyCoursesUrl,
                "Absence" => AbsencesUrl,
                "TableLesson" => ScheduleUrl,
                "Final" => FinalsUrl,
            };

            JObject jObject = JObject.Parse(await _authService.GetStringAsync(url + semesterId));
            List<JToken> results = jObject[typeof(T) == typeof(TableLesson) ? "json" : "data"].Children().ToList();

            List<T> list = new List<T>();

            foreach (var item in results)
            {
                list.Add(item.ToObject<T>());
            }

            return list;
        }

        public async Task<bool> UploadFileAsync(string filePath, int courseId, int uploadId)
        {
            await _authService.CheckIfNeededReLogin();
            var getDocumentAsync = _authService.GetHTMLAsync("https://lms.tuit.uz/student/my-courses/show/" + courseId);

            using var multipartFormContent = new MultipartFormDataContent();

            var fileStreamContent = new StreamContent(File.OpenRead(filePath));

            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

            multipartFormContent.Add(new StringContent(uploadId.ToString()), name: "id");
            multipartFormContent.Add(fileStreamContent, name: "file", fileName: new FileInfo(filePath).Name);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://lms.tuit.uz/student/my-courses/upload")
            {
                Content = multipartFormContent,
            };

            foreach (var pair in uploadRequestHeaders)
            {
                request.Headers.Add(pair.Key, pair.Value);
            }

            string? csrf_token = (await getDocumentAsync).GetElementsByName("csrf-token")[0].GetAttribute("content");
            request.Headers.Add("X-CSRF-TOKEN", csrf_token);

            using var response = await _authService.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UploadFileAsync(Stream stream, string fileName, int courseId, int uploadId)
        {
            await _authService.CheckIfNeededReLogin();
            var getDocumentAsync = _authService.GetHTMLAsync("https://lms.tuit.uz/student/my-courses/show/" + courseId);

            using var multipartFormContent = new MultipartFormDataContent();

            var fileStreamContent = new StreamContent(stream);

            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

            multipartFormContent.Add(new StringContent(uploadId.ToString()), name: "id");
            multipartFormContent.Add(fileStreamContent, name: "file", fileName: fileName);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://lms.tuit.uz/student/my-courses/upload")
            {
                Content = multipartFormContent,
            };

            foreach (var pair in uploadRequestHeaders)
            {
                request.Headers.Add(pair.Key, pair.Value);
            }

            string? csrf_token = (await getDocumentAsync).GetElementsByName("csrf-token")[0].GetAttribute("content");
            request.Headers.Add("X-CSRF-TOKEN", csrf_token);

            using var response = await _authService.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode;
        }

        public async Task<string?> GetAccountFullName()
        {
            await _authService.CheckIfNeededReLogin();
            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/dashboard/news");
            var fullName = document.QuerySelector("ul.dropdown-menu > li > div")?.TextContent.Trim().ToLower();

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fullName);
        }

        public async Task<TableLessonType> GetLessonSideAsync(int semesterId, TableLessonType firstWeekTableLessonSide = TableLessonType.left)
        {
            await _authService.CheckIfNeededReLogin();
            var getCoursesAsync = GetLMSObjectsAsync<Course>(semesterId);

            List<Task<List<Lesson>>> getLessonsAsync = [];

            foreach (var course in await getCoursesAsync)
            {
                getLessonsAsync.Add(GetLessonsAsync(course.Id, LessonType.Lecture));
            }

            var lessons = (await Task.WhenAll(getLessonsAsync)).SelectMany(l => l);

            DateTime firstLessonDate = DateTime.MinValue;

            try
            {
                firstLessonDate = lessons.Select(l => l.LessonDate).Min().ToDateTime(new TimeOnly());
            }
            catch
            {
                return firstWeekTableLessonSide;
            }

            var calendar = new GregorianCalendar();

            return (calendar.GetWeekOfYear(firstLessonDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday) % 2 == calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstDay, DayOfWeek.Monday) % 2)
                ?
                firstWeekTableLessonSide
                :
                (firstWeekTableLessonSide == TableLessonType.left ? TableLessonType.right : TableLessonType.left);
        }

        /// <summary>
        /// Change LMS language
        /// </summary>
        /// <param name="language">use <see cref="Languages">Languages</see> constants</param>
        /// <returns></returns>
        public async Task<bool> ChangeLanguageAsync(string language)
        {
            await _authService.CheckIfNeededReLogin();
            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/profile/language");
            string? _token = document.QuerySelector("input[name=_token]")?.GetAttribute("value");

            var content = new StringContent(string.Format(ChangeLanguageRequestFormat, _token, language));
            content.Headers.ContentType = new("application/x-www-form-urlencoded");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://lms.tuit.uz/profile/language")
            {
                Content = content,
            };

            foreach (var pair in uploadRequestHeaders)
            {
                request.Headers.Add(pair.Key, pair.Value);
            }

            using var response = await _authService.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            await _authService.CheckIfNeededReLogin();
            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/profile/password");
            string? _token = document.QuerySelector("input[name=_token]")?.GetAttribute("value");

            var content = new StringContent(string.Format(ChangePasswordRequestFormat, _token, oldPassword, newPassword, newPassword));
            content.Headers.ContentType = new("application/x-www-form-urlencoded");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://lms.tuit.uz/profile/password")
            {
                Content = content,
            };

            foreach (var pair in uploadRequestHeaders)
            {
                request.Headers.Add(pair.Key, pair.Value);
            }

            using var response = await _authService.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}

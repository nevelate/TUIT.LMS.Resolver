using AngleSharp.Browser;
using AngleSharp.Html.Dom;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TUIT.LMS.API.LMSObjects;

namespace TUIT.LMS.API
{
    public class LMSResolver
    {
        private const string MyCoursesUrl = "https://lms.tuit.uz/student/my-courses/data?semester_id=";
        private const string FinalsUrl = "https://lms.tuit.uz/student/finals/data?semester_id=";
        private const string ScheduleUrl = "https://lms.tuit.uz/student/schedule/load/";
        private const string AbsencesUrl = "https://lms.tuit.uz/student/attendance/data?semester_id=";
        
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
                    {"Referer", "https://lms.tuit.uz/student/my-courses/show/18454" },
                };
        }

        public async Task<Information> GetInformationAsync()
        {
            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/student/info");
            var information = new Information()
            {
                FullName = document.QuerySelectorAll("div.card.relative p.m-b-xs")[0].TextContent.RemoveUpToColonAndTrim(),
                BirthDate = DateOnly.Parse(document.QuerySelectorAll("div.card.relative p.m-b-xs")[1].TextContent.RemoveUpToColonAndTrim(), new CultureInfo("ru-RU")),
                StudentNumber = document.QuerySelectorAll("div.card.relative p.m-b-xs")[3].TextContent.RemoveUpToColonAndTrim(),

                Address = document.QuerySelectorAll("div.card.relative p.m-b-xs")[4].TextContent.RemoveUpToColonAndTrim(),
                TemporaryAddress = document.QuerySelectorAll("div.card.relative p")[6].TextContent.RemoveUpToColonAndTrim(),

                PhotoUrl = document.QuerySelector("div.card.relative p.text-center.m-b-md img")?.GetAttribute("src"),

                Specialization = document.QuerySelectorAll("div.card:not(.relative) p.m-b-xs")[0].TextContent.RemoveUpToColonAndTrim(),
                StudyLanguage = document.QuerySelectorAll("div.card:not(.relative) p.m-b-xs")[1].TextContent.RemoveUpToColonAndTrim(),
                Degree = document.QuerySelectorAll("div.card:not(.relative) p.m-b-xs")[2].TextContent.RemoveUpToColonAndTrim(),
                TypeOfStudy = document.QuerySelectorAll("div.card:not(.relative) p.m-b-xs")[3].TextContent.RemoveUpToColonAndTrim(),
                Year = int.Parse(document.QuerySelectorAll("div.card:not(.relative) p.m-b-xs")[4].TextContent.RemoveUpToColonAndTrim()),
                Group = document.QuerySelectorAll("div.card:not(.relative) p.m-b-xs")[5].TextContent.RemoveUpToColonAndTrim(),
                Tutor = document.QuerySelectorAll("div.card:not(.relative) p")[7].TextContent.RemoveUpToColonAndTrim(),
            };
            return information;
        }

        public async Task<List<News>> GetNewsAsync(int page = 1)
        {
            var newsUrl = page switch
            {
                1 => "https://lms.tuit.uz/dashboard/news",
                _ => "https://lms.tuit.uz/dashboard/news?page=" + page,
            };

            var document = await _authService.GetHTMLAsync(newsUrl);
            List<News> news = new List<News>();

            foreach (var column in document.QuerySelectorAll("div.row div.col-md-4 div.card.p"))
            {
                var url = column.QuerySelector("div.d-flex a")?.GetAttribute("href");
                var newsPage = await _authService.GetHTMLAsync(url);
                var description = newsPage.QuerySelector("div.panel-body blockquote div")?.TextContent;
                news.Add(new()
                {
                    Title = column.QuerySelector("div.panel-body p")?.TextContent,
                    Description = description,
                    NewsDate = DateOnly.Parse(column.QuerySelector("div.d-flex p").TextContent.Replace(':', '.'))
                }
                );
            }
            return news;
        }

        public async Task<List<Discipline>> GetDisciplinesAsync()
        {
            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/student/study-plan");

            List<Discipline> disciplines = new List<Discipline>();

            foreach (var row in document.QuerySelectorAll("div.page-inner > div.row"))
            {
                foreach (var card in document.QuerySelectorAll("div.col-lg-6 > div.card"))
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

        public async Task<List<Lesson>> GetLessonsAsync(int courseId, bool isLecture)
        {
            var dateRegex = new Regex(@"\(.*\)");

            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/student/calendar/" + courseId);
            List<Lesson> lessons = new List<Lesson>();

            foreach (var tr in document.QuerySelectorAll(isLecture ? "div#lecture tbody > tr" : "div#practise tbody > tr"))
            {
                var lesson = new Lesson
                {
                    ThemeTitle = tr.QuerySelector("td p").TextContent,
                    ThemeNumber = int.Parse(tr.QuerySelectorAll("td")[0].TextContent),
                    LessonDate = DateOnly.Parse(dateRegex.Replace(tr.QuerySelectorAll("td")[2].TextContent, (m) => string.Empty)),
                    IsLecture = isLecture,
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

        public async Task<AssignmentsPage> GetAssignmentsPageAsync(int courseId)
        {
            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/student/my-courses/show/" + courseId);

            AssignmentsPage assignmentsPage = new AssignmentsPage()
            {
                AchievedPoints = int.Parse(document.QuerySelectorAll("tbody tr td h4")[0].TextContent),
                MaxPoints = int.Parse(document.QuerySelectorAll("tbody tr td h4")[1].TextContent),
                Rating = int.Parse(document.QuerySelectorAll("tbody tr td h4")[2].TextContent.Replace("%", "")),
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
                    MaxGrade = int.Parse(tr.QuerySelectorAll("td.text-center div button")[1].TextContent),
                };

                assignment.TaskFile = new(
                    fileName: tr.QuerySelector("td div a").TextContent.Trim('\n', ' ', '\t'),
                    fileUrl: tr.QuerySelector("td div a").GetAttribute("href")
                    );

                if (tr.QuerySelector("td > a") != null)
                {
                    if (tr.QuerySelector("td > a").GetAttribute("href") == "#")
                    {
                        assignment.UploadId = int.Parse(tr.QuerySelector("td > a").GetAttribute("data-id"));
                    }
                    else
                    {
                        assignment.UploadId = tr.QuerySelector("td div button.js-btn-upload")?.GetAttribute("data-id").ParseOrReturnNull();

                        assignment.UploadedFile = new(
                            fileName: tr.QuerySelector("td > a").TextContent.Trim('\n', ' ', '\t'),
                            fileUrl: tr.QuerySelector("td > a").GetAttribute("href")
                            );
                    }
                }

                assignments.Add(assignment);
            }
            assignmentsPage.Assignments = assignments;

            return assignmentsPage;
        }

        public async Task<Dictionary<int, string>> GetSemesterIdsAsync()
        {
            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/student/my-courses");
            Dictionary<int, string> dictionary = new Dictionary<int, string>();

            foreach (var option in document.QuerySelectorAll("select.js-semester option"))
            {
                dictionary.Add(int.Parse(option.GetAttribute("value")), option.TextContent.Trim('\n', ' ', '\t'));
            }

            return dictionary;
        }

        /// <summary>
        /// Get Courses, Absences, TableLessons, Finals 
        /// </summary>
        /// <param name="url">Constants of this class</param>
        /// <param name="semesterId"></param>
        /// <returns></returns>
        public async Task<List<T>> GetLMSObjectsAsync<T>(int semesterId)
        {
            string url = typeof(T).Name switch
            {
                "Course" => MyCoursesUrl,
                "Absence" => AbsencesUrl,
                "TableLesson" => ScheduleUrl,
                "Final" => FinalsUrl,
            };

            string data = await _authService.GetStringAsync(url + semesterId);

            JObject jObject = JObject.Parse(data);
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
            var document = await _authService.GetHTMLAsync("https://lms.tuit.uz/student/my-courses/show/" + courseId);

            string csrf_token = document.GetElementsByName("csrf-token")[0].GetAttribute("content");

            using var multipartFormContent = new MultipartFormDataContent();

            var fileStreamContent = new StreamContent(File.OpenRead(filePath));
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

            multipartFormContent.Add(new StringContent(uploadId.ToString()), name: "id");
            multipartFormContent.Add(fileStreamContent, name: "file", fileName: "M.pdf");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://lms.tuit.uz/student/my-courses/upload")
            {
                Content = multipartFormContent,
            };

            foreach (var pair in uploadRequestHeaders)
            {
                request.Headers.Add(pair.Key, pair.Value);
            }

            request.Headers.Add("X-CSRF-TOKEN", csrf_token);

            using var response = await _authService.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();

            return responseText.Contains("true");
        }
    }
}

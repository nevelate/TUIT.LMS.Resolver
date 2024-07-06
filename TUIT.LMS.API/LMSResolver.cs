using AngleSharp.Browser;
using AngleSharp.Html.Dom;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TUIT.LMS.API.LMSObjects;

namespace TUIT.LMS.API
{
    public class LMSResolver
    {
        public const string MyCoursesUrl = "https://lms.tuit.uz/student/my-courses/data?semester_id=";
        public const string FinalsUrl = "https://lms.tuit.uz/student/finals/data?semester_id=";
        public const string ScheduleUrl = "https://lms.tuit.uz/student/schedule/load/";
        public const string AbsencesUrl = "https://lms.tuit.uz/student/attendance/data?semester_id=";

        private HttpClient _httpClient;
        private LMSAuthService _authService;

        public LMSResolver(LMSAuthService authService)
        {
            _authService = authService;
            _httpClient = authService.HttpClient;
        }

        public async Task<Information> GetInformationAsync()
        {
            var document = await _httpClient.GetHTMLAsync("https://lms.tuit.uz/student/info");
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

        public async Task<List<News>> GetAllNewsAsync()
        {
            var document = await _httpClient.GetHTMLAsync("https://lms.tuit.uz/dashboard/news");
            List<News> news = new List<News>();

            foreach (var column in document.QuerySelectorAll("div.row div.col-md-4 div.card.p"))
            {
                var url = column.QuerySelector("div.d-flex a")?.GetAttribute("href");
                var newsPage = await _httpClient.GetHTMLAsync(url);
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
            var document = await _httpClient.GetHTMLAsync("https://lms.tuit.uz/student/study-plan");

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
            var document = await _httpClient.GetHTMLAsync("https://lms.tuit.uz/student/calendar/" + courseId);
            List<Lesson> lessons = new List<Lesson>();

            foreach (var lesson in document.QuerySelectorAll(isLecture ? "div#lecture tbody > tr" : "div#practise tbody > tr"))
            {
                lessons.Add(new()
                {
                    ThemeTitle = lesson.QuerySelector("td p").TextContent,
                    ThemeNumber = int.Parse(lesson.QuerySelectorAll("td")[0].TextContent),
                    LessonDate = DateOnly.Parse(lesson.QuerySelectorAll("td")[2].TextContent),
                    IsLecture = isLecture,
                    AttachmentsUrl = lesson.QuerySelectorAll("td a").Select(a => a.GetAttribute("href")).ToList(),
                });
            }

            return lessons;
        }

        public async Task<AssignmentsPage> GetAssignmentsPageAsync(int courseId)
        {
            var document = await _httpClient.GetHTMLAsync("https://lms.tuit.uz/student/my-courses/show/" + courseId);

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
                assignments.Add(new Assignment()
                {
                    Teacher = tr.QuerySelectorAll("td")[0].TextContent,
                    TaskName = tr.QuerySelector("td div p").TextContent,
                    TaskUrl = tr.QuerySelector("td div a").GetAttribute("href"),
                    Deadline = DateTime.Parse(tr.QuerySelectorAll("td")[2].TextContent, new CultureInfo("ru-RU")),
                    CurrentGrade = int.Parse(tr.QuerySelectorAll("td.text-center div button")[0].TextContent),
                    MaxGrade = int.Parse(tr.QuerySelectorAll("td.text-center div button")[1].TextContent),
                    UploadedFileUrl = tr.QuerySelector("td a").GetAttribute("href"),
                });
            }
            assignmentsPage.Assignments = assignments;

            return assignmentsPage;
        }

        public async Task<Dictionary<int, string>> GetSemesterIdsAsync()
        {
            var document = await _httpClient.GetHTMLAsync("https://lms.tuit.uz/student/my-courses");
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
        public async Task<List<T>> GetLMSObjectsAsync<T>(string url, int semesterId)
        {
            string data = await _httpClient.GetStringAsync(url + semesterId);

            JObject jObject = JObject.Parse(data);
            List<JToken> results = jObject[typeof(T) == typeof(TableLesson) ? "json" : "data"].Children().ToList();

            List<T> list = new List<T>();

            foreach (var item in results)
            {
                list.Add(item.ToObject<T>());
            }

            return list;
        }
    }
}

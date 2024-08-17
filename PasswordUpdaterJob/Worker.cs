using System.Text.Json;
using System.Text;
using PasswordUpdaterJob.Models;
using Newtonsoft.Json;
using PasswordUpdaterJob.Services.Interfaces;

namespace PasswordUpdaterJob
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IUser _user;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, HttpClient httpClient, IUser user)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
            _user = user;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Background job running at: {time}", DateTimeOffset.Now);

                await ProcessUsers();

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Run daily
            }
        }
        private async Task ProcessUsers()
        {
            _logger.LogInformation("-------------Start Worker/ProcessUsers---------");
            List<UsersModel> users = await _user.FetchUsersWithOutdatedPasswords();
            _logger.LogInformation("-------------Count User: {a}---------", users.Count().ToString());
            foreach (UsersModel user in users)
            {
                await _user.UpdateStatus(user.Id, "REQUIRE_CHANGE_PWD");
                await SendEmailNotification(user.Email);
            }
        }
        private async Task SendEmailNotification(string userEmail)
        {
            _logger.LogInformation("-------------Start Worker/SendEmailNotification---------");
            MailModel mail = new MailModel();
            mail.FromEmail = _configuration["EmailApiSettings:SenderEmail"];
            mail.Recipients = userEmail;
            mail.CarbonCopys = "";
            mail.Subject = "Password Change Required";
            mail.Body = "Your password has not been updated in the last six months. Please change your password.";

            var httpContent = new StringContent(JsonConvert.SerializeObject(mail), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _configuration["EmailApiSettings:ApiKey"]);

            HttpResponseMessage response = await _httpClient.PostAsync(_configuration["EmailApiSettings:BaseUrl"], httpContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to send email to {userEmail}. Status Code: {response.StatusCode}");
            }
        }
    }
}
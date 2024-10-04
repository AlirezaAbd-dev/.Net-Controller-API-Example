namespace Cityinfo.API.Services {
    public class LocalMailService : IMailService {
        private readonly string _mailTo = string.Empty;
        private readonly string _mailFrom = string.Empty;

        public LocalMailService(IConfiguration config)
        {
            _mailTo = config["mailSetting:mailTo"]!;
            _mailFrom = config["mailSetting:mailFrom"]!;
        }

        public void Send(string subject, string message) {
            Console.WriteLine($"Mail From {_mailFrom} To  {_mailTo}  ,  " +
                $"with {nameof(LocalMailService)}  ,  "
                );
            Console.WriteLine($"subject {subject}");
            Console.WriteLine($"message  {message}");
        }
    }
}

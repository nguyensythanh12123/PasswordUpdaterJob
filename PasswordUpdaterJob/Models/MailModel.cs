using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordUpdaterJob.Models
{
    public class MailModel
    {
        public string FromEmail { get; set; }
        public string Recipients { get; set; }
        public string CarbonCopys { get; set; }
        public string BlindCarbonCopys { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}

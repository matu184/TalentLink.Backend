using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink.Domain.Entities
{
    public class StripeSettings
    {
        public string SecretKey { get; set; } = null!;
        public string WebhookSecret { get; set; } = null!;
    }

}

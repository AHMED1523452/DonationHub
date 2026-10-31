using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public class IdempotencyRecord
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Key { get; set; }
        public string UserId { get; set; }
        public string RequestHash { get; set; }
        public string ResponseBody { get; set; }
        public string ResponseStatusCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

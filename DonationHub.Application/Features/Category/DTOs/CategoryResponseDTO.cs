using System;

namespace DonationHub.Application.Features.Category.DTOs
{
    public class CategoryResponseDTO
    {
        public Guid Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int CampaignsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

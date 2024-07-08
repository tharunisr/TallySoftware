﻿using System.ComponentModel.DataAnnotations;
using TallySoftware.Entity;

namespace TallySoftware.DTO
{
    public class EditEnquiryDto
        {
            public int Id { get; set; }
            
            public string? CustomerName { get; set; }
            public List<string>? CustomerNameList { get; set; }
            public List<string>? RecruitmentList { get; set; }
            
            public string? RecruitmentName { get; set; }
            [Required]
            public string Status { get; set; }
            [Required]
            public string Remark { get; set; }
            [Required]
            [DataType(DataType.Date)]
            public DateTime Schedule { get; set; }
            [Required]
            public string Payment { get; set; }
            public Customer? Customer { get; set; }
            public string Resource { get; set; }

        }
    }
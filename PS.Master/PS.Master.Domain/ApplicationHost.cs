﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PS.Master.Domain
{
    [Table("tblApplicationHosts")]
    public class ApplicationHost
    {
        [Key]
        public int AppId { get; set; }
        public string AppName { get; set; }
        public string? AppLogo { get; set; }
        public string AppRootPath { get; set; }
        public string AppVPath { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

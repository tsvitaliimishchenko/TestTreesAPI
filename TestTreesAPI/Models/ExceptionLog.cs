using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TestTreesAPI.Models
{
    public class ExceptionLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid EventId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public string Type { get; set; }

        public ExceptionLogData Data { get; set; }
    }
    public record ExceptionLogData
    {
        public string QueryParameters { get; set; }

        public string BodyParameters { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }
    }
}
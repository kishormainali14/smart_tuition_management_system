using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuitionManagementSystem.Entities;

[Table("Attendance")]  // Note: Your table is named "Attendance" (singular)
public class AttendanceEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int StudentId { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime AttendanceDate { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Present"; // ✅ FIXED: Added default value
    
    [MaxLength(200)]
    public string Remarks { get; set; } = string.Empty;
    
    public DateTime MarkedAt { get; set; } = DateTime.UtcNow;
    
    public int? MarkedBy { get; set; } // UserId who marked
    
    [ForeignKey("StudentId")]
    public virtual StudentEntity Student { get; set; } = new StudentEntity(); // ✅ FIXED: Initialized
}
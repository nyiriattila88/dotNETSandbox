using System.ComponentModel.DataAnnotations;

namespace LeaveRequestManager.Common.Models;

public record LeaveTypeVM
{
    public int Id { get; set; }

    [Display(Name = "Leave Type Name")]
    [Required]
    public string Name { get; set; }

    [Display(Name = "Default Number Of Days")]
    [Required]
    [Range(1, 25, ErrorMessage = "Please Enter A Valid Number")]
    public int DefaultDays { get; set; }
}


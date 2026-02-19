using System.ComponentModel.DataAnnotations;

namespace MCPServerDemo.Models;

public class SumRequest
{
    [Required]
    public int A { get; set; }

    [Required]
    public int B { get; set; }
}
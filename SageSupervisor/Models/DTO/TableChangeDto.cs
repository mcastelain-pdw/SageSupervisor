using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SageSupervisor.Models.DTO;

public class TableChangeDto()
{
    public int Id { get; set; }

    [Required]
    public string NumPiece { get; set; } = "";

    [Required]
    public TableChangeType ChangeType { get; set; }

    [Required]
    public DateTime UpdatedDate { get; set; }

    [DefaultValue(0)]
    public int Domaine { get; set; }

    [DefaultValue(0)]
    public int Type { get; set; }
}

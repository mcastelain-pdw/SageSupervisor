using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SageSupervisor.Models.DTO;

public class TiersChangeDto
{
    public int Id { get; set; }

    [Required]
    public string NumTiers { get; set; } = "";

    [Required]
    public TableChangeType ChangeType { get; set; }

    [Required]
    public DateTime UpdatedDate { get; set; }

    [DefaultValue(0)]
    public TiersTypeEnum Type { get; set; }
}

public enum TiersTypeEnum
{
    Client = 0,
    Fournisseur = 1,
    Salarie = 2,
    autres = 3
}
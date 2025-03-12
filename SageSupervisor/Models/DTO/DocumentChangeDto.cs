using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SageSupervisor.Models.DTO;

public class DocumentChangeDto()
{
    public int Id { get; set; }

    [Required]
    public string NumPiece { get; set; } = "";

    [Required]
    public TableChangeType ChangeType { get; set; }

    [Required]
    public DateTime UpdatedDate { get; set; }

    [Required]
    [DefaultValue(0)]
    public decimal TotalHT { get; set; }

    [DefaultValue(0)]
    public DocDomaineEnum Domaine { get; set; }

    [DefaultValue(0)]
    public DocTypeEnum Type { get; set; }
}

public enum DocDomaineEnum
{
    Vente = 0,
    Achat = 1,
    Stock = 2,
    Ticket = 3,
    DocumentInterne = 4
}

public enum DocTypeEnum
{
    V_Devis = 0,
    V_Bon_Commande = 1,
    V_Preparation_Livraison = 2,
    V_Bon_Livraison = 3,
    V_Bon_Retour = 4,
    V_Bon_Avoir = 5,
    V_Facture = 6,
    V_Facture_Comptabilise = 7,
    V_Archive = 8,

    A_Demande_Achat = 10,
    A_Preparation_Commande = 11,
    A_Bon_Commande = 12,
    A_Bon_Livraison = 13,
    A_Bon_Retour = 14,
    A_Bon_Avoir = 15,
    A_Facture = 16,
    A_Facture_Comptabilise = 17,
    A_Archive = 18,

    S_Mouvement_Entree = 20,
    S_Mouvement_Sortie = 21,
    S_Depreciation_Stock = 22,
    S_Virement_Depot = 23,
    S_Preparation_Fabrication = 24,
    S_Ordre_Fabrication = 25,
    S_Bon_Fabrication = 26,
    S_Archive = 27,

    T_Ticket = 30,

    D_Document_1 = 40,
    D_Document_2 = 41,
    D_Document_3 = 42,
    D_Document_4 = 43,
    D_Document_5 = 44,
    D_Document_6 = 45,
    D_Saisie_Realise = 46,
    D_Archive = 47
}
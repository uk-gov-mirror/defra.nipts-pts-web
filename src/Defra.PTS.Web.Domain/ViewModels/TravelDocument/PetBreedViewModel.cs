using Defra.PTS.Web.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Defra.PTS.Web.Domain.ViewModels.TravelDocument;

public class PetBreedViewModel : TravelDocumentFormPage
{
    public string FormTitle => $"What breed is your {PetTypeNameLowered}?";

    public PetSpecies PetSpecies { get; set; }

    public string BreedName { get; set; }    

    public int BreedId { get; set; }
    public string BreedAdditionalInfo { get; set; }

    public string PetTypeNameLowered => Enum.GetName(PetSpecies).ToLower();

    public override Enums.TravelDocumentFormPageType PageType => Enums.TravelDocumentFormPageType.PetBreed;

    public override void TrimUnwantedData()
    {
    }

    public override void ClearData()
    {
        IsCompleted = default;
        PetSpecies = default;
        BreedName = default;
        BreedId = default;
    }
}

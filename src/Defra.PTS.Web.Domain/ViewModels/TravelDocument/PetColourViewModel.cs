using Defra.PTS.Web.Domain.DTOs;
using Defra.PTS.Web.Domain.Enums;

namespace Defra.PTS.Web.Domain.ViewModels.TravelDocument;

public class PetColourViewModel : TravelDocumentFormPage
{
    public string FormTitle => $"What is the main colour of your {PetTypeNameLowered}?";

    public PetSpecies PetSpecies { get; set; }

    public int PetColour { get; set; }

    public string PetColourName { get; set; }

    public string PetColourOther { get; set; }

    public int OtherColourID { get; set; }    

    public string PetTypeNameLowered => Enum.GetName(PetSpecies).ToLower();

    public override Enums.TravelDocumentFormPageType PageType => Enums.TravelDocumentFormPageType.PetColour;


    public override void TrimUnwantedData()
    {
        if (PetColour != OtherColourID)
        {
            PetColourOther = string.Empty;
        }
    }

    public override void ClearData()
    {
        IsCompleted = default;
        PetSpecies = default;
        PetColour = default;
        PetColourOther = default;
    }
}

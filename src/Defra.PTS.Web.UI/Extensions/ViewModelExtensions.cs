using Defra.PTS.Web.Application.Extensions;
using Defra.PTS.Web.Domain.Enums;
using Defra.PTS.Web.Domain.ViewModels.TravelDocument;

namespace Defra.PTS.Web.UI.Extensions;

public static class ViewModelExtensions
{
    public static bool DoesPageMeetPreConditions(this TravelDocumentViewModel vm, TravelDocumentFormPageType formPage, out string actionName)
    {
        actionName = string.Empty;

        var keeperResult = CheckKeeperPreConditions(vm, formPage, ref actionName);
        if (keeperResult.HasValue)
        {
            return keeperResult.Value;
        }

        return CheckPetDetailPreConditions(vm, formPage, ref actionName);
    }

    // Returns true/false when a keeper page resolves the outcome, or null to continue to the pet detail pages.
    private static bool? CheckKeeperPreConditions(TravelDocumentViewModel vm, TravelDocumentFormPageType formPage, ref string actionName)
    {
        // PetKeeperUserDetails
        if (formPage == TravelDocumentFormPageType.PetKeeperUserDetails)
        {
            return true;
        }

        if (!vm.PetKeeperUserDetails.IsCompleted)
        {
            actionName = "PetKeeperUserDetails";
            return false;
        }

        // PetKeeperName
        if (formPage == TravelDocumentFormPageType.PetKeeperName)
        {
            return true;
        }

        if (!vm.PetKeeperName.IsCompleted && vm.PetKeeperUserDetails.PetOwnerDetailsRequired)
        {
            actionName = "PetKeeperName";
            return false;
        }

        var addressResult = CheckAddressPreConditions(vm, formPage, ref actionName);
        if (addressResult.HasValue)
        {
            return addressResult.Value;
        }

        // PetKeeperPhone
        if (formPage == TravelDocumentFormPageType.PetKeeperPhone)
        {
            return true;
        }

        if (!vm.PetKeeperPhone.IsCompleted && vm.PetKeeperUserDetails.PetOwnerDetailsRequired)
        {
            actionName = "PetKeeperPhone";
            return false;
        }

        return null;
    }

    // Returns true/false when an address page resolves the outcome, or null to continue.
    private static bool? CheckAddressPreConditions(TravelDocumentViewModel vm, TravelDocumentFormPageType formPage, ref string actionName)
    {
        var addressPages = new List<TravelDocumentFormPageType>
        {
            TravelDocumentFormPageType.PetKeeperPostcode,
            TravelDocumentFormPageType.PetKeeperAddress,
            TravelDocumentFormPageType.PetKeeperAddressManual
        };

        if (addressPages.Contains(formPage))
        {
            if (!vm.PetKeeperUserDetails.PetOwnerDetailsRequired)
            {
                return true;
            }

            if (formPage == TravelDocumentFormPageType.PetKeeperAddress && !vm.PetKeeperPostcode.IsCompleted)
            {
                actionName = "PetKeeperPostcode";
                return false;
            }

            return true;
        }

        // Select Address or Manual Address must be completed before continuing to later pages.
        if (vm.PetKeeperUserDetails.PetOwnerDetailsRequired)
        {
            var addressCompleted = vm.PetKeeperPostcode.IsCompleted && vm.PetKeeperAddress.IsCompleted;
            var manualAddressCompleted = vm.PetKeeperAddressManual.IsCompleted;
            if (!(addressCompleted || manualAddressCompleted))
            {
                actionName = "PetKeeperPostcode";
                return false;
            }
        }

        return null;
    }

    private static bool CheckPetDetailPreConditions(TravelDocumentViewModel vm, TravelDocumentFormPageType formPage, ref string actionName)
    {
        var gates = new List<(TravelDocumentFormPageType Page, bool Completed, string Action)>
        {
            (TravelDocumentFormPageType.PetMicrochip, vm.PetMicrochip.IsCompleted, "PetMicrochip"),
            (TravelDocumentFormPageType.PetMicrochipNotAvailable, true, string.Empty),
            (TravelDocumentFormPageType.PetMicrochipDate, vm.PetMicrochipDate.IsCompleted, "PetMicrochipDate"),
            (TravelDocumentFormPageType.PetSpecies, vm.PetSpecies.IsCompleted, "PetSpecies"),
        };

        if (vm.PetSpecies.PetSpecies.HasBreed())
        {
            gates.Add((TravelDocumentFormPageType.PetBreed, vm.PetBreed.IsCompleted, "PetBreed"));
        }

        gates.Add((TravelDocumentFormPageType.PetName, vm.PetName.IsCompleted, "PetName"));
        gates.Add((TravelDocumentFormPageType.PetGender, vm.PetGender.IsCompleted, "PetGender"));
        gates.Add((TravelDocumentFormPageType.PetAge, vm.PetAge.IsCompleted, "PetAge"));
        gates.Add((TravelDocumentFormPageType.PetColour, vm.PetColour.IsCompleted, "PetColour"));
        gates.Add((TravelDocumentFormPageType.PetFeature, vm.PetFeature.IsCompleted, "PetFeature"));
        gates.Add((TravelDocumentFormPageType.Declaration, vm.Declaration.IsCompleted, "Declaration"));

        foreach (var gate in gates)
        {
            if (formPage == gate.Page)
            {
                return true;
            }

            if (!gate.Completed)
            {
                actionName = gate.Action;
                return false;
            }
        }

        return true;
    }
}

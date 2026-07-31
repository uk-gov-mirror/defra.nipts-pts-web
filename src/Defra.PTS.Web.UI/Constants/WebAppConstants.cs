namespace Defra.PTS.Web.UI.Constants;

public static class WebAppConstants
{
    public const string HistoryBack = "javascript: history.back();";
    public const string ServiceName = "Taking a pet from Great Britain to Northern Ireland";

    public static class ViewKeys
    {
        public const string Title = "Title";
        public const string HideNeedHelp = "HideNeedHelp";
        public const string BackUrl = "BackUrl";
    }

    public static class IdentityKeys
    {
        public const string PTSUserId = "PTSUserId";
    }

    public static class Pages
    {
        public static class TravelDocument
        {
            public const string Index = "/TravelDocument";

            public const string InvalidDocuments = "/TravelDocument/InvalidDocuments";

            public const string PetKeeperUserDetails = "/TravelDocument/PetKeeperUserDetails";
            public const string PetKeeperNonGbAddress = "/TravelDocument/PetKeeperNonGbAddress";
            public const string PetKeeperName = "/TravelDocument/PetKeeperName";
            public const string PetKeeperPostcode = "/TravelDocument/PetKeeperPostcode";
            public const string PetKeeperAddress = "/TravelDocument/PetKeeperAddress";
            public const string PetKeeperAddressManual = "/TravelDocument/PetKeeperAddressManual";
            public const string PetKeeperPhone = "/TravelDocument/PetKeeperPhone";

            public const string PetMicrochip = "/TravelDocument/PetMicrochip";
            public const string PetMicrochipNotAvailable = "/TravelDocument/PetMicrochipNotAvailable";
            public const string PetMicrochipDate = "/TravelDocument/PetMicrochipDate";
            public const string PetSpecies = "/TravelDocument/PetSpecies";
            public const string PetBreed = "/TravelDocument/PetBreed";
            public const string PetName = "/TravelDocument/PetName";
            public const string PetGender = "/TravelDocument/PetGender";
            public const string PetAge = "/TravelDocument/PetAge";
            public const string PetColour = "/TravelDocument/PetColour";
            public const string PetFeature = "/TravelDocument/PetFeature";
            public const string Declaration = "/TravelDocument/Declaration";
            public const string Acknowledgement = "/TravelDocument/Acknowledgement";

            public const string ApplicationDetails = "/TravelDocument/ApplicationDetails";
            public const string ApplicationCertificate = "/TravelDocument/ApplicationCertificate";
        }

        public static class Content
        {
            public const string AccessibilityStatement = "/Content/AccessibilityStatement";
            public const string Cookies = "/Content/Cookies";
            public const string TermsAndConditions = "/Content/TermsAndConditions";
            public const string Help = "/Content/Help";
        }

        public static class User
        {
            public const string ManageAccount = "/User/ManageAccount";
            public const string SignOut = "/User/OSignOut";
            public const string AccessTokenKey = "access_token";
        }
    }

    public static class ExternalLinks
    {
        public const string GovUK = "https://www.gov.uk/";
        public const string BetaServiceFeedback = "https://defragroup.eu.qualtrics.com/jfe/form/SV_b95IAit5HqB53Wm#";
        public const string PrivacyNotice = "https://www.gov.uk/government/publications/animal-and-plant-health-agency-privacy-notices/northern-ireland-pet-travel-scheme-privacy-notice";
        public const string OpenGovernmentLicence = "https://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/";
        public const string CrownCopyright = "https://www.nationalarchives.gov.uk/information-management/re-using-public-sector-information/uk-government-licensing-framework/crown-copyright/";
    }

    public static class PartialViews
    {
        // Shared
        public const string Header = "~/Views/Shared/_HeaderPartial.cshtml";
        public const string BetaPanel = "~/Views/Shared/_BetaPanelPartial.cshtml";
        public const string Navigation = "~/Views/Shared/_NavigationPartial.cshtml";
        public const string LanguageSelect = "~/Views/Shared/_LanguageSelect.cshtml";
        public const string Footer = "~/Views/Shared/_FooterPartial.cshtml";
        public const string NeedHelp = "~/Views/Shared/_NeedHelp.cshtml";
        public const string BackLink = "~/Views/Shared/_BackLink.cshtml";

        public static class TravelDocument
        {
            public const string ApplicationsList = "~/Views/TravelDocument/Partials/_ApplicationsList.cshtml";

            public const string ApplicationDetails_Issued = "~/Views/TravelDocument/Partials/_ApplicationDetails_Issued.cshtml";
            public const string ApplicationDetails_IssuingAuthority = "~/Views/TravelDocument/Partials/_ApplicationDetails_IssuingAuthority.cshtml";
            public const string ApplicationDetails_MicrochipInfo = "~/Views/TravelDocument/Partials/_ApplicationDetails_MicrochipInfo.cshtml";
            public const string ApplicationDetails_PetDetails = "~/Views/TravelDocument/Partials/_ApplicationDetails_PetDetails.cshtml";
            public const string ApplicationDetails_PetKeeperDetails = "~/Views/TravelDocument/Partials/_ApplicationDetails_PetKeeperDetails.cshtml";
            public const string ApplicationDetails_Declaration = "~/Views/TravelDocument/Partials/_ApplicationDetails_Declaration.cshtml";
            public const string ApplicationDetails_PrintLogos = "~/Views/TravelDocument/Partials/_ApplicationDetails_PrintLogos.cshtml";
            public const string ApplicationDetails_ActionLinks = "~/Views/TravelDocument/Partials/_ApplicationDetails_ActionLinks.cshtml";
            public const string ApplicationDetails_PreLoadAssets = "~/Views/TravelDocument/Partials/_ApplicationDetails_PreLoadAssets.cshtml";

            
        }
    }

    public static class Views
    {
        public const string AccessDenied = "~/Views/Error/AccessDenied.cshtml";
        public const string PageNotFound = "~/Views/Error/PageNotFound.cshtml";
        public const string Forbidden = "~/Views/Error/Forbidden.cshtml";
        public const string InternalServer = "~/Views/Error/InternalServer.cshtml";
        public const string HandleError = "~/Views/Error/HandleError.cshtml";
    }

    public static class TempDataKey
    {
        /// <summary>
        /// A key name used to store the travel document view model between requests
        /// </summary>
        public const string TravelDocumentViewModel = "TravelDocumentViewModel";

        /// <summary>
        /// A key name used to store the if the magic word has been entered by the user
        /// </summary>
        public const string HasUserUsedMagicWord = "HasUserUsedMagicWord";

        /// <summary>
        /// A key name used to store the if the has been submitted
        /// </summary>
        public const string FormSubmissionQueue = "FormSubmissionQueue";
        
        /// <summary>
        /// A key name used to store application reference
        /// </summary>
        public const string ApplicationReference = "ApplicationReference";

        /// <summary>
        /// A key name used to store if the user has been suspended
        /// </summary>
        public const string IsUserSuspended = "IsUserSuspended";

    }
    public static class FormAutofill
    {
        // see https://www.w3.org/TR/WCAG21/#input-purposes

        // Full name
        public const string FullName = "name";

        // Given name (in some Western cultures, also known as the first name)
        public const string FirstName = "given-name";

        // Additional names (in some Western cultures, also known as middle names, forenames other than the first name)
        public const string MiddleName = "additional-name";

        // Family name (in some Western cultures, also known as the last name or surname)
        public const string LastName = "family-name";

        // Company name corresponding to the person, address, or contact information in the other fields associated with this field
        public const string Organization = "organization ";

        // Street address (multiple lines, newlines preserved)
        public const string StreetAddress = "street-address";

        // Street address (one line per field, line 1)
        public const string AddressLineOne = "address-line1";

        // Street address (one line per field, line 2)
        public const string AddressLineTwo = "address-line2";

        //Street address (one line per field, line 3)
        public const string AddressLineThree = "address-line3";

        // The second administrative level, in addresses with two or more administrative levels; in the countries with two administrative levels, this would typically be the city, town, village, or other locality within which the relevant street address is found
        public const string CityOrTown = "address-level2";

        // Postal code, post code, ZIP code, CEDEX code (if CEDEX, append "CEDEX", and the dissement, if relevant, to the address-level2 field)
        public const string PostalCode = "postal-code";

        // Full telephone number, including country code
        public const string TelephoneIntl = "tel";

        // Telephone number without the country code component, with a country-internal prefix applied if applicable
        public const string Telephone = "tel-national";

        // E-mail address
        public const string Email = "email";
    }

    /// <summary>
    /// Places post code
    /// </summary>
    public static class PlacesPostCode
    {
        public const string PostCode = "JE2 6SF";
    }
}

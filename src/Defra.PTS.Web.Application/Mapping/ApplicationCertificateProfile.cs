using AutoMapper;
using Defra.PTS.Web.Application.Constants;
using Defra.PTS.Web.Application.DTOs.Services;
using Defra.PTS.Web.Application.Mapping.Converters;
using Defra.PTS.Web.Domain.Enums;

namespace Defra.PTS.Web.Application.Mapping;

public class ApplicationCertificateProfile : Profile
{
    public ApplicationCertificateProfile()
    {
CreateMap<VwApplication, ApplicationCertificateDto>()
    .ForMember(dest => dest.Status, opt => opt.ConvertUsing(new ConvertDisplayStatus(), src => src.Status))
    .ForMember(dest => dest.MicrochipInformation, opt => opt.MapFrom(src => MappingConverter.MapMicrochipInformation(src)))
    .ForMember(dest => dest.PetDetails, opt => opt.MapFrom(src => MappingConverter.MapPetDetails(src)))
    .ForMember(dest => dest.PetKeeperDetails, opt => opt.MapFrom(src => MappingConverter.MapPetKeeperDetails(src)))
    .ForMember(dest => dest.Declaration, opt => opt.MapFrom(src => MappingConverter.MapDeclaration(src)))
    .ForMember(dest => dest.CertificateIssued, opt => opt.MapFrom(src => MappingConverter.MapCertificateIssued(src)))
    .ForMember(dest => dest.CertificateIssuingAuthority, opt => opt.MapFrom(src => MappingConverter.MapCertificateIssuingAuthority(src)))
    .ForMember(dest => dest.ActionLinks, opt => opt.MapFrom(src =>
    MappingConverter.MapActionLinks(
    src.ApplicationId,
    PdfType.Certificate,
    src.DocumentReferenceNumber,
            true)))
    .AfterMap<SetApplicationCertificateAction>();

    }
}

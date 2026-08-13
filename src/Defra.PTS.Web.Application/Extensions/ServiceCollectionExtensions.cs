using Defra.PTS.Web.Application.Features.Pipelines;
using Defra.PTS.Web.Application.Features.TravelDocument.Commands;
using Defra.PTS.Web.Application.Services;
using Defra.PTS.Web.Application.Services.Interfaces;
using Defra.PTS.Web.Application.Validation;
using Defra.PTS.Web.QRCoder.Services;
using Defra.PTS.Web.QRCoder.Services.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Web.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper((Action<AutoMapper.IMapperConfigurationExpression>)null, typeof(Defra.PTS.Web.Application.Mapping.ApplicationCertificateProfile));

        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDynamicService, DynamicService>();
        services.AddValidatorsFromAssemblyContaining<TravelDocumentValidator>();
        services.AddScoped<IValidationService, ValidationService>();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CreateTravelDocumentRequest>();
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingPipeline<,>));
        });

        services.AddScoped<IQRCodeService, QRCodeService>();

        return services;
    }
}

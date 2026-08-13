using Defra.PTS.Web.CertificateGenerator.Interfaces;
using Defra.PTS.Web.CertificateGenerator.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Defra.PTS.Web.CertificateGenerator.Services;

public class CompositeCertificateGenerator : ICertificateGenerator
{
    private static readonly MethodInfo renderAsyncMethod = ReflectionUtil.GetGenericMethod<CompositeCertificateGenerator>(c => c.GenerateAsync(0, default));

    private readonly IServiceProvider services;

    public CompositeCertificateGenerator(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        this.services = services;
    }

    public async Task<CertificateResult> GenerateAsync<TModel>(TModel certificate, CancellationToken cancellationToken)
    {
        var generator = services.GetRequiredService<ICertificateGenerator<TModel>>();
        return await generator.GenerateAsync(certificate, cancellationToken).ConfigureAwait(false);
    }

    public Task<CertificateResult> GenerateAsync(object certificate, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(certificate);
        var method = renderAsyncMethod.MakeGenericMethod(certificate.GetType());
        return (Task<CertificateResult>)method.Invoke(this, new object[] { certificate, cancellationToken });
    }
}
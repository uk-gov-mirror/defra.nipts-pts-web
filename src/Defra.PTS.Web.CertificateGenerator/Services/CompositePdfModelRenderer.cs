using Defra.PTS.Web.CertificateGenerator.Interfaces;
using Defra.PTS.Web.CertificateGenerator.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Defra.PTS.Web.CertificateGenerator.Services;

public class CompositePdfModelRenderer : IPdfModelRenderer
{
    private static readonly MethodInfo renderAsyncMethod = ReflectionUtil.GetGenericMethod<CompositePdfModelRenderer>(c => c.RenderAsync(0, default));

    private readonly IServiceProvider services;

    public CompositePdfModelRenderer(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        this.services = services;
    }

    public async Task<RenderResult<Stream>> RenderAsync<TModel>(TModel model, CancellationToken cancellationToken)
    {
        var renderer = services.GetRequiredService<IPdfModelRenderer<TModel>>();
        return await renderer.RenderAsync(model, cancellationToken).ConfigureAwait(false);
    }

    public Task<RenderResult<Stream>> RenderAsync(object model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);
        var method = renderAsyncMethod.MakeGenericMethod(model.GetType());
        return (Task<RenderResult<Stream>>)method.Invoke(this, new object[] { model, cancellationToken });
    }
}
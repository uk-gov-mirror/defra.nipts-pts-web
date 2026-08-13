using Defra.PTS.Web.CertificateGenerator.Interfaces;
using Defra.PTS.Web.CertificateGenerator.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Defra.PTS.Web.CertificateGenerator.Services;

public class CompositeHtmlModelRenderer : IHtmlModelRenderer
{
    private static readonly MethodInfo renderAsyncMethod = ReflectionUtil.GetGenericMethod<CompositeHtmlModelRenderer>(c => c.RenderAsync(0, default));

    private readonly IServiceProvider services;

    public CompositeHtmlModelRenderer(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        this.services = services;
    }

    public async Task<RenderResult<string>> RenderAsync<TModel>(TModel model, CancellationToken cancellationToken)
    {
        var renderer = services.GetRequiredService<IHtmlModelRenderer<TModel>>();
        return await renderer.RenderAsync(model, cancellationToken).ConfigureAwait(false);
    }

    public Task<RenderResult<string>> RenderAsync(object model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);
        var method = renderAsyncMethod.MakeGenericMethod(model.GetType());
        return (Task<RenderResult<string>>)method.Invoke(this, new object[] { model, cancellationToken });
    }
}
using System;
using HybridModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HybridMvcCoreBuilderExtensions
    {
        public static IMvcBuilder AddHybridModelBinder(this IMvcBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.AddMvcOptions(mvcOptions => mvcOptions.AddHybridModelBinder(builder.Services));
        }

        public static IMvcBuilder AddHybridModelBinder(this IMvcBuilder builder,
            Action<HybridModelBinderOptions> setupAction)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.AddMvcOptions(mvcOptions => mvcOptions.AddHybridModelBinder(builder.Services, setupAction));
        }

        public static IMvcCoreBuilder AddHybridModelBinder(this IMvcCoreBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.AddMvcOptions(mvcOptions => mvcOptions.AddHybridModelBinder(builder.Services));
        }

        public static IMvcCoreBuilder AddHybridModelBinder(this IMvcCoreBuilder builder,
            Action<HybridModelBinderOptions> setupAction)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.AddMvcOptions(mvcOptions => mvcOptions.AddHybridModelBinder(builder.Services, setupAction));
        }

        private static void AddHybridModelBinder(this MvcOptions mvcOptions, IServiceCollection services,
            Action<HybridModelBinderOptions> setupAction = null)
        {
            var options = new HybridModelBinderOptions();
            setupAction?.Invoke(options);

            var serviceProvider = services.BuildServiceProvider();
            var readerFactory = serviceProvider.GetRequiredService<IHttpRequestStreamReaderFactory>();

            var hybridConvention = new HybridModelBinderApplicationModelConvention();
            mvcOptions.Conventions.Add(hybridConvention);

            var provider = !options.Passthrough
                ? (IModelBinderProvider) new DefaultHybridModelBinderProvider(mvcOptions.InputFormatters,
                    readerFactory)
                : new DefaultPassthroughHybridModelBinderProvider(mvcOptions.InputFormatters, readerFactory);
            mvcOptions.ModelBinderProviders.Insert(0, provider);
        }
    }
}
// =====================================================================================================================
// = LICENSE:       Copyright (c) 2022 Kevin De Coninck
// =
// =                Permission is hereby granted, free of charge, to any person
// =                obtaining a copy of this software and associated documentation
// =                files (the "Software"), to deal in the Software without
// =                restriction, including without limitation the rights to use,
// =                copy, modify, merge, publish, distribute, sublicense, and/or sell
// =                copies of the Software, and to permit persons to whom the
// =                Software is furnished to do so, subject to the following
// =                conditions:
// =
// =                The above copyright notice and this permission notice shall be
// =                included in all copies or substantial portions of the Software.
// =
// =                THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// =                EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// =                OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// =                NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// =                HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// =                WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// =                FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// =                OTHER DEALINGS IN THE SOFTWARE.
// =====================================================================================================================
namespace Restify.Modules.Tests.Extensions;

using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Restify.Modules.Extensions;
using Restify.Modules.Middleware.Abstractions;
using Restify.Modules.Routing.Abstractions;
using Restify.Modules.Tests.Internal.Extensions;

using Xunit;

public abstract class WebApplicationBuilderMiddlewareExtensionsUts
{
    public abstract class AddMiddlewareModule
    {
        public sealed class ASingleModuleWithoutDepdendencies
        {
            [SuppressMessage("Performance", "CA1812", Justification = "API Design.")]
            internal sealed class Module : IRestifyMiddlewareModule
            {
                public void UseMiddleware(WebApplication app)
                {
                    // NOTE: Intentionally left blank.
                }
            }

            [Fact]
            internal void TheModuleIsRegistered()
            {
                // ARRANGE.
                WebApplicationBuilder builder = WebApplication.CreateBuilder();

                // ACT.
                _ = builder.AddMiddlewareModule<Module>();

                // ASSERT.
                Assert.True(builder.HasService<IRestifyMiddlewareModule, Module>());
            }
        }

        public sealed class MultipleModulesWithoutDepdendencies
        {
            [SuppressMessage("Performance", "CA1812", Justification = "API Design.")]
            internal sealed class ModuleOne : IRestifyMiddlewareModule
            {
                public void UseMiddleware(WebApplication app)
                {
                    // NOTE: Intentionally left blank.
                }
            }

            [SuppressMessage("Performance", "CA1812", Justification = "API Design.")]
            internal sealed class ModuleTwo : IRestifyMiddlewareModule
            {
                public void UseMiddleware(WebApplication app)
                {
                    // NOTE: Intentionally left blank.
                }
            }

            [Fact]
            internal void TheModulesAreRegistered()
            {
                // ARRANGE.
                WebApplicationBuilder builder = WebApplication.CreateBuilder();

                // ACT.
                _ = builder.AddMiddlewareModule<ModuleOne>();
                _ = builder.AddMiddlewareModule<ModuleTwo>();

                // ASSERT.
                Assert.True(builder.HasService<IRestifyMiddlewareModule, ModuleOne>());
                Assert.True(builder.HasService<IRestifyMiddlewareModule, ModuleTwo>());
            }
        }

        public sealed class AModuleWithAnUnresolvedDepdendency
        {
            [SuppressMessage("Performance", "CA1812", Justification = "API Design.")]
            internal sealed class Module : IRestifyMiddlewareModule
            {
                public Module(IDependencyService _)
                {
                }

                public void UseMiddleware(WebApplication app)
                {
                    // NOTE: Intentionally left blank.
                }

                internal interface IDependencyService
                {
                }
            }

            [Fact]
            internal void AnInvalidOperationExceptionIsThrowed()
            {
                // ARRANGE.
                WebApplicationBuilder builder = WebApplication.CreateBuilder();

                // ACT.
                Exception exception = Record.Exception(() => builder.AddMiddlewareModule<Module>());

                // ASSERT.
                _ = Assert.IsType<InvalidOperationException>(exception);
            }
        }

        public sealed class AModuleWithAResolvedDepdendency
        {
            [SuppressMessage("Performance", "CA1812", Justification = "API Design.")]
            internal sealed class Module : IRestifyMiddlewareModule
            {
                public Module(IConfiguration _)
                {
                }

                public void UseMiddleware(WebApplication app)
                {
                    // NOTE: Intentionally left blank.
                }
            }

            internal interface IDependencyService
            {
            }

            [SuppressMessage("Performance", "CA1812", Justification = "API Design.")]
            internal sealed class DependencyService : IDependencyService
            {
            }

            [Fact]
            internal void TheModuleIsRegisteredAsAnIRestifyRoutingModuleInstance()
            {
                // ARRANGE.
                WebApplicationBuilder builder = WebApplication.CreateBuilder();

                // ACT.
                _ = builder.AddMiddlewareModule<Module>();

                // ASSERT.
                ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
                _ = Assert.IsType<Module>(serviceProvider.GetRequiredService<IRestifyMiddlewareModule>());
            }
        }
    }
}

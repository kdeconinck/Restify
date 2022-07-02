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
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;

using Restify.Modules.Extensions;
using Restify.Modules.Routing.Abstractions;
using Restify.Modules.Tests.Internal.Extensions;

using Xunit;

public abstract class WebApplicationBuilderRoutingExtensionsUts
{
    public abstract class AddRoutingModule
    {
        public sealed class ASingleModuleWithoutDepdendencies
        {
            [Fact]
            internal void TheModuleIsRegistered()
            {
                // ARRANGE.
                WebApplicationBuilder builder = WebApplication.CreateBuilder();

                // ACT.
                _ = builder.AddRoutingModule<Module>();

                // ASSERT.
                Assert.True(builder.HasService<IRestifyRoutingModule, Module>());
            }

            [SuppressMessage("Performance", "CA1812", Justification = "API Design.")]
            internal sealed class Module : IRestifyRoutingModule
            {
                public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
                {
                    // NOTE: Intentionally left blank.
                }
            }
        }

        public sealed class MultipleModulesWithoutDepdendencies
        {
            [Fact]
            internal void TheModulesAreRegistered()
            {
                // ARRANGE.
                WebApplicationBuilder builder = WebApplication.CreateBuilder();

                // ACT.
                _ = builder.AddRoutingModule<ModuleOne>();
                _ = builder.AddRoutingModule<ModuleTwo>();

                // ASSERT.
                Assert.True(builder.HasService<IRestifyRoutingModule, ModuleOne>());
                Assert.True(builder.HasService<IRestifyRoutingModule, ModuleTwo>());
            }

            [SuppressMessage("Performance", "CA1812", Justification = "API Design.")]
            internal sealed class ModuleOne : IRestifyRoutingModule
            {
                public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
                {
                    // NOTE: Intentionally left blank.
                }
            }

            [SuppressMessage("Performance", "CA1812", Justification = "API Design.")]
            internal sealed class ModuleTwo : IRestifyRoutingModule
            {
                public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
                {
                    // NOTE: Intentionally left blank.
                }
            }
        }

        public sealed class AModuleWithAnUnresolvedDepdendency
        {
            [Fact]
            internal void AnInvalidOperationExceptionIsThrowed()
            {
                // ARRANGE.
                WebApplicationBuilder builder = WebApplication.CreateBuilder();

                // ACT.
                Exception exception = Record.Exception(() => builder.AddRoutingModule<Module>());

                // ASSERT.
                _ = Assert.IsType<InvalidOperationException>(exception);
            }

            [SuppressMessage("Performance", "CA1812", Justification = "API Design.")]
            internal sealed class Module : IRestifyRoutingModule
            {
                public Module(IDependencyService _)
                { }

                public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
                {
                    // NOTE: Intentionally left blank.
                }

                internal interface IDependencyService
                { }
            }
        }

        public sealed class AModuleWithAResolvedDepdendency
        {
            [Fact]
            internal void TheModuleIsRegisteredAsAnIRestifyRoutingModuleInstance()
            {
                // ARRANGE.
                WebApplicationBuilder builder = WebApplication.CreateBuilder();

                // ACT.
                _ = builder.AddRoutingModule<Module>();

                // ASSERT.
                Assert.True(builder.HasService<IRestifyRoutingModule, Module>());
            }

            [SuppressMessage("Performance", "CA1812", Justification = "API Design.")]
            internal sealed class Module : IRestifyRoutingModule
            {
                public Module(IConfiguration _)
                { }

                public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
                {
                    // NOTE: Intentionally left blank.
                }
            }
        }
    }
}

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using org.testar.statemodel.analysis;

namespace org.testar.statemodel.analysis.webserver
{
    public class JettyServer
    {
        private WebApplication? _app;

        public void Start(string resourceBase, AnalysisManager analysisManager)
        {
            var options = new WebApplicationOptions
            {
                ContentRootPath = resourceBase,
                WebRootPath = resourceBase
            };

            WebApplicationBuilder builder = WebApplication.CreateBuilder(options);
            builder.Services.AddSingleton(analysisManager);

            WebApplication app = builder.Build();
            app.UseStaticFiles();

            app.MapGet("/models", (AnalysisManager manager) => Results.Json(manager.FetchModels()));
            app.MapPost("/models", async (HttpRequest request, AnalysisManager manager) =>
            {
                var form = await request.ReadFormAsync();
                string sequenceId = form["sequenceId"];
                return Results.Json(manager.FetchTestSequence(sequenceId));
            });

            app.MapPost("/graph", async (HttpRequest request, AnalysisManager manager) =>
            {
                var form = await request.ReadFormAsync();
                bool abstractLayerRequired = form.ContainsKey("abstractoption");
                bool concreteLayerRequired = form.ContainsKey("concreteoption");
                bool sequenceLayerRequired = form.ContainsKey("sequenceoption");
                bool showCompoundGraph = form.ContainsKey("compoundoption");
                string modelIdentifier = form["modelIdentifier"];
                string concreteStateIdentifier = form["concrete_state_id"];

                if (!string.IsNullOrWhiteSpace(modelIdentifier))
                {
                    string fileName = manager.FetchGraphForModel(modelIdentifier, abstractLayerRequired, concreteLayerRequired, sequenceLayerRequired, showCompoundGraph);
                    return Results.Json(new { graphContentFile = fileName, contentFolder = modelIdentifier });
                }

                if (!string.IsNullOrWhiteSpace(concreteStateIdentifier))
                {
                    string fileName = manager.FetchWidgetTree(concreteStateIdentifier);
                    return Results.Json(new { graphContentFile = fileName, contentFolder = concreteStateIdentifier });
                }

                return Results.BadRequest();
            });

            app.Urls.Add("http://localhost:8090");
            app.StartAsync().GetAwaiter().GetResult();
            _app = app;
        }

        public void Stop()
        {
            if (_app == null)
            {
                return;
            }

            _app.StopAsync().GetAwaiter().GetResult();
            _app.DisposeAsync().GetAwaiter().GetResult();
            _app = null;
        }
    }
}

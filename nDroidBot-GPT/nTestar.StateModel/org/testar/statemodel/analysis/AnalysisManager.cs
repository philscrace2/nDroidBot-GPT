using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using org.testar.statemodel.analysis.jsonformat;
using org.testar.statemodel.analysis.representation;
using org.testar.statemodel.persistence.orientdb.entity;

namespace org.testar.statemodel.analysis
{
    public class AnalysisManager
    {
        private readonly Config _dbConfig;
        private readonly string _outputDir;

        public AnalysisManager(Config config, string outputDir)
        {
            _dbConfig = config;
            _outputDir = outputDir;
        }

        public void Shutdown()
        {
        }

        public List<AbstractStateModel> FetchModels()
        {
            // TODO: Implement using graph DB abstraction.
            return new List<AbstractStateModel>();
        }

        public List<ActionViz> FetchTestSequence(string sequenceId)
        {
            // TODO: Implement using graph DB abstraction.
            return new List<ActionViz>();
        }

        public string FetchGraphForModel(string modelIdentifier, bool includeAbstract, bool includeConcrete, bool includeSequence, bool showCompoundGraph)
        {
            // TODO: Implement using graph DB abstraction.
            return WriteJson(modelIdentifier, new List<Element>());
        }

        public string FetchWidgetTree(string concreteStateIdentifier)
        {
            // TODO: Implement using graph DB abstraction.
            return WriteJson(concreteStateIdentifier, new List<Element>());
        }

        private string WriteJson(string folderName, List<Element> elements)
        {
            string directory = Path.Combine(_outputDir, folderName);
            Directory.CreateDirectory(directory);
            string fileName = Path.Combine(directory, "graph.json");
            string json = JsonSerializer.Serialize(elements, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, json);
            return fileName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using org.testar;
using org.testar.monkey.alayer;
using org.testar.statemodel.actionselector;
using org.testar.statemodel.events;
using org.testar.statemodel.persistence;
using org.testar.statemodel.sequence;

namespace org.testar.statemodel
{
    public static class StateModelManagerFactory
    {
        public static StateModelManager GetStateModelManager(string applicationName, string applicationVersion, TaggableBase configTags)
        {
            if (!configTags.get(StateModelTags.StateModelEnabled))
            {
                return new DummyModelManager();
            }

            ISet<ITag> abstractTags = new HashSet<ITag>(CodingManager.getCustomTagsForAbstractId());
            if (abstractTags.Count == 0)
            {
                throw new ArgumentException("No Abstract State Attributes were provided in the settings file");
            }

            PersistenceManagerFactoryBuilder.ManagerType managerType;
            if (configTags.get(StateModelTags.DataStoreMode) == PersistenceManager.DataStoreModeNone)
            {
                managerType = PersistenceManagerFactoryBuilder.ManagerType.Dummy;
            }
            else
            {
                managerType = Enum.Parse<PersistenceManagerFactoryBuilder.ManagerType>(configTags.get(StateModelTags.DataStore), true);
            }

            PersistenceManagerFactory persistenceManagerFactory = PersistenceManagerFactoryBuilder.CreatePersistenceManagerFactory(managerType);
            PersistenceManager persistenceManager = persistenceManagerFactory.GetPersistenceManager(configTags);

            string modelIdentifier = CodingManager.getAbstractStateModelHash(applicationName, applicationVersion);

            HashSet<StateModelEventListener> eventListeners = new HashSet<StateModelEventListener>();
            if (persistenceManager is StateModelEventListener listener)
            {
                eventListeners.Add(listener);
            }

            SequenceManager sequenceManager = new SequenceManager(eventListeners, modelIdentifier);

            AbstractStateModel abstractStateModel = new AbstractStateModel(
                modelIdentifier,
                applicationName,
                applicationVersion,
                abstractTags,
                persistenceManager as StateModelEventListener);

            ActionSelector actionSelector = CompoundFactory.GetCompoundActionSelector(configTags);

            bool storeWidgets = configTags.get(StateModelTags.StateModelStoreWidgets);

            return new ModelManager(abstractStateModel, actionSelector, persistenceManager, sequenceManager, storeWidgets);
        }
    }
}

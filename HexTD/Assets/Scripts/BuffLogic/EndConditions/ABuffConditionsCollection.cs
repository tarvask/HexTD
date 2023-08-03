using System.Collections.Generic;
using ExitGames.Client.Photon;
using Match.Serialization;
using Newtonsoft.Json;
using Tools;

namespace BuffLogic
{
    public abstract class ABuffConditionsCollection : BaseDisposable, ISerializableToNetwork
    {        
        [JsonProperty("Conditions")] protected readonly LinkedList<IBuffCondition> Conditions;
        [JsonIgnore] private bool _isEndConditionDone;

        [JsonIgnore] public bool IsEndConditionDone => _isEndConditionDone;

        protected ABuffConditionsCollection()
        {
            Conditions = new LinkedList<IBuffCondition>();
            _isEndConditionDone = false;
        }

        public void MergeConditions(ABuffConditionsCollection conditionCollection)
        {
            foreach (var condition in Conditions)
            {
                condition.Dispose();
            }
            Conditions.Clear();
            
            foreach (var condition in conditionCollection.Conditions)
                Conditions.AddFirst(condition);

            _isEndConditionDone = false;
        }

        public void AddCondition(IBuffCondition condition)
        {
            Conditions.AddFirst(condition);
        }

        public void Update()
        {
            _isEndConditionDone = InvokeEndConditions();
        }

        protected abstract bool InvokeEndConditions();

        protected override void OnDispose()
        {
            foreach (var condition in Conditions)
            {
                condition.Dispose();
            }
            Conditions.Clear();
        }
        
        public Hashtable ToNetwork()
        {
            return SerializerToNetwork.EnumerableToNetwork(Conditions, Conditions.Count);
        }
    }
}
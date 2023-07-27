using System.Collections.Generic;
using ExitGames.Client.Photon;
using Match.Serialization;
using Tools;

namespace BuffLogic
{
    public abstract class ABuffConditionsCollection : BaseDisposable, ISerializableToNetwork
    {        
        [SerializableArrayToNetwork("Conditions", typeof(IBuffCondition))] protected readonly LinkedList<IBuffCondition> Conditions;
        private bool _isEndConditionDone;

        public bool IsEndConditionDone => _isEndConditionDone;

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
            return Match.Serialization.SerializerToNetwork.EnumerableToNetwork(Conditions, Conditions.Count);
        }
    }
}
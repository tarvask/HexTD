using System.Collections.Generic;
using Match.State.CheckSum;
using UnityEngine;

namespace Match.State
{
    public class MatchStateVerificationClient : MatchStateVerificationBase
    {
        private const byte CheckSumHistoryMaxLength = 5;
        private readonly Queue<MatchStateCheckSum> _serverCheckSumHistory;
        
        public MatchStateVerificationClient(Context context) : base(context)
        {
            _serverCheckSumHistory = new Queue<MatchStateCheckSum>(CheckSumHistoryMaxLength);
        }
        
        public override void BroadcastMatchCheckSum(MatchStateCheckSum checkSum)
        {
            // do nothing
        }

        public override void VerifyCheckSum(MatchStateCheckSum serverCheckSum)
        {
            _serverCheckSumHistory.Enqueue(serverCheckSum);

            if (CheckHistory())
            {
                RequestState();
                _serverCheckSumHistory.Clear();
            }
        }

        private bool CheckHistory()
        {
            bool isStateCorrupted = false;
            bool isStateBackward = false;
            int checkSumsToDequeue = 0;
            
            foreach (MatchStateCheckSum serverCheckSum in _serverCheckSumHistory)
            {
                // stop checking on absent checksum
                if (!_context.MatchStateCheckSumComputerController.TryGetCheckSumForEngineFrame(serverCheckSum.EngineFrame,
                        out MatchStateCheckSum clientCheckSum))
                    break;

                // count checksums to drop
                if (clientCheckSum.Equals(serverCheckSum))
                    checkSumsToDequeue++;
                else
                {
                    Debug.LogError($"Different checksums for engine frame {clientCheckSum.EngineFrame}, requesting state from server");
                    isStateCorrupted = true;
                    break;
                }
            }

            // drop verified checksum
            for (int i = 0; i < checkSumsToDequeue; i++)
                _serverCheckSumHistory.Dequeue();

            if (_serverCheckSumHistory.Count == CheckSumHistoryMaxLength)
            {
                Debug.LogError($"State is too backward, now requesting it from server");
                isStateBackward = true;
            }

            return isStateCorrupted || isStateBackward;
        }

        private void RequestState()
        {
            _context.RequestMatchStateReactiveCommand.Execute();
        }
            
        }
    }
}
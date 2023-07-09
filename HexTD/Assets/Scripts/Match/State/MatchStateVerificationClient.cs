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
                MatchStateCheckSumFindingResultType checkSumFindingResult =
                    _context.MatchStateCheckSumComputerController.TryGetCheckSumForEngineFrame(
                        serverCheckSum.EngineFrame, out MatchStateCheckSum clientCheckSum);

                // do not drop and exit, because further elements are even newer
                if (checkSumFindingResult is MatchStateCheckSumFindingResultType.TooNew)
                    break;

                // drop, because there is no chance to get such checksums in history later
                if (checkSumFindingResult is MatchStateCheckSumFindingResultType.TooOld or MatchStateCheckSumFindingResultType.NotExistInRange)
                {
                    checkSumsToDequeue++;
                    continue;
                }

                // count checksums to drop
                if (checkSumFindingResult is MatchStateCheckSumFindingResultType.ExistInRange
                    && clientCheckSum.Equals(serverCheckSum))
                    checkSumsToDequeue++;
                else
                {
                    Debug.LogError($"Different checksums for engine frame {clientCheckSum.EngineFrame}: " +
                                   $"{GetDecodedMessageForCheckSumComparison(clientCheckSum, serverCheckSum)}, " +
                                   $"requesting state from server");
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

        // only valid for objects' count tactic
        private string GetDecodedMessageForCheckSumComparison(
            MatchStateCheckSum clientCheckSum,
            MatchStateCheckSum serverCheckSum)
        {
            var decodedMessage = new StringBuilder(256);

            // player1
            int clientPlayer1ProjectilesCount = clientCheckSum.Player1CheckSum % 256;
            int serverPlayer1ProjectilesCount = serverCheckSum.Player1CheckSum % 256;

            if (clientPlayer1ProjectilesCount != serverPlayer1ProjectilesCount)
                decodedMessage.Append($"Player1 projectiles: client={clientPlayer1ProjectilesCount}, server={serverPlayer1ProjectilesCount}\n");
            
            int clientPlayer1MobsCount = clientCheckSum.Player1CheckSum / 256 % 256;
            int serverPlayer1MobsCount = serverCheckSum.Player1CheckSum / 256 % 256;
            
            if (clientPlayer1MobsCount != serverPlayer1MobsCount)
                decodedMessage.Append($"Player1 mobs: client={clientPlayer1MobsCount}, server={serverPlayer1MobsCount}\n");
            
            int clientPlayer1TowersCount = clientCheckSum.Player1CheckSum / 256 / 256 % 256;
            int serverPlayer1TowersCount = serverCheckSum.Player1CheckSum / 256 / 256 % 256;
            
            if (clientPlayer1TowersCount != serverPlayer1TowersCount)
                decodedMessage.Append($"Player1 towers: client={clientPlayer1TowersCount}, server={serverPlayer1TowersCount}\n");
            
            // player2
            int clientPlayer2ProjectilesCount = clientCheckSum.Player2CheckSum % 256;
            int serverPlayer2ProjectilesCount = serverCheckSum.Player2CheckSum % 256;

            if (clientPlayer2ProjectilesCount != serverPlayer2ProjectilesCount)
                decodedMessage.Append($"Player2 projectiles: client={clientPlayer2ProjectilesCount}, server={serverPlayer2ProjectilesCount}\n");
            
            int clientPlayer2MobsCount = clientCheckSum.Player2CheckSum / 256 % 256;
            int serverPlayer2MobsCount = serverCheckSum.Player2CheckSum / 256 % 256;
            
            if (clientPlayer2MobsCount != serverPlayer2MobsCount)
                decodedMessage.Append($"Player2 mobs: client={clientPlayer2MobsCount}, server={serverPlayer2MobsCount}\n");
            
            int clientPlayer2TowersCount = clientCheckSum.Player2CheckSum / 256 / 256 % 256;
            int serverPlayer2TowersCount = serverCheckSum.Player2CheckSum / 256 / 256 % 256;
            
            if (clientPlayer2TowersCount != serverPlayer2TowersCount)
                decodedMessage.Append($"Player2 towers: client={clientPlayer2TowersCount}, server={serverPlayer2TowersCount}\n");

            return decodedMessage.ToString();
        }
    }
}
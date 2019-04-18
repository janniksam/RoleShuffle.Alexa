﻿using System.Text;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Response;
using RoleShuffle.Base;

namespace RoleShuffle.Application.Games.TheResistanceAvalon
{
    public class TheResistanceAvalonGame : BaseGame<TheResistanceAvalonRound>
    {
        public TheResistanceAvalonGame()
            : base("The Resistance Avalon", Constants.GameNumbers.TheResistanceAvalon)
        {
        }

        public override Task<SkillResponse> DistributeRoles(SkillRequest request)
        {
            return PerformDefaultDistributionPhase(request);
        }

        public override Task<SkillResponse> StartGameRequested(SkillRequest request)
        {
            var userId = request.Context.System.User.UserId;

            var newRound = new TheResistanceAvalonRound();
            RunningRounds.AddOrUpdate(userId, newRound, (k, v) => newRound);

            return PerformDefaultStartGamePhaseWithDistributionPhaseContinuation(request);
        }
    }
}
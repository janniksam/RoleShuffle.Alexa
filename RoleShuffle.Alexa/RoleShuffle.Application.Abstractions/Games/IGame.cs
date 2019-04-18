﻿using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Response;

namespace RoleShuffle.Application.Abstractions.Games
{
    public interface IGame
    {
        Task<SkillResponse> StartGameRequested(SkillRequest skillRequest);

        Task<SkillResponse> DistributeRoles(SkillRequest request);

        Task<SkillResponse> PerformNightPhase(SkillRequest request);

        bool IsPlaying(string userId);

        short GameNumber { get; }

        string GameName { get; }
    }
}
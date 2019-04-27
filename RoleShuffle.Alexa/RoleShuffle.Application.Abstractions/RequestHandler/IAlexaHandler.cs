﻿using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Response;
using RoleShuffle.Base.Aspects;

namespace RoleShuffle.Application.Abstractions.RequestHandler
{
    [LogMethodScope]
    public interface IAlexaHandler
    {
        Task<SkillResponse> HandleAync(SkillRequest requestJson);
    }
}
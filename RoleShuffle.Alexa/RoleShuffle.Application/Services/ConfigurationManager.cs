﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RoleShuffle.Application.Abstractions.Games;
using RoleShuffle.Application.Abstractions.Model;
using RoleShuffle.Application.Abstractions.Services;

namespace RoleShuffle.Application.Services
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IEnumerable<IGame> m_games;
        private DateTime m_lastBackupDate = DateTime.MinValue;

        public ConfigurationManager(IEnumerable<IGame> games)
        {
            m_games = games;
        }

        public DateTime GetLastBackupDate() => m_lastBackupDate;
    
        public async Task LoadAsync(CancellationToken cancellationToken)
        {
            var configuration = await ReadFromFileAsync(cancellationToken).ConfigureAwait(false);
            m_lastBackupDate = configuration.LastBackupDate;
            foreach (var configurationSavedGame in configuration.SavedGames)
            {
                var game = m_games.FirstOrDefault(p => p.GameId == configurationSavedGame.Id);
                game?.InitFromConfiguration(configurationSavedGame);
            }
        }

        private async Task<RoleServantConfiguration> ReadFromFileAsync(CancellationToken cancellationToken)
        {
            try
            {
                var configLocation = GetConfigLocation();
                if (!Directory.Exists(configLocation))
                {
                    Directory.CreateDirectory(configLocation);
                }

                if (!File.Exists(GetConfigFilePath()))
                {
                    return new RoleServantConfiguration();
                }

                var content = await File.ReadAllTextAsync(GetConfigFilePath(), cancellationToken)
                    .ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(content))
                {
                    return new RoleServantConfiguration();
                }

                return JsonConvert.DeserializeObject<RoleServantConfiguration>(content, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
            }
            catch (Exception e)
            {
                throw new ApplicationException("Could not load the configuration ", e);
            }
        }

        public async Task WriteToDiskAsync(CancellationToken cancellationToken)
        {
            var configuration = GetCurrentConfiguration();
            var serializeObject = JsonConvert.SerializeObject(configuration, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            });
            
            await File.WriteAllTextAsync(GetConfigFilePath(), serializeObject, cancellationToken)
                .ConfigureAwait(false);
        }

        private RoleServantConfiguration GetCurrentConfiguration()
        {
            var now = DateTime.UtcNow;

            var configuration = new RoleServantConfiguration();
            foreach (var game in m_games)
            {
                configuration.SavedGames.Add(new SavedGame
                {
                    Id = game.GameId,
                    GameRounds = game.GetOpenGames().Where(p => p.CreationTime.AddMonths(6) > now).ToList(),
                    GameStats = game.GetCurrentStats()
                });
            }

            m_lastBackupDate = DateTime.UtcNow;
            configuration.LastBackupDate = m_lastBackupDate;
            return configuration;
        }

        private string GetConfigLocation()
        {
            var currentDirectory = AppContext.BaseDirectory;
            var path = Path.Combine(currentDirectory, "data");
            return path;
        }

        private string GetConfigFilePath()
        {
            return Path.Combine(GetConfigLocation(), "roleservant.json");
        }
    }
}
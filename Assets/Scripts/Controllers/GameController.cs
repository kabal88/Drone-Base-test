﻿using System.Collections.Generic;
using System.Linq;
using DroneBase.Enums;
using DroneBase.Interfaces;
using DroneBase.Libraries;
using DroneBase.Services;
using DroneBase.Systems;

namespace DroneBase.Controllers
{
    public sealed class GameController : IUpdatable, IFixUpdatable
    {
        private readonly IGameModel _model;

        private GameController(IGameModel model)
        {
            _model = model;
        }

        public static GameController CreateGameController(IGameDescription description, Library library)
        {
            var game = new GameController(description.GameModel);
            game._model.Init(library);

            return game;
        }

        public void StartGame()
        {
            SetupServices();
            var spawnSystem =
                SpawnSystem.CreateSpawnSystem(
                    _model.Library.GetSpawnSystemDescription(_model.PresetData.SpawnSystemId));

            var camera = CameraController.CreateCameraController(
                _model.Library.GetCameraDescription(_model.PresetData.CameraContainerId),
                spawnSystem.GetSpawnPointsByPredicate(x => x.Model.PointType == EntityType.Camera)
                    .First().Model.PointData);
            
            var inputSystem = new InputSystem(camera.Camera);
            
            camera.InjectMouseInputSystem(inputSystem);
            
            var playerController = PlayerController.CreatePlayerController(
                _model.Library.GetPlayerDescription(_model.PresetData.PlayerContainerId),
                inputSystem);

            var drone = DroneController.CreateDroneController(
                _model.Library.GetDroneDescription(_model.PresetData.DroneContainerId),
                spawnSystem.GetSpawnPointsByPredicate(x => x.Model.PointType == EntityType.Unit).First().Model
                    .PointData);
        }

        private void SetupServices()
        {
            var updateService = new UpdateLocalService();
            _model.SetUpdateService(updateService);
            ServiceLocator.SetService<IUpdateService>(updateService);

            var fixUpdateService = new FixUpdateLocalService();
            _model.SetFixUpdateService(fixUpdateService);
            ServiceLocator.SetService<IFixUpdateService>(fixUpdateService);
            
            ServiceLocator.SetService<ISelectionService>(new SelectionService());
        }

        public void UpdateLocal(float deltaTime)
        {
            _model.UpdateService.UpdateLocal(deltaTime);
        }

        public void FixedUpdateLocal()
        {
            _model.FixUpdateService.FixedUpdateLocal();
        }
    }
}
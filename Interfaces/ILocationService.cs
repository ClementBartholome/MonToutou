﻿using DogTracker.Models;
using DogTracker.ViewModels;

namespace DogTracker.Interfaces;

public interface ILocationService
{
    Task<GeolocationPosition> GetCurrentPositionAsync();
    event EventHandler<GeolocationPosition> OnPositionChanged;
    Task StartWatchingPositionAsync();
    Task StopWatchingPositionAsync();
    Task SyncPositions(GeolocationPosition[] newPositions);
    Task<WalkDataViewModel?> CheckForOngoingTrackedWalkAsync();
    Task<DateTime> CheckForOngoingUntrackedWalkAsync();
    Task StartTimer();
    Task StopUntrackedWalkAsync();

}
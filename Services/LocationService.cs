﻿using System.Text.Json;
using DogTracker.Interfaces;
using DogTracker.Models;
using DogTracker.ViewModels;
using Microsoft.JSInterop;

namespace DogTracker.Services;

public class LocationService : ILocationService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IWebHostEnvironment _env;
    private DotNetObjectReference<LocationService> _objectReference;
    public event EventHandler<GeolocationPosition>? OnPositionChanged;

    public LocationService(IJSRuntime jsRuntime, IWebHostEnvironment env)
    {
        _jsRuntime = jsRuntime;
        _env = env;
        _objectReference = DotNetObjectReference.Create(this);
    }

    public async Task<GeolocationPosition> GetCurrentPositionAsync()
    {
        return await _jsRuntime.InvokeAsync<GeolocationPosition>("getCurrentPosition");
    }

    public async Task StartWatchingPositionAsync()
    {
        if (_env.IsDevelopment())
        {
            await _jsRuntime.InvokeVoidAsync("startWatchingPosition", _objectReference, true);
        }
        else
        {
            await _jsRuntime.InvokeVoidAsync("startWatchingPosition", _objectReference, false);
        }
        
    }

    public async Task StopWatchingPositionAsync()
    {
        await _jsRuntime.InvokeVoidAsync("stopWatchingPosition");
    }

    public async Task StopUntrackedWalkAsync()
    {
        await _jsRuntime.InvokeVoidAsync("stopUntrackedWalk");
    }

    [JSInvokable]
    public void OnLocationUpdate(GeolocationPosition position)
    {
        OnPositionChanged?.Invoke(this, position);
    }
    
    [JSInvokable]
    public Task SyncPositions(GeolocationPosition[] newPositions)
    {
        foreach (var position in newPositions)
        {
            OnPositionChanged?.Invoke(this, position);
        }
    
        return Task.CompletedTask;
    }
    
    public async Task<WalkDataViewModel?> CheckForOngoingTrackedWalkAsync()
    {
        return await _jsRuntime.InvokeAsync<WalkDataViewModel?>("getStoredWalkData");
    }
    
    public async Task<DateTime> CheckForOngoingUntrackedWalkAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string>("getStoredUntrackedWalkData");
            return string.IsNullOrEmpty(result) ? DateTime.MinValue : DateTime.Parse(result, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la récupération de la date de promenade non suivie : {ex.Message}");
            return DateTime.MinValue;
        }
    }
    
    public async Task StartTimer()
    {
        await _jsRuntime.InvokeVoidAsync("startTimer");
    }
}
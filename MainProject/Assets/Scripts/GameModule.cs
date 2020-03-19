using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModule : MonoBehaviour , IModule
{
    ModuleBeginPlay _eventBeginPlay;
    ModuleInitialize _eventInitialize;
    ModulePause _eventPause;
    ModuleResume _eventResume;

    public void AttachBeginPlayEvent(ModuleBeginPlay eventHandler)
    {
        _eventBeginPlay += eventHandler;
    }

    public void AttachInitializeEvent(ModuleInitialize eventHandler)
    {
        _eventInitialize += eventHandler;
    }

    public void AttachPauseEvent(ModulePause eventHandler)
    {
        _eventPause += eventHandler;
    }

    public void AttachResumeEvent(ModuleResume eventHandler)
    {
        _eventResume += eventHandler;
    }

    public void BeginPlay()
    {
        
    }

    public void DetachBeginPlayEvent(ModuleBeginPlay eventHandler)
    {
        
    }

    public void DetachInitializeEvent(ModuleInitialize eventHandler)
    {
        
    }

    public void DetachPauseEvent(ModulePause eventHandler)
    {
        
    }

    public void DetachResumeEvent(ModuleResume eventHandler)
    {
        
    }

    public void Initialize()
    {
        
    }

    public void Pause()
    {
        
    }

    public void Resume()
    {
        
    }

}

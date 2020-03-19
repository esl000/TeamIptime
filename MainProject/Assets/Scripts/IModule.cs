using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ModuleInitialize();
public delegate void ModuleBeginPlay();
public delegate void ModuleResume();
public delegate void ModulePause();

public interface IModule
{
    void Initialize();
    void BeginPlay();
    void Resume();
    void Pause();

    void AttachInitializeEvent(ModuleInitialize eventHandler);
    void AttachBeginPlayEvent(ModuleBeginPlay eventHandler);
    void AttachResumeEvent(ModuleResume eventHandler);
    void AttachPauseEvent(ModulePause eventHandler);

    void DetachInitializeEvent(ModuleInitialize eventHandler);
    void DetachBeginPlayEvent(ModuleBeginPlay eventHandler);
    void DetachResumeEvent(ModuleResume eventHandler);
    void DetachPauseEvent(ModulePause eventHandler);
}

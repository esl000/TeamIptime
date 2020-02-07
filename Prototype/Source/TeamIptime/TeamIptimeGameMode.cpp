// Copyright 1998-2019 Epic Games, Inc. All Rights Reserved.

#include "TeamIptimeGameMode.h"
#include "TeamIptimeCharacter.h"
#include "UObject/ConstructorHelpers.h"
#include "TeamIptimeCharacter.h"

ATeamIptimeGameMode::ATeamIptimeGameMode()
{
	// set default pawn class to our Blueprinted character
	//static ConstructorHelpers::FClassFinder<APawn> PlayerPawnBPClass(TEXT("/Game/ThirdPersonCPP/Blueprints/ThirdPersonCharacter"));
	//if (PlayerPawnBPClass.Class != NULL)
	//{
	DefaultPawnClass = ATeamIptimeCharacter::StaticClass(); //PlayerPawnBPClass.Class;
	//}
}

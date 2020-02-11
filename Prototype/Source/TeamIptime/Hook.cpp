// Fill out your copyright notice in the Description page of Project Settings.


#include "Hook.h"
#include "TeamIptimeCharacter.h"

// Sets default values
AHook::AHook()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	Hook = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Hook"));
	Hook->SetCollisionProfileName(TEXT("NoCollision"));
	Hook->SetVisibility(false);
	RootComponent = Hook;
	Hook->SetRelativeScale3D(FVector(0.05f, 0.05f, 0.05f));

	static ConstructorHelpers::FObjectFinder<UStaticMesh> SM_HOOK(TEXT("StaticMesh'/Game/StarterContent/Props/MaterialSphere.MaterialSphere'"));
	if (SM_HOOK.Succeeded())
		Hook->SetStaticMesh(SM_HOOK.Object);

	CurrentWireState = EWireState::E_IDLE;
	WireLength = 1000.f;
	WireSpeed = 1500.f;

}

// Called when the game starts or when spawned
void AHook::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void AHook::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	if (CurrentWireState == EWireState::E_EJACULATE)
	{
		FVector direction = (WireDestination - StartPosition).GetSafeNormal();
		FHitResult hit;
		SetActorLocation(GetActorLocation() + direction * WireSpeed * DeltaTime, true, &hit);
		if (hit.bBlockingHit)
		{
			UE_LOG(TIP, Warning, TEXT("Block"));
			CurrentWireState = EWireState::E_RECOIL;
			FVector muzzle = OriginLocationMesh->GetSocketLocation(TEXT("TwoHandedWeapon"));
			ATeamIptimeCharacter* owner = Cast<ATeamIptimeCharacter>(OriginLocationMesh->GetOwner());
			FVector Distance = (hit.ImpactPoint - muzzle);
			owner->DragToWire(Distance * 2.f + FVector::UpVector * Distance.Size());
			return;
		}
		FVector LocToGoalDir = (WireDestination - GetActorLocation()).GetSafeNormal();
		if (FVector::DotProduct(direction, LocToGoalDir) < 0.9f || (WireDestination - GetActorLocation()).Size() < 20.f)
		{
			CurrentWireState = EWireState::E_RECOIL;
			return;
		}
	}
	else if (CurrentWireState == EWireState::E_RECOIL)
	{
		FVector muzzle = OriginLocationMesh->GetSocketLocation(TEXT("TwoHandedWeapon"));

		FVector direction = (muzzle - GetActorLocation()).GetSafeNormal();
		SetActorLocation(GetActorLocation() + WireSpeed * direction * DeltaTime);

		FVector GoalToStart = (muzzle - WireDestination).GetSafeNormal();

		if (FVector::DotProduct(direction, GoalToStart) < 0.f || (muzzle - GetActorLocation()).Size() < 20.f)
		{
			CurrentWireState = EWireState::E_IDLE;
			Hook->SetVisibility(false);
			ATeamIptimeCharacter* owner = Cast<ATeamIptimeCharacter>(OriginLocationMesh->GetOwner());
			owner->FinishWireAction();
			Hook->SetCollisionProfileName(TEXT("NoCollision"));
			return;
		}
	}

}

void AHook::Shoot(USkeletalMeshComponent * OriginLocationMesh, FVector Destination)
{
	this->OriginLocationMesh = OriginLocationMesh;
	StartPosition = OriginLocationMesh->GetSocketLocation(TEXT("TwoHandedWeapon"));
	SetActorLocation(StartPosition);
	WireDestination = Destination;
	Hook->SetVisibility(true);
	CurrentWireState = EWireState::E_EJACULATE;
	Hook->SetCollisionProfileName(TEXT("Hook"));
}


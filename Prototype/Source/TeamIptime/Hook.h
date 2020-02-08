// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "TeamIptime.h"
#include "GameFramework/Actor.h"
#include "Hook.generated.h"


UENUM(BlueprintType)
enum class EWireState : uint8
{
	E_IDLE = 1,
	E_EJACULATE = 2,
	E_RECOIL = 3
};

UCLASS()
class TEAMIPTIME_API AHook : public AActor
{
	GENERATED_BODY()
	
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, meta = (AllowPrivateAccess = "true"))
	UStaticMeshComponent* Hook;

	USkeletalMeshComponent* OriginLocationMesh;

	float WireLength;
	float WireSpeed;
	FVector WireDestination;
	FVector StartPosition;

	EWireState CurrentWireState;
	
public:	
	// Sets default values for this actor's properties
	AHook();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	void Shoot(USkeletalMeshComponent* OriginLocationMesh, FVector Destination);

};

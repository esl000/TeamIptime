// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "TeamIptime.h"
#include "GameFramework/Actor.h"
#include "Bullet.generated.h"

UCLASS()
class TEAMIPTIME_API ABullet : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ABullet();

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, meta = (AllowPrivateAccess = "true"))
	class UStaticMeshComponent* Body;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, meta = (AllowPrivateAccess = "true"))
	class UProjectileMovementComponent* Movement;

	AController* OwnerController;

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

	UFUNCTION()
	void OnHit(UPrimitiveComponent* HitComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, FVector NormalImpulse, const FHitResult& Hit);
public:	
	UFUNCTION(BlueprintCallable)
	void Init(AController* ownerController);
};

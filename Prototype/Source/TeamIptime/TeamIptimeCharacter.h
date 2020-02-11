// Copyright 1998-2019 Epic Games, Inc. All Rights Reserved.

#pragma once

#include "TeamIptime.h"
#include "GameFramework/Character.h"
#include "TeamIptimeCharacter.generated.h"

UENUM(BlueprintType)
enum class ECharacterState : uint8
{
	E_IDLE = 1,
	E_MOVEMENT = 2,
	E_SPRINT = 3,
	E_AVOID = 4,
	E_ACTION = 5,
	E_WIREACTION = 6,
	E_CROUNCH = 1 << 4
};

UCLASS(config=Game)
class ATeamIptimeCharacter : public ACharacter
{
	GENERATED_BODY()

	/** Camera boom positioning the camera behind the character */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = Camera, meta = (AllowPrivateAccess = "true"))
	class USpringArmComponent* CameraBoom;

	/** Follow camera */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = Camera, meta = (AllowPrivateAccess = "true"))
	class UCameraComponent* FollowCamera;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, meta = (AllowPrivateAccess = "true"))
	USkeletalMeshComponent* Weapon;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, meta = (AllowPrivateAccess = "true"))
	TSubclassOf<UUserWidget> CrossHair;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, meta = (AllowPrivateAccess = "true"))
	class UCableComponent* Wire;


	FVector LocalDirection;
	FVector AnimDirection;

public:
	ATeamIptimeCharacter();

	/** Base turn rate, in deg/sec. Other scaling may affect final turn rate. */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category=Camera)
	float BaseTurnRate;

	/** Base look up/down rate, in deg/sec. Other scaling may affect final rate. */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category=Camera)
	float BaseLookUpRate;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float SprintSpeedRate;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float OriginSpeed;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	uint8 CurrentState;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	bool IsFire;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	bool IsSprint;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float WeaponReboundDistance;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float FireWaitingTime;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float FireWaitingElapsedTime;

	TMap<uint8, FVector> RightHandRelativePosMap;
	FVector LeftHandOffset;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	class AHook* Hook;

	float WireLength;

	float Hp;
protected:

	/** Resets HMD orientation in VR. */
	void OnResetVR();

	/** Called for forwards/backward input */
	void MoveForward(float Value);

	/** Called for side to side input */
	void MoveRight(float Value);

	/** 
	 * Called via input to turn at a given rate. 
	 * @param Rate	This is a normalized rate, i.e. 1.0 means 100% of desired turn rate
	 */
	void TurnAtRate(float Rate);

	/**
	 * Called via input to turn look up/down at a given rate. 
	 * @param Rate	This is a normalized rate, i.e. 1.0 means 100% of desired turn rate
	 */
	void LookUpAtRate(float Rate);

	/** Handler for when a touch input begins. */
	void TouchStarted(ETouchIndex::Type FingerIndex, FVector Location);

	/** Handler for when a touch input stops. */
	void TouchStopped(ETouchIndex::Type FingerIndex, FVector Location);

	void ToggleCrounching(float axis);
	void ToggleFire(float axis);
	void ToggleSprint(float axis);

	UFUNCTION(BlueprintCallable)
	FVector GetAnimDirection() const;

	UFUNCTION(BlueprintCallable)
	bool IsCrounching() const;

	void CalculateRateOfFire(float DeltaSeconds);
	void Shoot();
	void ThrowHook();

	void Zoom(float Axis);

	UFUNCTION(BlueprintCallable)
	FVector CalculateIKHandLocation(bool isRightHands);

	void CalculateWeaponRotation(float DeltaSeconds);


protected:
	// APawn interface
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;
	// End of APawn interface

public:
	/** Returns CameraBoom subobject **/
	FORCEINLINE class USpringArmComponent* GetCameraBoom() const { return CameraBoom; }
	/** Returns FollowCamera subobject **/
	FORCEINLINE class UCameraComponent* GetFollowCamera() const { return FollowCamera; }


	virtual void BeginPlay() override;
	virtual void Tick(float DeltaSeconds) override;

	void FinishWireAction();

	void DragToWire(FVector velocity);
	void PullObject(AActor* actor);

	UFUNCTION()
	void Hit(AActor* DamagedActor, float Damage, const class UDamageType* DamageType, class AController* InstigatedBy, AActor* DamageCauser);
};


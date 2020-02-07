// Copyright 1998-2019 Epic Games, Inc. All Rights Reserved.

#include "TeamIptimeCharacter.h"
#include "HeadMountedDisplayFunctionLibrary.h"
#include "Camera/CameraComponent.h"
#include "Components/CapsuleComponent.h"
#include "Components/InputComponent.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "GameFramework/Controller.h"
#include "GameFramework/SpringArmComponent.h"
#include "Bullet.h"
#include "Blueprint/UserWidget.h"

//////////////////////////////////////////////////////////////////////////
// ATeamIptimeCharacter

ATeamIptimeCharacter::ATeamIptimeCharacter()
{
	PrimaryActorTick.bCanEverTick = true;

	// Set size for collision capsule
	GetCapsuleComponent()->InitCapsuleSize(42.f, 96.0f);

	// set our turn rates for input
	BaseTurnRate = 45.f;
	BaseLookUpRate = 45.f;
	LocalDirection = FVector::ZeroVector;

	// Don't rotate when the controller rotates. Let that just affect the camera.
	bUseControllerRotationPitch = false;
	bUseControllerRotationYaw = true;
	bUseControllerRotationRoll = false;

	GetCharacterMovement()->RotationRate = FRotator(0.0f, 540.0f, 0.0f); // ...at this rotation rate
	GetCharacterMovement()->JumpZVelocity = 600.f;
	GetCharacterMovement()->AirControl = 0.2f;

	// Create a camera boom (pulls in towards the player if there is a collision)
	CameraBoom = CreateDefaultSubobject<USpringArmComponent>(TEXT("CameraBoom"));
	CameraBoom->SetupAttachment(RootComponent);
	CameraBoom->TargetArmLength = 300.0f; // The camera follows at this distance behind the character	
	CameraBoom->bUsePawnControlRotation = false; // Rotate the arm based on the controller
	CameraBoom->bInheritPitch = false;
	CameraBoom->bInheritYaw = true;
	CameraBoom->bInheritRoll = false;

	// Create a follow camera
	FollowCamera = CreateDefaultSubobject<UCameraComponent>(TEXT("FollowCamera"));
	FollowCamera->SetupAttachment(CameraBoom, USpringArmComponent::SocketName); // Attach the camera to the end of the boom and let the boom adjust to match the controller orientation
	FollowCamera->bUsePawnControlRotation = false; // Camera does not rotate relative to arm

	Weapon = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("Weapon"));

	static ConstructorHelpers::FObjectFinder<USkeletalMesh> SM_WEAPON(TEXT("SkeletalMesh'/Game/FPS_Weapon_Bundle/Weapons/Meshes/Ka47/SK_KA47_X.SK_KA47_X'"));
	if (SM_WEAPON.Succeeded())
		Weapon->SetSkeletalMesh(SM_WEAPON.Object);
	Weapon->SetupAttachment(GetMesh(), TEXT("Weapon"));


	GetMesh()->SetRelativeLocation(FVector(0.f, 0.f, -97.f));
	GetMesh()->SetRelativeRotation(FRotator(0.f, -90.f, 0.f));
	GetMesh()->SetAnimationMode(EAnimationMode::AnimationBlueprint);

	GetCharacterMovement()->NavAgentProps.bCanCrouch = true;
	CameraBoom->bEnableCameraLag = true;
	SprintSpeedRate = 1.4f;
	FireWaitingTime = 0.3f;
	WeaponReboundDistance = 20.f;

	CameraBoom->SocketOffset = FVector(0.f, 55.f, 65.f);
	CameraBoom->TargetArmLength = 130.f;

	OriginSpeed = GetCharacterMovement()->MaxWalkSpeed;
	AnimDirection = FVector::ZeroVector;
	
	RightHandRelativePosMap.Add((uint8)ECharacterState::E_IDLE, FVector(6.f, 12.f, 18.f));
	RightHandRelativePosMap.Add((uint8)ECharacterState::E_MOVEMENT, FVector(12.f, 11.f, 21.f));
	RightHandRelativePosMap.Add((uint8)ECharacterState::E_IDLE | (uint8)ECharacterState::E_CROUNCH,
		FVector(20.f, 23.f, 3.f));
	RightHandRelativePosMap.Add((uint8)ECharacterState::E_MOVEMENT | (uint8)ECharacterState::E_CROUNCH,
		FVector(16.f, 20.f, 14.f));
	LeftHandOffset = FVector(8.f, 13.f, -30.f);


	static ConstructorHelpers::FObjectFinder<USkeletalMesh> SM_BODY(TEXT("SkeletalMesh'/Game/AnimStarterPack/UE4_Mannequin/Mesh/SK_Mannequin.SK_Mannequin'"));
	if (SM_BODY.Succeeded())
		GetMesh()->SetSkeletalMesh(SM_BODY.Object);

	static ConstructorHelpers::FClassFinder<UAnimInstance> AB_ANIM(TEXT("AnimBlueprint'/Game/AnimStarterPack/CAnim.CAnim_C'"));
	if (AB_ANIM.Succeeded())
	{
		GetMesh()->SetAnimClass(AB_ANIM.Class);
	}

	static ConstructorHelpers::FClassFinder<UUserWidget> UI_CROSSHAIR(TEXT("WidgetBlueprint'/Game/UserInterface/CrossHair.CrossHair_C'"));
	if (UI_CROSSHAIR.Succeeded())
		CrossHair = UI_CROSSHAIR.Class;

	// Note: The skeletal mesh and anim blueprint references on the Mesh component (inherited from Character) 
	// are set in the derived blueprint asset named MyCharacter (to avoid direct content references in C++)
}

//////////////////////////////////////////////////////////////////////////
// Input

void ATeamIptimeCharacter::SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent)
{
	// Set up gameplay key bindings
	check(PlayerInputComponent);
	//PlayerInputComponent->BindAction("Jump", IE_Pressed, this, &ACharacter::Jump);
	//PlayerInputComponent->BindAction("Jump", IE_Released, this, &ACharacter::StopJumping);

	PlayerInputComponent->BindAxis("MoveForward", this, &ATeamIptimeCharacter::MoveForward);
	PlayerInputComponent->BindAxis("MoveRight", this, &ATeamIptimeCharacter::MoveRight);

	// We have 2 versions of the rotation bindings to handle different kinds of devices differently
	// "turn" handles devices that provide an absolute delta, such as a mouse.
	// "turnrate" is for devices that we choose to treat as a rate of change, such as an analog joystick
	PlayerInputComponent->BindAxis("Turn", this, &APawn::AddControllerYawInput);
	PlayerInputComponent->BindAxis("TurnRate", this, &ATeamIptimeCharacter::TurnAtRate);
	PlayerInputComponent->BindAxis("LookUp", this, &APawn::AddControllerPitchInput);
	PlayerInputComponent->BindAxis("LookUpRate", this, &ATeamIptimeCharacter::LookUpAtRate);

	PlayerInputComponent->BindAxis("Crounch", this, &ATeamIptimeCharacter::ToggleCrounching);
	PlayerInputComponent->BindAxis("Fire", this, &ATeamIptimeCharacter::ToggleFire);
	PlayerInputComponent->BindAxis("Sprint", this, &ATeamIptimeCharacter::ToggleSprint);

	// handle touch devices
	PlayerInputComponent->BindTouch(IE_Pressed, this, &ATeamIptimeCharacter::TouchStarted);
	PlayerInputComponent->BindTouch(IE_Released, this, &ATeamIptimeCharacter::TouchStopped);

	// VR headset functionality
	PlayerInputComponent->BindAction("ResetVR", IE_Pressed, this, &ATeamIptimeCharacter::OnResetVR);
}

void ATeamIptimeCharacter::BeginPlay()
{
	Super::BeginPlay();
	
	CreateWidget(Cast<APlayerController>(GetController()), CrossHair)->AddToPlayerScreen();
}

void ATeamIptimeCharacter::Tick(float DeltaSeconds)
{
	Super::Tick(DeltaSeconds);

	AnimDirection = FMath::Lerp(AnimDirection, LocalDirection, 7.f * DeltaSeconds);

	if (AnimDirection.Size() < 0.1f)
		CurrentState = (uint8)ECharacterState::E_IDLE;
	else
		CurrentState = (uint8)ECharacterState::E_MOVEMENT;

	if (IsCrounching())
		CurrentState |= (uint8)ECharacterState::E_CROUNCH;

	CalculateRateOfFire(DeltaSeconds);
	CalculateWeaponRotation(DeltaSeconds);


	//calc hand offset
	//FTransform parent = GetMesh()->GetSocketTransform(TEXT("rootTRF"), RTS_Component);
	//FVector offsetl = GetMesh()->GetBoneLocation(TEXT("hand_l"), EBoneSpaces::ComponentSpace);
	//FVector offsetr = GetMesh()->GetBoneLocation(TEXT("hand_r"), EBoneSpaces::ComponentSpace);
	
	//FVector outputl = parent.InverseTransformPosition(offsetl);
	//FVector outputr = parent.InverseTransformPosition(offsetr);
	//UE_LOG(TIP, Warning, TEXT("hand_r - %f , %f , %f"), outputl.X, outputl.Y, outputl.Z);
	//UE_LOG(TIP, Warning, TEXT("hand_r - %f , %f , %f"), outputr.X, outputr.Y, outputr.Z);
}


void ATeamIptimeCharacter::OnResetVR()
{
	UHeadMountedDisplayFunctionLibrary::ResetOrientationAndPosition();
}

void ATeamIptimeCharacter::TouchStarted(ETouchIndex::Type FingerIndex, FVector Location)
{
		Jump();
}

void ATeamIptimeCharacter::TouchStopped(ETouchIndex::Type FingerIndex, FVector Location)
{
		StopJumping();
}

void ATeamIptimeCharacter::ToggleCrounching(float axis)
{
	if (axis > 0.f && !GetCharacterMovement()->IsCrouching())
	{
		Crouch();
	}
	else if(axis < 0.0001f && GetCharacterMovement()->IsCrouching())
	{
		UnCrouch();
	}
}

void ATeamIptimeCharacter::ToggleFire(float axis)
{
	if (IsSprint)
	{
		IsFire = false;
		return;
	}

	IsFire = axis > 0.f;
}

void ATeamIptimeCharacter::ToggleSprint(float axis)
{
	if (axis > 0.f && LocalDirection.X > 0.3f && FMath::Abs(LocalDirection.Y) < 0.001f)
	{
		IsSprint = true;
		GetCharacterMovement()->MaxWalkSpeed = SprintSpeedRate * OriginSpeed;
	}
	else
	{
		IsSprint = false;
		GetCharacterMovement()->MaxWalkSpeed = OriginSpeed;
	}

}

FVector ATeamIptimeCharacter::GetAnimDirection() const
{
	return AnimDirection;
}

bool ATeamIptimeCharacter::IsCrounching() const
{
	return GetCharacterMovement()->IsCrouching();
}

void ATeamIptimeCharacter::CalculateRateOfFire(float DeltaSeconds)
{
	if (FireWaitingElapsedTime <= 0.001f)
	{
		if (IsFire)
		{
			Shoot();
			FireWaitingElapsedTime += FireWaitingTime;
		}
	}

	FireWaitingElapsedTime = FMath::Max(FireWaitingElapsedTime - DeltaSeconds, 0.f);
}

void ATeamIptimeCharacter::Shoot()
{
	FHitResult hit;
	FVector start = FollowCamera->GetComponentLocation(), 
		end = FollowCamera->GetComponentLocation() + FollowCamera->GetComponentRotation().Vector() * 3000.f;
	FVector muzzle = Weapon->GetSocketLocation(TEXT("Muzzle"));
	FRotator bulletRot;

	if (GetWorld()->LineTraceSingleByChannel(hit, start, end,
		ECC_GameTraceChannel1, FCollisionQueryParams(NAME_None, false, this)))
	{
		bulletRot = (hit.ImpactPoint - muzzle).GetSafeNormal().ToOrientationRotator();
	}
	else
	{
		bulletRot = (end - muzzle).GetSafeNormal().ToOrientationRotator();
	}

	GetWorld()->SpawnActor<ABullet>(ABullet::StaticClass(), muzzle + bulletRot.Vector() * 20.f, bulletRot);
}

FVector ATeamIptimeCharacter::CalculateIKHandLocation(bool isRightHands)
{
	float percentOfFireWaitingElapsedTime = FireWaitingElapsedTime / FireWaitingTime;
	FTransform parent = GetMesh()->GetSocketTransform(TEXT("rootTRF"), RTS_Component);
	FVector handOffset = RightHandRelativePosMap[CurrentState];

	FVector reboundHead = GetMesh()->GetSocketTransform(TEXT("ReboundHead"), RTS_Component).GetLocation();
	FVector reboundTail = GetMesh()->GetSocketTransform(TEXT("ReboundTail"), RTS_Component).GetLocation();
	FVector reboundDirection = (reboundHead - reboundTail).GetSafeNormal();

	if (!isRightHands)
	{
		handOffset += LeftHandOffset;
	}

	handOffset = parent.TransformPosition(handOffset);

	handOffset += reboundDirection * WeaponReboundDistance * percentOfFireWaitingElapsedTime;

	return handOffset;
}

void ATeamIptimeCharacter::CalculateWeaponRotation(float DeltaSeconds)
{
	if (IsFire)
	{
		FVector lHand = GetMesh()->GetSocketLocation(TEXT("TwoHandedWeapon"));
		FVector rHand = GetMesh()->GetSocketLocation(TEXT("Weapon"));
		FRotator targetRot = (lHand - rHand - FVector(0.f, 0.f, 8.f)).ToOrientationRotator();
		FRotator fixedRot = FMath::RInterpTo(Weapon->GetComponentRotation(), targetRot, DeltaSeconds, 5.f);
		Weapon->SetWorldRotation(fixedRot);
	}
	else
	{
		Weapon->SetRelativeRotation(FRotator::ZeroRotator);
	}
}

void ATeamIptimeCharacter::TurnAtRate(float Rate)
{
	// calculate delta for this frame from the rate information
	AddControllerYawInput(Rate * BaseTurnRate * GetWorld()->GetDeltaSeconds());
}

void ATeamIptimeCharacter::LookUpAtRate(float Rate)
{
	// calculate delta for this frame from the rate information
	AddControllerPitchInput(Rate * BaseLookUpRate * GetWorld()->GetDeltaSeconds());
	CameraBoom->SetRelativeRotation(FRotator(GetControlRotation().Pitch, 0.f, 0.f).Quaternion());
}

void ATeamIptimeCharacter::MoveForward(float Value)
{
	if ((Controller != NULL) && (Value != 0.0f))
	{
		// find out which way is forward
		const FRotator Rotation = Controller->GetControlRotation();
		const FRotator YawRotation(0, Rotation.Yaw, 0);

		// get forward vector
		const FVector Direction = FRotationMatrix(YawRotation).GetUnitAxis(EAxis::X);
		AddMovementInput(Direction, Value);
	}
	LocalDirection.X = Value;
}

void ATeamIptimeCharacter::MoveRight(float Value)
{
	if ( (Controller != NULL) && (Value != 0.0f) )
	{
		// find out which way is right
		const FRotator Rotation = Controller->GetControlRotation();
		const FRotator YawRotation(0, Rotation.Yaw, 0);
	
		// get right vector 
		const FVector Direction = FRotationMatrix(YawRotation).GetUnitAxis(EAxis::Y);
		// add movement in that direction
		AddMovementInput(Direction, Value);
	}
	LocalDirection.Y = Value;

	//Find Bone Pos By AnimSequence Frame

	//UAnimationBlueprintLibrary::GetBonePoseForFrame()
	//AnimationSequence->ExtractBoneTransform(GetRawAnimationTrackByName(AnimationSequence, BoneName), Transform, Time)
	//const int32 TrackIndex = AnimationSequence->AnimationTrackNames.IndexOfByKey(TrackName);
	//return AnimationSequence->GetRawAnimationTrack(TrackIndex);
}

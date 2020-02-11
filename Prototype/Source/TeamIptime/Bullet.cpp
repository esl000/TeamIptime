// Fill out your copyright notice in the Description page of Project Settings.


#include "Bullet.h"
#include "GameFramework/ProjectileMovementComponent.h"

// Sets default values
ABullet::ABullet()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = false;
	InitialLifeSpan = 5.f;

	Body = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Body"));
	Movement = CreateDefaultSubobject<UProjectileMovementComponent>(TEXT("Movement"));
	RootComponent = Body;

	static ConstructorHelpers::FObjectFinder<UStaticMesh> SM_BODY(TEXT("StaticMesh'/Game/Geometry/Meshes/1M_Cube.1M_Cube'"));
	if (SM_BODY.Succeeded())
		Body->SetStaticMesh(SM_BODY.Object);

	Body->SetRelativeScale3D(FVector(0.05f, 0.05f, 0.05f));
	Body->SetNotifyRigidBodyCollision(true);
	Body->SetCollisionProfileName(TEXT("Bullet"));
	Body->OnComponentHit.AddUniqueDynamic(this, &ABullet::OnHit);
	Movement->ProjectileGravityScale = 0.01f;
}

// Called when the game starts or when spawned
void ABullet::BeginPlay()
{
	Super::BeginPlay();
	Movement->SetVelocityInLocalSpace(FVector::ForwardVector * 4000.f);
}

void ABullet::OnHit(UPrimitiveComponent * HitComponent, AActor * OtherActor, UPrimitiveComponent * OtherComp, FVector NormalImpulse, const FHitResult & Hit)
{
	if (ACharacter* target = Cast<ACharacter>(OtherActor))
	{
		target->TakeDamage(10.f, FDamageEvent(), OwnerController, this);
	}
	Destroy();
}

void ABullet::Init(AController * ownerController)
{
	OwnerController = ownerController;
}


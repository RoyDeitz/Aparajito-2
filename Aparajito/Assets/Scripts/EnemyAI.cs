using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //AI
    NavMeshAgent agent;

    public Transform[] waypoints;
    int currentWaypoint;
    //Gravity/Jumping Velocity
    Vector3 verticalVelocity;
    public float jumpHeight = 2f;
    public float gravity = -9.81f; //9.81 m/s²

    //Check if grounded
    public Transform groundCheck;
    public LayerMask groundLayer;
    bool isGrounded;


    //animation
    public Animator anim;

    //movement
    public float movementSpeed = 12f;
    public float runningSpeed;
    public float walkingSpeed;
    public float alertWalkingSpeed;
    Vector3 movementVector;

    //health and death
    public int health;
    public bool isDead=false;
    int deathType;

    public enum AlertLevel 
    {
    Guard,
    Suspicious,
    Alert,
    Combat
    }
    public AlertLevel alertLevel;

    public enum EnemyType 
    {
    Soldier,
    Sniper,
    Officer
    }
    public EnemyType enemyType;

    public enum Weapon 
    {
    Rifle,
    SMG,
    Pistol
    }
    public Weapon weapon;

    //CharacterController enemyController;

    // Detection attributes
    public bool isPlayerFound=false;
    public bool isHeaing;
    public bool isObjFound;

    public Transform eyeTransform;
    public LineRenderer lr;
    public Material safeObjMaterial;
    public Material dangerObjMaterial;
    public Vector3 objPosition;



    //weapon
    public int smgMaxAmmo;
    public int smgCurrentAmmo;
    public int smgMagCapacity;
    public int smgCurrentMag;
    public float smgFiringRate;
    public float smgReloadTime = 1f;
    public int bulletsPerBurst = 6;
    float burstInterval;
    bool isBurstFiring;

    public int rifleMaxAmmo;
    public int rifleCurrentAmmo;
    public int rifleMagCapacity;
    public int rifleCurrentMag;
    public float rifleFiringRate;
    public float rifleReloadTime = 1f;

    public GameObject SniperScope;
    public Transform mainCamera;
    public float scopeDistance=.08f;
    public float scopeAdjustmentHeight = 30f;

    public int rifleDamage = 40;
    public int sniperDamage = 1000;
    public int smgDamage=15;
    public Transform weaponPoint;
    public int bulletRadius;
    public int weaponDamage;
    public int weaponRange=1000;
    public LayerMask playerLayer;

    bool isFiring = false;
    bool isReloading = false;
    float timeTillNextAction;

    public Transform direction;

    EnemyAIGlobal globalAI;


    // audio
    public AudioSource fireSound;
    public AudioSource reloadSound;
    public AudioSource deathSound;


    // Start is called before the first frame update
    void Start()
    {
        globalAI = FindObjectOfType<EnemyAIGlobal>();
        lr = GetComponent<LineRenderer>();
        lr.material = safeObjMaterial;
        //enemyController = GetComponent<CharacterController>();
        SniperScope.SetActive(false);
        if (enemyType != EnemyType.Sniper)
        {
            agent = gameObject.GetComponent<NavMeshAgent>();
            agent.SetDestination(waypoints[currentWaypoint].position);
            if (weapon == Weapon.Rifle || weapon==Weapon.Pistol)
            {
                weaponDamage = rifleDamage;
            }
            else 
            {
                weaponDamage = smgDamage;
            }
        }
        else 
        {
            weaponDamage = sniperDamage;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //isGrounded = Physics.CheckSphere(groundCheck.position, .5f, groundLayer);
        if (!isDead)
        {
            if (alertLevel == AlertLevel.Guard) anim.SetInteger("AlertLevel", 0);
            else if (alertLevel == AlertLevel.Suspicious) anim.SetInteger("AlertLevel", 1);
            else if (alertLevel == AlertLevel.Combat) anim.SetInteger("AlertLevel", 2);

            if (isObjFound)
            {
                lr.positionCount = 2;
                lr.startWidth = 2f;
                lr.endWidth = 10;
                lr.SetPosition(0, eyeTransform.position);
                //lr.SetPosition(1,objPosition);

                if (isPlayerFound)
                {
                    if (!FindObjectOfType<PlayerMovementController>().isDead)
                    {
                        alertLevel = AlertLevel.Combat;
                        lr.material = dangerObjMaterial;
                        GameObject target = new GameObject();
                        target.transform.position = new Vector3(objPosition.x, transform.position.y, objPosition.z);
                        transform.LookAt(target.transform);
                        if (enemyType == EnemyType.Sniper)
                        {
                            Vector3 scopeTarget = new Vector3(objPosition.x, objPosition.y + scopeAdjustmentHeight, objPosition.z);
                            Vector3 direction = mainCamera.position - scopeTarget;
                            SniperScope.SetActive(true);
                            //SniperScope.transform.position = direction *(Vector3.Distance(mainCamera.position,target.transform.position)/ 40);
                            SniperScope.transform.position = scopeTarget + direction * scopeDistance;
                            SniperScope.transform.rotation = mainCamera.rotation;
                            SniperScope.SetActive(true);
                        }
                        lr.SetPosition(1, new Vector3(objPosition.x, objPosition.y + 35, objPosition.z));
                        Destroy(target);

                        //shoot;
                        if (!FindObjectOfType<PlayerMovementController>().isDead)
                        {
                            ShootRifle();
                        }
                    }
                    //InvokeRepeating("ShootRifle",rifleFiringRate,rifleFiringRate+1f);
                }
                else
                {
                    lr.SetPosition(1, objPosition);
                    //alertLevel = AlertLevel.Suspicious;
                    lr.material = safeObjMaterial;
                    SniperScope.SetActive(false);
                    //CancelInvoke();

                }

            }
            else
            {
                SniperScope.SetActive(false);
                //CancelInvoke();
                lr.positionCount = 0;
                lr.material = safeObjMaterial;
            }
            //navigation
            if (enemyType == EnemyType.Soldier)// && isGrounded)
            {
                if (!isPlayerFound)
                {
                    agent.speed = movementSpeed;

                    if (agent.remainingDistance < 1f)
                    {
                        currentWaypoint++;
                        if (currentWaypoint >= waypoints.Length)
                        {
                            currentWaypoint = 0;
                        }
                    }
                 
                    agent.SetDestination(waypoints[currentWaypoint].position);
                    
                }
                else
                {
                    agent.speed = movementSpeed;

                }
                anim.SetFloat("Speed", agent.speed);
            }
            else if (enemyType == EnemyType.Sniper)//&& isGrounded)
            {
                if (!isPlayerFound && (!isReloading || !isFiring))
                {
                    transform.forward = direction.forward;
                }
            }
            //Shooting
            if (isFiring || isReloading)
            {
                movementSpeed = 0f;
                // anim.SetFloat("Speed", 0f);

                if (isFiring)
                {
                    if (weapon == Weapon.Rifle)
                    {
                        if (timeTillNextAction >= 0)
                        {
                            timeTillNextAction -= Time.deltaTime;
                        }
                        else
                        {
                            isFiring = false;
                            if (rifleCurrentMag <= 0) ReloadRifle();
                        }
                    }
                }
                else if (isReloading)
                {
                    if (timeTillNextAction >= 0)
                    {
                        timeTillNextAction -= Time.deltaTime;
                    }
                    else
                    {
                        isReloading = false;
                    }
                }
            }
            else 
            {
                movementSpeed = walkingSpeed;
            }

        }
        else
        {// death condition
            if (enemyType == EnemyType.Soldier || enemyType == EnemyType.Officer) agent.speed = 0f;
            lr.positionCount = 0;
        }
    }

    public void TakeDamage(int damage) 
    {
        if (!isDead)
        {
            health -= damage;
            
            if (health <= 0)
            {
                isDead = true;
                anim.SetBool("IsDead", true);
                anim.SetInteger("DeathType", 1);
                anim.SetTrigger("IsDeadTrigger");
                deathSound.Play();
            }
            else 
            {
                anim.SetTrigger("IsHit");
            }
        }
    }

    public void TakeDamageWithDeathtype(int damage, int deathType)
    {
        if (!isDead)
        {
            health -= damage;

            if (health <= 0)
            {
                isDead = true;
                anim.SetBool("IsDead", true);
                anim.SetInteger("DeathType", deathType);
                anim.SetTrigger("IsDeadTrigger");
                deathSound.Play();
            }
            else 
            {
                anim.SetTrigger("IsHit");
            }
        }
    }
    public void ShootRifle()
    {
        if (!isDead)
        {
            if (!isFiring && !isReloading && weapon == Weapon.Rifle)
            {
                if (rifleCurrentMag > 0)
                {
                    isFiring = true;
                    
                    timeTillNextAction = rifleFiringRate;
                    anim.SetTrigger("ShootRifle");
                    rifleCurrentMag -= 1;
                    
                    Vector3 direction = (objPosition - weaponPoint.position);
                    RaycastHit hit;
                    if (Physics.SphereCast(weaponPoint.position,bulletRadius, direction, out hit,weaponRange, playerLayer))
                    {
                        if (hit.collider.GetComponent<PlayerMovementController>() != null)
                        {
                            hit.collider.GetComponent<PlayerMovementController>().TakeDamageWithDeathType(weaponDamage, 2);
                            Debug.DrawLine(weaponPoint.position, hit.point);
                        }
                    }
                    

                    fireSound.Play();
                }
                else
                {
                    ReloadRifle();
                }
            }
        }
    }
    public void ReloadRifle()
    {
        if (!isDead)
        {
            if (rifleCurrentMag < rifleMagCapacity)
            {
                if (rifleCurrentAmmo > 0)
                {
                    if (!isReloading)
                    {
                        isReloading = true;
                        timeTillNextAction = rifleReloadTime;
                        anim.SetTrigger("ReloadRifle");
                        reloadSound.Play();

                        if (rifleCurrentAmmo > rifleMagCapacity)
                        {
                            rifleCurrentAmmo -= (rifleMagCapacity - rifleCurrentMag);
                            rifleCurrentMag += (rifleMagCapacity - rifleCurrentMag);
                        }
                        else
                        {
                            rifleCurrentMag = rifleCurrentAmmo;
                            rifleCurrentAmmo = 0;
                        }
                    }

                }
                else
                {
                    //dry fire or switch weapon
                    rifleCurrentAmmo = rifleMaxAmmo;
                }
            }
        }

    }
}

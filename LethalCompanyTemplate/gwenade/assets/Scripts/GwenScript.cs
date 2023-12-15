using GameNetcodeStuff;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace GwenMod
{
    public class GwenScript : GrabbableObject
    {
        public AudioSource gwenAudio;
        [SerializeField] public AudioClip[] clips;
        [SerializeField] public gwenData gwenDatatest;
        public AudioClip explosionsfx;

        public Ray grenadeThrowRay;

        public RaycastHit grenadeHit;
        private System.Random noisemakerRandom;

        public float explodeTimer;

        public AnimationCurve itemFallCurve;
        public AnimationCurve itemVerticalFallCurve;
        public GameObject gwenadeExplosion;
        public AnimationCurve itemVerticalFallCurveNoBounce;

        private bool thrown = false;
        private bool exploded = false;


        public void Awake()
        {
            var scNode = GetComponentInChildren<ScanNodeProperties>();
            gwenAudio = GetComponent<AudioSource>();
            //curveGen();
            Debug.Log("Audio Clips Test");
            Debug.Log(gwenDatatest.name);
            scNode.minRange = 1;
            scNode.maxRange = 13;
            scNode.headerText = "Gweny Plush";
            scNode.subText = "Value:";
            scNode.creatureScanID = -1;
            scNode.nodeType = 2;

            grabbable = true;
        }

        public override void Start()
        {
            base.Start();
            noisemakerRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85);
        }
        public override void Update()
        {
            base.Update();
            if (thrown && !exploded)
            {
                explodeTimer += Time.deltaTime;
                if (explodeTimer > 2.25f)
                {
                    if (base.IsHost)
                    {
                        DetonateClientRPC();
                    }
                    else
                    {
                        DetonateServerRPC();
                    }
                    //Plugin.logger.LogInfo("Explode uwu");
                    exploded = true;
                }
            }
        }
        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            Debug.Log("test lol");
            base.ItemActivate(used, buttonDown);
            thrown = true;
            if (base.IsOwner)
            {
                playerHeldBy.DiscardHeldObject(true, null, GetThrowDestination());
            }
        }
        public void Detonate(Vector3 explosionPosition, float maxDamageRange = 10f, float minDamageRange = 0f)
        {
            if (exploded)
            {
                return;
            }
            exploded = true;
            UnityEngine.Object.Instantiate(parent: (!isInElevator) ? RoundManager.Instance.mapPropsContainer.transform : StartOfRound.Instance.elevatorTransform, original: gwenadeExplosion, position: base.transform.position, rotation: Quaternion.identity);
            Debug.Log("Spawning explosion at pos: {explosionPosition}");
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts.FirstOrDefault((PlayerControllerB x) => x.OwnerClientId == this.OwnerClientId);
            float num = Vector3.Distance(GameNetworkManager.Instance.localPlayerController.transform.position, explosionPosition);
            if (num < 14f)
            {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
            }
            else if (num < 25f)
            {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
            }
            Collider[] array = Physics.OverlapSphere(explosionPosition, 7.5f, 2621448, QueryTriggerInteraction.Collide);
            PlayerControllerB playerControllerB = null;
            for (int i = 0; i < array.Length; i++)
            {
                float num2 = Vector3.Distance(explosionPosition, array[i].transform.position);
                if (num2 > 4f && Physics.Linecast(explosionPosition, array[i].transform.position + Vector3.up * 0.3f, 256, QueryTriggerInteraction.Ignore))
                {
                    continue;
                }
                if (array[i].gameObject.layer == 3)
                {
                    playerControllerB = array[i].gameObject.GetComponent<PlayerControllerB>();
                    if (playerControllerB != null && playerControllerB.IsOwner)
                    {
                        //if (num2 < killRange)
                        //{
                        //    Vector3 bodyVelocity = (playerControllerB.gameplayCamera.transform.position - explosionPosition) * 80f / Vector3.Distance(playerControllerB.gameplayCamera.transform.position, explosionPosition);
                        //    playerControllerB.KillPlayer(bodyVelocity, spawnBody: true, CauseOfDeath.Blast);
                        //
                        //    GameNetworkManager.Instance.localPlayerController.KillPlayer(bodyVelocity, spawnBody: true, CauseOfDeath.Blast);
                        //}
                        //else if (num2 < damageRange)
                        //{
                        //    GameNetworkManager.Instance.localPlayerController.DamagePlayer(20, hasDamageSFX: false, callRPC: true, CauseOfDeath.Blast);
                        //}
                        float damageMultiplier = (num2 - minDamageRange) / (maxDamageRange - minDamageRange)/10f;
                        //Plugin.logger.LogInfo($"Gwenade Damage {(int)(20f * damageMultiplier)}");
                        int gwenDamage = Mathf.Max((int)(20f * damageMultiplier), 0);
                        playerControllerB.DamagePlayer(gwenDamage, false, true, CauseOfDeath.Blast, 0, false, default(Vector3));
                    }
                }
                else if (array[i].gameObject.layer == 19)
                {
                    EnemyAICollisionDetect componentInChildren2 = array[i].gameObject.GetComponentInChildren<EnemyAICollisionDetect>();
                    if (componentInChildren2 != null && componentInChildren2.mainScript.IsOwner && num2 < 4.5f)
                    {
                        componentInChildren2.mainScript.HitEnemyOnLocalClient(10, default, player, false);
                    }
                }
            }
            int num3 = ~LayerMask.GetMask("Room");
            num3 = ~LayerMask.GetMask("Colliders");
            array = Physics.OverlapSphere(explosionPosition, 10f, num3);
            for (int j = 0; j < array.Length; j++)
            {
                Rigidbody component = array[j].GetComponent<Rigidbody>();
                if (component != null)
                {
                    component.AddExplosionForce(70f, explosionPosition, 10f);
                }
            }

            if (this.IsHost)
            {
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }

        [ClientRpc]
        public void DetonateClientRPC()
        {
            Detonate(base.transform.position, 5.7f, 6.4f);
        }

        [ServerRpc(RequireOwnership = false)]
        public void DetonateServerRPC()
        {
            DetonateClientRPC();
        }

        public override void EquipItem()
        {
            base.EquipItem();
            base.playerHeldBy.equippedUsableItemQE = true;
        }
        public override void ItemInteractLeftRight(bool right)
        {
            int num = noisemakerRandom.Next(0, clips.Length);
            base.ItemInteractLeftRight(right);
            if (right)
            {
                if (!gwenAudio.isPlaying)
                {
                    gwenAudio.PlayOneShot(clips[num]);
                }
            }
            else
            {
                Debug.Log("not right");
                gwenAudio.Play();
            }
        }
        public Vector3 GetThrowDestination()
        {
            Vector3 position = base.transform.position;
            Debug.DrawRay(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward, Color.yellow, 15f);
            grenadeThrowRay = new Ray(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward);
            position = ((!Physics.Raycast(grenadeThrowRay, out grenadeHit, 12f, StartOfRound.Instance.collidersAndRoomMaskAndDefault)) ? grenadeThrowRay.GetPoint(10f) : grenadeThrowRay.GetPoint(grenadeHit.distance - 0.05f));
            Debug.DrawRay(position, Vector3.down, Color.blue, 15f);
            grenadeThrowRay = new Ray(position, Vector3.down);
            if (Physics.Raycast(grenadeThrowRay, out grenadeHit, 30f, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
            {
                return grenadeHit.point + Vector3.up * 0.05f;
            }
            return grenadeThrowRay.GetPoint(30f);
        }

        public override void FallWithCurve()
        {
            float magnitude = (startFallingPosition - targetFloorPosition).magnitude;
            base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(itemProperties.restingRotation.x, base.transform.eulerAngles.y, itemProperties.restingRotation.z), 14f * Time.deltaTime / magnitude);
            base.transform.localPosition = Vector3.Lerp(this.startFallingPosition, this.targetFloorPosition, this.itemFallCurve.Evaluate(this.fallTime));
            base.transform.localPosition = Vector3.Lerp(new Vector3(base.transform.localPosition.x, this.startFallingPosition.y, base.transform.localPosition.z), new Vector3(base.transform.localPosition.x, this.targetFloorPosition.y, base.transform.localPosition.z), this.itemVerticalFallCurve.Evaluate(this.fallTime));
            this.fallTime += Mathf.Abs(Time.deltaTime * 12f / magnitude);
        }
        //public void curveGen()
        //{
        //   itemFallCurve = new AnimationCurve(new Keyframe(0, 0, 2, 2), new Keyframe(1, 1));
        //   itemVerticalFallCurve = new AnimationCurve(new Keyframe(0, 0, .1169f, .1169f,0,.2723f), new Keyframe(.5f, 1, 4f, -2f,0f,.3f),new Keyframe(.75f,1,1.4f,-1.4f,.3f,.6f), new Keyframe(1,1,.9f,0,.5f,1f));
        //   itemVerticalFallCurveNoBounce = AnimationCurve.Linear(0,0,1,1);
        //}

    }

}
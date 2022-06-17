using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
namespace Valve.VR.InteractionSystem
{
    public class GrabChecker : MonoBehaviour
    {
        //
        [SerializeField] GameManager GM;
        //Audio code

        public AudioSource _audio;

        public float sensitivity = 100;
        public float loudness = 0;
        public float SoundtoHaptic;

        //여기 만짐
        public GameObject holder;
        public Color color;
        public Color clickColor = Color.green;
        public GameObject pointer;
        public float thickness = 0.002f;
        public bool addRigidBody = false;
        //여기까지




        //VR Code

        public SteamVR_Input_Sources handType;
        //        public GameObject temp;
        public float duration;
        public float frequency;
        public float amplitude;
        [SerializeField]
        private AudioSource Fire;

        [SerializeField]
        private GameObject effect;

        //  public GameObject make;
        public SteamVR_Action_Vibration hapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic");
        public SteamVR_Action_Boolean grabPinchAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");
        // Start is called before the first frame update
        void Start()
        {
            //_audio = GetComponent<AudioSource>();
            holder = new GameObject();
            holder.transform.parent = this.transform;
            holder.transform.localPosition = Vector3.zero;
            holder.transform.localRotation = Quaternion.identity;

            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.transform.parent = holder.transform;
            pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
            pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
            pointer.transform.localRotation = Quaternion.identity;
            BoxCollider collider = pointer.GetComponent<BoxCollider>();
            if (addRigidBody)
            {
                if (collider)
                {
                    collider.isTrigger = true;
                }
                Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
            }
            else
            {
                if (collider)
                {
                    Object.Destroy(collider);
                }
            }
            Material newMaterial = new Material(Shader.Find("Unlit/Color"));
            newMaterial.SetColor("_Color", color);
            pointer.GetComponent<MeshRenderer>().material = newMaterial;
        }

        // Update is called once per frame
        void Update()
        {
            float dist = 100f;
            loudness = GetAveragedVolume() * sensitivity;
            if (loudness > SoundtoHaptic)
            {
                hapticAction.Execute(0, 0.1f, 1, loudness * 0.1f, handType);
            }

            if (grabPinchAction.GetStateDown(handType))
            {
                pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);
                pointer.GetComponent<MeshRenderer>().material.color = clickColor;

                Fire.Play();
                hapticAction.Execute(0, duration, frequency, amplitude, handType);
                RaycastHit hit;
                //Ray ray = new Ray(this.transform.position,this.transform.up*-1);
                Ray ray = new Ray(this.transform.position, pointer.transform.forward);
                ray.GetPoint(dist);
                //Debug.DrawRay(this.transform.position, this.transform.up*-1*1000f, Color.red, 5f);
                Debug.DrawRay(this.transform.position, pointer.transform.forward * 1000f, Color.red, 5f);

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.collider.name);
                    if (hit.collider.tag == "Item")
                    {
                        Debug.Log(hit.transform.GetComponent<ChildItemScore>().score + "/ " + hit.transform.parent.name + " : " + hit.transform.name);
                        GM.currScore = GM.currScore + hit.transform.GetComponent<ChildItemScore>().score;
                        hit.collider.gameObject.GetComponent<AudioSource>().Play();

                        //effect.GetComponent<TextMesh>().text = "+";
                        Vector3 LookDir = this.transform.position - hit.point;
                        Quaternion rotation = Quaternion.LookRotation(LookDir, Vector3.forward);
                        GameObject a =
                        Instantiate(effect, hit.transform);

                        if(hit.collider.name == "Balloon_item1" || hit.collider.name == "HorrorTarget_item1")
                        {
                            Destroy(hit.transform.parent.gameObject.transform.parent.gameObject, 0.3f);
                        }
                        else
                        {
                            Destroy(hit.transform.parent.gameObject, 0.3f);
                        }
                        

                        Destroy(a, 0.8f);
                    }
                    else if (hit.collider.tag == "Zombie")
                    {
                        Debug.Log("Hit Zombie:"+hit.collider.name);
                        GM.currScore = GM.currScore + hit.transform.GetComponent<ItemInfo>().score;
                        if(hit.collider.name == "pk"){
                            hit.collider.gameObject.GetComponent<PumpkinActiveController>().ApplyDamage(50);
                        }else hit.collider.gameObject.GetComponent<DefenseController>().ApplyDamage(50);
                    }
                    else if(hit.collider.name == "End")
                    {
                        DestroyApp();
                    }
                    else if (hit.collider.name == "Restart")
                    {
                        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        SceneManager.LoadSceneAsync("LoadingScene");
                    }
                }
            }
            else
            {
                pointer.transform.localScale = new Vector3(0, 0, 0);
                pointer.GetComponent<MeshRenderer>().material.color = color;
            }

            /*
             *  if (interactWithUI != null && interactWithUI.GetState(pose.inputSource))
            {
                pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);
                pointer.GetComponent<MeshRenderer>().material.color = clickColor;
            }
            else
            {
                pointer.transform.localScale = new Vector3(thickness, thickness, dist);
                pointer.GetComponent<MeshRenderer>().material.color = color;
            }
             */


        }

        void DestroyApp()
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
            Application.Quit();

        }
        float GetAveragedVolume()
        {
            float[] data = new float[64];
            float a = 0;
            _audio.GetOutputData(data, 0);
            foreach (float s in data)
            {
                a += Mathf.Abs(s);
            }
            return a / 256;
        }
    }
}
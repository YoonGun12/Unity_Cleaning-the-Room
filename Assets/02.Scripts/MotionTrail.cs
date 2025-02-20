using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTrail : MonoBehaviour
{
    [Header("잔상 효과가 활성화 되는 시간")]
    [SerializeField] private float activeTime = 2f;
    
    [Header("메쉬 생성 주기 (잔상이 얼마나 자주 생성 될지 결정")] 
    [SerializeField] private float meshRefreshRate = 0.1f;
    
    [Header("생성된 매쉬가 사라지기까지의 시간")] 
    [SerializeField] private float meshDestoryDelay = 3f;
    
    [Header("잔상 오브젝트가 생성될 위치와 회전 정보")]
    [SerializeField] private Transform positionToSpawn;

    [Header("잔상 효과에 사용할 머티리얼")]
    [SerializeField] private Material mat;
    
    [Header("쉐이더 변수의 이름")]
    [SerializeField] private string shaderVarRef;
    
    [Header("쉐이더 변수값이 감소하는 속도")]
    [SerializeField] private float shaderVarRate = 0.1f;
    
    [Header("쉐이더 값이 갱신되는 주기")]
    [SerializeField] private float shaderVarRefreshRate = 0.05f;
    
    private bool isTrailActive;//잔상효과 활성화 여부
    private SkinnedMeshRenderer[] skinnedMeshRenderers;//플레이어의 스킨드 메쉬 렌더러 배열

    public void StartMotionTrail()
    {
        isTrailActive = true;
        StartCoroutine(ActiveTrail(activeTime));
    }

    IEnumerator ActiveTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate; //잔상 효과 지속 시간 감소

            if (skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            //각 매쉬렌더러에 대해 잔상 생성
            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject gObj = new GameObject("TrailObject");
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();//새로운 메쉬 생성
                skinnedMeshRenderers[i].BakeMesh(mesh); //현재 스킨드 메쉬를 복제하여 메쉬로 bake

                mf.mesh = mesh;
                mr.material = mat;

                SphereCollider collider = gObj.AddComponent<SphereCollider>();
                collider.isTrigger = true;
                collider.radius = 0.5f;
                
                //쉐이더 변수를 애니메이션으로 변화시키는 코루틴 실행
                StartCoroutine(AnimatedMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));
                
                Destroy(gObj, meshDestoryDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;

    }
    

    IEnumerator AnimatedMaterialFloat(Material mat, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = mat.GetFloat(shaderVarRef);

        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }

    

   
}

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
    private List<Vector3> trailPosition = new List<Vector3>();
    
    private void Update()
    {
        if (Input.GetMouseButton(1) && !isTrailActive)
        {
            isTrailActive = true;
            trailPosition.Clear();
            StartCoroutine(ActiveTrail(activeTime));
        }

        if (Input.GetMouseButtonUp(1))
        {
            CheckObjectsInTrail();
        }
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

                collider.enabled = false;
                StartCoroutine(EnableColliderAfterDelay(collider, 1f));

                trailPosition.Add(positionToSpawn.position);
                
                //쉐이더 변수를 애니메이션으로 변화시키는 코루틴 실행
                StartCoroutine(AnimatedMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));
                
                Destroy(gObj, meshDestoryDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;

    }

    IEnumerator EnableColliderAfterDelay(Collider collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        collider.enabled = true;
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

    private void CheckObjectsInTrail()
    {
        if (trailPosition.Count < 3) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 50f);

        foreach (var col in colliders)
        {
            if (IsPointInPolygon(trailPosition, col.transform.position))
            {
                if (!col.CompareTag("Player"))
                {
                    Destroy(col.gameObject);
                }
            }
        }
    }

    private bool IsPointInPolygon(List<Vector3> polygon, Vector3 point)
    {
        int crossings = 0;
        for (int i = 0; i < polygon.Count; i++)
        {
            Vector3 a = polygon[i];
            Vector3 b = polygon[(i + 1) % polygon.Count];

            if ((a.z > point.z) != (b.z > point.z) && (point.x < (b.x = a.x) * (point.z - a.z) / (b.z - a.z) + a.x))
            {
                crossings++;
            }
        }

        return (crossings % 2 == 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("TrailObject"))
        {
            Debug.Log("잔상과 충돌");
            CheckObjectsInTrail();
        }
    }
}

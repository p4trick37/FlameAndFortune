using UnityEngine;

public class PSMesh : MonoBehaviour
    {
        [SerializeField] private ParticleSystem ps;
        [SerializeField] private MeshFilter targetMesh;

        private void Update()
        {
            ParticleSystem.ShapeModule shapeModule = ps.shape;
            shapeModule.mesh = targetMesh.mesh;
        }
    }

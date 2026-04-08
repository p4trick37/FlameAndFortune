using UnityEngine;

public class PSMesh : MonoBehaviour
    {
        [SerializeField] private ParticleSystem ps;
        [SerializeField] private MeshFilter targetMesh;

        private void Update()
        {
            
        }

        public void SetMesh (MeshFilter meshFilter)
        
        {
        ParticleSystem.ShapeModule shapeModule = ps.shape;
            shapeModule.mesh = meshFilter.mesh;
        }
    }

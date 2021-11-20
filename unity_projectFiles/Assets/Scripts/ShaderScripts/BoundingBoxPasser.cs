using UnityEngine;

namespace ShaderScripts
{
    public class BoundingBoxPasser : MonoBehaviour
    {
        // Start is called before the first frame update
        BoxCollider _boxCollider;

        Material _material; 
        void Start()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(_boxCollider);
        }
    }
}

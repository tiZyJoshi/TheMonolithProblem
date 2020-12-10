using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceContainer : MonoBehaviour
{
    public string Name;

    private Rigidbody _rigidbody;
    private List<(SpringJoint spring, LineRenderer lineRenderer)> _springs = new List<(SpringJoint, LineRenderer)>();

    void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    public void Init(Service service, List<ServiceContainer> serviceContainer, LineRenderer lineRendererPrefab)
    {
        Name = service.Name;
        foreach (var (index, number) in service.Dependencies)
        {
            var force = 1.0f + Mathf.Log(1.0f + Mathf.Log(number));
            var connectedGameObject = serviceContainer[index].gameObject;
            
            var spring = AddSpring(gameObject, connectedGameObject, force * 0.1f);
            var lineRenderer = Instantiate(lineRendererPrefab, transform);
            _springs.Add((spring, lineRenderer)); 
        }
    }

    public void AddForce(Vector3 force)
    {
        _rigidbody.AddForce(force);
    }

    private static SpringJoint AddSpring(GameObject gameObject, GameObject connectedGameObject, float force)
    {
        var spring = gameObject.AddComponent<SpringJoint>();
        spring.autoConfigureConnectedAnchor = false;
        spring.spring = force;
        spring.anchor = Vector3.zero;
        spring.connectedAnchor = Vector3.zero;
        spring.connectedBody = connectedGameObject.GetComponent<ServiceContainer>()._rigidbody;
        spring.enableCollision = false;
        spring.maxDistance = 50.0f;
        spring.enablePreprocessing = false;

        return spring;
    }

    void Update()
    {
        foreach (var (spring, lineRenderer) in _springs)
        {
            lineRenderer.SetPositions(new[] {_rigidbody.position, spring.connectedBody.position});
            var currentForce = spring.currentForce;
            lineRenderer.startColor = new Color(currentForce.x, currentForce.y, currentForce.z, 1.0f);
            lineRenderer.endColor = new Color(currentForce.x, currentForce.y, currentForce.z, 1.0f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        //if (collision.gameObject.GetComponent<ServiceContainer>())
        //{
        //    collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.relativeVelocity.normalized * 10f);
        //}
    }
}

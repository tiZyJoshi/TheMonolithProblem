using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceContainer : MonoBehaviour
{
    public string Name;

    public void Init(Service service, List<ServiceContainer> serviceContainer)
    {
        Name = service.Name;

        

        foreach (var (index, number) in service.Calling)
        {
            AddSpring(index, number, serviceContainer, 0.01f);
        }
        foreach (var (index, number) in service.CalledBy)
        {
            //AddSpring(index, number, serviceContainer, 1.0f);
        }
        foreach (var (index, number) in service.CommonChanges)
        {
            AddSpring(index, number, serviceContainer, 0.01f);
        }
    }
    
    private void AddSpring(int index, int number, List<ServiceContainer> serviceContainer, float factor)
    {
        var spring = gameObject.AddComponent<SpringJoint>();
        spring.autoConfigureConnectedAnchor = false;
        spring.spring = (float)Math.Sqrt(number) * factor;
        spring.anchor = Vector3.zero;
        spring.connectedAnchor = Vector3.zero;
        spring.connectedBody = serviceContainer[index].gameObject.GetComponent<Rigidbody>();
        spring.enableCollision = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.GetComponent<ServiceContainer>())
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.relativeVelocity.normalized * 10f);
        }
    }
}

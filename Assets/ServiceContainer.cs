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
            AddSpring(index, number, serviceContainer, 1.0f);
        }
        foreach (var (index, number) in service.CalledBy)
        {
            //AddSpring(index, number, serviceContainer, 1.0f);
        }
        foreach (var (index, number) in service.CommonChanges)
        {
            //AddSpring(index, number, serviceContainer, 1.0f);
        }
    }

    private void AddSpring(int index, int number, List<ServiceContainer> serviceContainer, float factor)
    {
        var spring = gameObject.AddComponent<SpringJoint>();
        spring.autoConfigureConnectedAnchor = false;
        spring.spring = number * factor;
        spring.anchor = Vector3.zero;
        spring.connectedAnchor = Vector3.zero;
        spring.connectedBody = serviceContainer[index].gameObject.GetComponent<Rigidbody>();
        spring.enableCollision = true;
    }
}

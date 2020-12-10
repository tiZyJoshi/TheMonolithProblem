using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

public class DataLoader : MonoBehaviour
{
    public ServiceContainer ServicePrefab;
    public LineRenderer LineRendererPrefab;

    public int ClusterCount = 10;
    public int Iterations = 100;
    public float ForceScale = 10.0f;

    private List<Service> Services;
    private List<ServiceContainer> ServiceContainers;

    // Start is called before the first frame update
    void Awake()
    {
        var serializer = new XmlSerializer(typeof(List<Service>));
        using (var reader = new FileStream("Assets/data.xml", FileMode.Open))
        {
            Services = serializer.Deserialize(reader) as List<Service>;
            reader.Close();
        }
        if (Services is null)
        {
            throw new InvalidOperationException();
        }

        ServiceContainers = Services
            .Select((s, index) => Instantiate(ServicePrefab))
            .ToList();

        for (var i = 0; i < Services.Count; i++)
        {
            ServiceContainers[i].Init(Services[i], ServiceContainers, LineRendererPrefab);
        }

        ServiceContainers.Explode(1000.0f);
    }

    // Update is called once per frame
    private bool running = true;
    private float timeScaleBackup;
    private bool clusterized = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ServiceContainers.Explode(ForceScale);
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            var itemsTmp = ServiceContainers.Select(c => c.transform.position).ToArray();
            var resultTmp = KMeans.Cluster(itemsTmp, ClusterCount, Iterations, 0);
            foreach (var cluster in resultTmp.clusters)
            {
                var force = Random.onUnitSphere * ForceScale;
                cluster.Select(i => ServiceContainers[i]).AddForce(force);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (var spring in ServiceContainers.SelectMany(s => s.gameObject.GetComponents<SpringJoint>()))
            {
                spring.maxDistance = 3.0f;
                spring.damper = 1.0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (running)
            {
                timeScaleBackup = Time.timeScale;
                Time.timeScale = 0;
                running = false;
            }
            else
            {
                Time.timeScale = timeScaleBackup;
                running = true;
                clusterized = false;
            }
        }

        if (running || clusterized) return;

        var items = ServiceContainers.Select(c => c.transform.position).ToArray();
        var result = KMeans.Cluster(items, ClusterCount, Iterations, 0);

        for (var i = 0; i < result.clusters.Length; i++)
        {
            var color = Color.HSVToRGB(1f * i / result.clusters.Length, 1f, 1f);
            for (var j = 0; j < result.clusters[i].Length; j++)
            {
                var index = result.clusters[i][j];
                ServiceContainers[index].GetComponent<MeshRenderer>().material.color = color;
            }
        }

        clusterized = true;
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.blue;
        if (ServiceContainers is null)
        {
            return;
        }

        foreach (var serviceContainer in ServiceContainers)
        {
            foreach (var spring in serviceContainer.GetComponents<SpringJoint>())
            {
                if (spring != null)
                {
                    Gizmos.DrawLine(spring.gameObject.transform.position, spring.connectedBody.position);
                }
            }
        }
    }
}

public class Service
{
    public string Name { get; set; }
    public List<(int index, int number)> Dependencies { get; set; }
}

public static class ServicesExtensions
{
    public static void Explode(this IEnumerable<ServiceContainer> serviceContainers, float force)
    {
        foreach (var serviceContainer in serviceContainers)
        {
            serviceContainer.AddForce(Random.onUnitSphere * force);
        }
    }

    public static void AddForce(this IEnumerable<ServiceContainer> serviceContainers, Vector3 force)
    {
        foreach (var serviceContainer in serviceContainers)
        {
            serviceContainer.AddForce(force);
        }
    }
}
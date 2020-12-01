using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    public GameObject ServicePrefab;

    public int ClusterCount = 10;
    public int Iterations = 100;

    private List<Service> Services;
    private List<ServiceContainer> ServiceContainer;

    // Start is called before the first frame update
    void Start()
    {
        var serializer = new XmlSerializer(typeof(List<Service>));
        using (var reader = new FileStream("Assets/data.xml", FileMode.Open))
        {
            Services = serializer.Deserialize(reader) as List<Service>;
        }
        if (Services is null)
        {
            throw new InvalidOperationException();
        }

        var side = (int)Math.Ceiling(Math.Sqrt(Services.Count));

        ServiceContainer = Services
            .Select((_, index) => Instantiate(ServicePrefab, new Vector3(index/side, index%side), Quaternion.identity))
            .Select(go => go.GetComponent<ServiceContainer>())
            .ToList();

        

        for (var i = 0; i < Services.Count; i++)
        {
            ServiceContainer[i].Init(Services[i], ServiceContainer);
        }
    }

    // Update is called once per frame
    private bool running = true;
    private float timeScaleBackup;
    private bool clusterized = false;
    void Update()
    {
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

        var items = ServiceContainer.Select(c => c.transform.position).ToArray();
        var result = KMeans.Cluster(items, ClusterCount, Iterations, 0);

        for (var i = 0; i < result.clusters.Length; i++)
        {
            var color = Color.HSVToRGB(1f * i / result.clusters.Length, 1f, 1f);
            for (var j = 0; j < result.clusters[i].Length; j++)
            {
                var index = result.clusters[i][j];
                ServiceContainer[index].GetComponent<MeshRenderer>().material.color = color;
            }
        }

        clusterized = true;
    }
}

public class Service
{
    public string Name { get; set; }
    public List<(int index, int number)> Calling { get; set; }
    public List<(int index, int number)> CalledBy { get; set; }
    public List<(int index, int number)> CommonChanges { get; set; }
}

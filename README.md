# TheMonolithProblem

Trying to solve the Monolith Problem as discussed in the FH Technikum Wien LVA SWA.
Services are represented as spheres. The dependencies between services are modeled as springs between those services. The strength of the spring is defined by the number of dependencies between the services. Let Unity Physics handle the rest.

Let the simulation run until you are happy with the result and hit space.
The services are clusterd by a kmeans clustering implementation found on github.
[https://github.com/ogxd/kmeans-clustering-unity](https://github.com/ogxd/kmeans-clustering-unity) 
and each cluster gets a unique color.

You can add (random) force to each sphere by hitting f. 
Hitting c will calculate clusters and add the same amount of (random) force to each member of a cluster
(Once, this worked, currently, it does not)

# Screenshots:

![alt text](https://github.com/tiZyJoshi/TheMonolithProblem/blob/main/Screenshots/Screenshot%202020-12-01%20225332.png "")
![alt text](https://github.com/tiZyJoshi/TheMonolithProblem/blob/main/Screenshots/Screenshot%202020-12-01%20225414.png "")
![alt text](https://github.com/tiZyJoshi/TheMonolithProblem/blob/main/Screenshots/Screenshot%202020-12-01%20225437.png "")
![alt text](https://github.com/tiZyJoshi/TheMonolithProblem/blob/main/Screenshots/Screenshot%202020-12-01%20230639.png "")
![alt text](https://github.com/tiZyJoshi/TheMonolithProblem/blob/main/Screenshots/Screenshot%202020-12-01%20231248.png "")

# Notes

This is by no means a very good or correct implementation of the idea. It's a prototype to see if it could work and what it would take to implement this in a more serious fashion.
Current answers: "maybe" and "a lot of optimizing springs and the rest of the simulation parameters".

Also see https://github.com/tiZyJoshi/ExcelDataLoader for xlsx to xml conversion (original data was in xlsx format)

Ideas for improvement:
+ evaluate how the service data is mapped to the springs and find a better model.
+ Use different springs for better modellation possibilities of the different dependencies. Currently they only pull and have some dampening (pretty much unity Spring Joint defaults https://docs.unity3d.com/Manual/class-SpringJoint.html)
+ Use a better method to find initial service positions and velocities
+ better data = better results => I think you could improve the data a lot by adding a few data entries in the original data tables. Eg: Common Changes in relation to total changes. Currently, there are 60K dataentries regarding common changes, but 40k of them have 1 common change and another 10k or so have 2. For performance reasons I ignored those, but this can hide huge dependencies in rarely changed code. (Not, that this matters too much in this example)
+ I feel like the springs are way too strong right now
+ Use a second collider to try and drive services away from each other
+ find better parameters for the kmeans clustering algorithm. Obvious clusters shouldnt be separated like on the second to last screenshot. 
+ find better ways to interact with the "net". Add support to grab a service and pin it to a position. Add support to shoot the net to add random force to it. When was the last time you developed software architectures while shooting the rocket launcher?
+ The number of clusters currently can be defined by the user in the Unity Editor. This is a hard set value and should probably be more dynamic for better results.
+ Use dots/ecs/jobs... for performance
+ Use a computeshader for performance
+ when the performance is no issue, implement more complex springs :)

# Thanks

to Robert Hofmann for valuable input

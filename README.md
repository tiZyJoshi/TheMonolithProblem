# TheMonolithProblem

Trying to solve the Monolith Problem as discussed in the FH Technikum Wien LVA SWA.
Services are represented as spheres. The dependencies between services are modeled as springs between those services. The strength of the spring is defined by the number of dependencies between the services. Let Unity Physics handle the rest.

Let the simulation run until you are happy with the result and hit space.
The services are clusterd by a kmeans clustering implementation found on github.
[https://github.com/ogxd/kmeans-clustering-unity](https://github.com/ogxd/kmeans-clustering-unity) 
Each cluster gets a unique color.

You can add (random) force to each sphere by hitting f. 
Hitting c will calculate clusters and add the same amount of (random) force to each member of a cluster
(Once, this worked, currently, it does not)

# Gephi

Checkout https://github.com/gephi/gephi !

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

+ find plausible parameters and better interaction methods for this simple implementation to work better.
    + Spring Strengths (!!)
    + Sphere Collisions?
    + use sphere sizes and masses to model something?
    + use sphere drag to model something?
    + Use a second, larger collider to try and drive services away from each other (?)
    + Initial velocities/forces/movements
        + Spawn all of the spheres at the same time with different movementvectors, probably in a conus, with one sphere pinned to the spawnlocation (or one per independent net)
        + Throw them like a fishing net (implement intertia)
        + "Hang up the net" (by pinning one or more spheres to a location) and add gravity to it (like hanging up an actual net)
        + Spawn all Spheres on random locations and then gradually add Springs
        + Spawn Spheres with all their springs attached gradually
    + calculate how many independent nets there are (if there are any; independent means: no dependencies between them) and treat them separately
    + calculate distances between services (distance: how many dependencies do i have to follow to get to another service in the same net) and use this information to find a nice spawning location for each service
    + find better ways to interact with the "net". Add support to grab a sphere and pin it to a position. Dont allow pins that would change the current cumulative force on other "pins" too much.
    + Add support to shoot the net to add random force to it. When was the last time you developed software architectures while shooting the rocket launcher?
    + implement a function that blows the net up for visual fidelity and relaxation
+ evaluate how the service data is mapped to the springs and find a better model.
+ Use different springs for better modellation possibilities of the different dependencies. Currently they can only pull and have dampening (Unity Spring Joint https://docs.unity3d.com/Manual/class-SpringJoint.html do research, they can probably do more)
+ better data = better results => I think you could improve the data a lot by adding a few data entries in the original data tables. Eg: Common Changes in relation to total changes. Currently, there are 60K dataentries regarding common changes, but 40k of them have 1 common change and another 10k or so have 2. For performance reasons I ignored those, but this can hide huge dependencies in rarely changed code. (Not, that this matters too much in this example)
+ find better parameters for the kmeans clustering algorithm. Obvious clusters shouldnt be separated like on the second to last screenshot. 
+ The number of clusters currently can be defined by the user in the Unity Editor. This is a hard set value and should probably be more dynamic for better results.
+ Use dots/ecs/jobs... for performance
+ Use a computeshader for performance
+ Use Particle System for performance
+ when the performance is no issue, implement more complex springs :)

# Thanks

to Robert Hofmann for valuable input

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StaniEdit
{
    [DataContract]
    class SpawnGroupDefinition {
        [DataMember]
        public double spawnChance = 1.0;
        [DataMember]
        public List<MeshDefinition> meshes = new List<MeshDefinition>();
        [DataMember]
        public List<ItemDefinition> items = new List<ItemDefinition>();
        [DataMember]
        public List<GuardDefinition> guards = new List<GuardDefinition>();
    }

    [DataContract]
    class MeshDefinition {
        [DataMember]
        public double x = 0;
        [DataMember]
        public double y = 0;
        [DataMember]
        public String staticMesh;
    }

    [DataContract]
    class GuardDefinition
    {
        [DataMember]
        public double x = 0;
        [DataMember]
        public double y = 0;
        [DataMember]
        public int patrolRouteIndex = -1;
        [DataMember]
        public int startIndex = 0;
    }

    [DataContract]
    class PatrolPointDefinition {
        [DataMember]
        public double x = 0;
        [DataMember]
        public double y = 0;
    }

    [DataContract]
    class ItemDefinition {
        [DataMember]
        public double x = 0;
        [DataMember]
        public double y = 0;
    }

    [DataContract]
    class PatrolRouteDefinition
    {
        [DataMember]
        public List<PatrolPointDefinition> patrolPoints = new List<PatrolPointDefinition>();
    }

    [DataContract]
    class RoomDefinition
    {
        [DataMember]
        public bool northDoor = false;
        [DataMember]
        public bool eastDoor = false;
        [DataMember]
        public bool southDoor = false;
        [DataMember]
        public bool westDoor = false;
        [DataMember]
        public int roomType = 0; //Types are: 0, normal; 1, treasure; 2, objective; 3, start.
        [DataMember]
        public int roomRarity = 0;
        [DataMember]
        public List<SpawnGroupDefinition> spawnGroups = new List<SpawnGroupDefinition>();
        [DataMember]
        public List<PatrolRouteDefinition> patrolRoutes = new List<PatrolRouteDefinition>();
    }
}

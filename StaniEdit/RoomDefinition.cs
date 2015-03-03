using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StaniEdit
{
    [DataContract]
    class MeshDefinition {
        [DataMember]
        public double x;
        [DataMember]
        public double y;
        [DataMember]
        public String staticMesh;
    }

    [DataContract]
    class RoomDefinition
    {
        [DataMember]
        public bool northDoor;
        [DataMember]
        public bool eastDoor;
        [DataMember]
        public bool southDoor;
        [DataMember]
        public bool westDoor;
        [DataMember]
        public List<MeshDefinition> meshes;
    }
}

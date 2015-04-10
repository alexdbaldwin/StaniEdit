using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StaniEdit
{
    [DataContract]
    class CustomMesh
    {
        [DataMember]
        public double width = 0.0;
        [DataMember]
        public double height = 0.0;
        [DataMember]
        public double originX = 0.0;
        [DataMember]
        public double originY = 0.0;
        [DataMember]
        public double startRotation = 0.0;
        [DataMember]
        public string assetName = "";

        public CustomMesh(double w, double h, double oX, double oY, double rot, string name) {
            width = w;
            height = h;
            originX = oX;
            originY = oY;
            startRotation = rot;
            assetName = name;
        }

        public override string ToString()
        {
            return assetName;
        }
    }
}

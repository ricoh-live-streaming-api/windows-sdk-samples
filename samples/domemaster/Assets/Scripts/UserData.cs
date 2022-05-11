using Newtonsoft.Json;
using UnityEngine;

public class UserData
{
    [JsonProperty("camera_position")]
    public Param Position { get; set; } = new Param();

    [JsonProperty("camera_rotation")]
    public Param Rotation { get; set; } = new Param();

    public class Param
    {
        [JsonProperty("x")]
        public float X { get; private set; }

        [JsonProperty("Y")]
        public float Y { get; private set; }

        [JsonProperty("z")]
        public float Z { get; private set; }

        public Param()
        {
        }

        public Param(Vector3 vector3)
        {
            X = vector3.x;
            Y = vector3.y;
            Z = vector3.z;
        }
    }
}
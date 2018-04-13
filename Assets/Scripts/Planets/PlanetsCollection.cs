using UnityEngine;
using UnityEditor;
using System;


public class PlanetsCollection
{
    [Serializable]
    public struct PlanetMain
    {
        public Planet.PlanetCharacteristics characteristics;
        public Planet.PlanetResources resources;
    }

    [Serializable]
    public struct PlanetSerialized
    {
        public PlanetMain planetMain;
        public int radius;
        public string texture;
        public float[] position;
        public string name;
    }

   
    public PlanetSerialized[] planets;
}
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ErrorColorGen 
{
    public Color color {get; set;}

    public ErrorColorGen() {
        GenerateRandomCOlor();
    }

    private void GenerateRandomCOlor(){

        color = new Color32(
            (byte)Random.Range(62,256),
            (byte)Random.Range(62,256),
            (byte)Random.Range(62,256),
            255
        );
    }
}
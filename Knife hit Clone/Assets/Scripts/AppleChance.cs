using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chance", menuName = "AppleChance")]
public class AppleChance : ScriptableObject
{
    [SerializeField] private int chance;

    public int Chance { get { return chance; } }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class EnergyRing : MonoBehaviour
{
    private Player player;

    private float MaxEnergy;

    Audio gameAudio;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindAnyObjectByType<Player>();
        }

        MaxEnergy = player.Energy;

        gameAudio = GameObject.Find("EnergyRingPass").GetComponent<Audio>();
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player.gameObject && player.Energy < MaxEnergy )
        {
            player.Energy += 1;
            ScoreData.AddEnergyRingCount(1);
            gameAudio.PlayStart();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateTrains : MonoBehaviour
{
    public GameObject train;
    public GameObject trainMoving;
    const float gap = 7.075f;
    private Vector3 positionStart = new Vector3(325.0f, 5.1f, 255.0f);
    private Vector3 position = new Vector3(325.0f, 5.1f, 255.0f);
    private const int instantiations = 12;

    // Start is called before the first frame update
    void Start()
    {
        long seed = System.DateTime.Now.Ticks;
        Random.seed = (int)seed/(Random.Range(1,50000));
        int clearTrack = Random.Range(0,5);
        Debug.Log("skipped track: " + clearTrack);
        for(int j = 0; j < 5; j++) {

            position = positionStart;
            position.x += (j*gap);
            position.z -= (j*gap);

            if(!(clearTrack == j)) {

                seed = System.DateTime.Now.Ticks;
                Random.seed = (int)seed/(Random.Range(1,50000));

                int gaps = Random.Range(0,6);
                int[] positions = new int[gaps];
                for (int i = 0; i < gaps; i++) {
                    Random.seed = (int)seed/(Random.Range(1,50000));
                    positions[i] = Random.Range(0,instantiations);
                }
                for (int i = 0; i < instantiations; i++){
                    if(!inSkip(i, positions, gaps)){
                        Instantiate(train, position, Quaternion.Euler(0f, 0f, 0f));
                    }
                    position.z += 8.15f;
                }
            } else {
                for (int i = 0; i < instantiations; i++){
                    Instantiate(trainMoving, position, Quaternion.Euler(0f,0f,0f));
                    position.z += 8.2f;
                }
            }

        }
    }
    
    bool inSkip(int i, int[] positions, int gaps) {
        for(int j = 0; j < gaps; j++) {
            if(i == positions[j]) return true;
        }
        return false;
    }
}

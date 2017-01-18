using UnityEngine;

public class HandleCollapse : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameState.State.OnLose();
    }
}

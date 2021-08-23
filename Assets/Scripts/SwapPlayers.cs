using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapPlayers : MonoBehaviour
{
    public GameObject withGun;
    public GameObject withoutGun;

    bool holdsGun;
    bool didSwitch;

    // Start is called before the first frame update
    void Start()
    {
        holdsGun = false;
        didSwitch = false;
    }

    // Update is called once per frame
    void Update()
    {
        holdsGun = withoutGun.GetComponent<PlayerMove>().GetHoldsGun();
        if (!didSwitch && holdsGun)
        {
            withGun.transform.position = withoutGun.transform.position;
            withGun.transform.GetChild(0).transform.gameObject.transform.rotation = withoutGun.transform.GetChild(0).gameObject.transform.rotation;
            withGun.transform.rotation = withoutGun.transform.rotation;
            withGun.SetActive(true);
            withoutGun.SetActive(false);
            withGun.GetComponent<PlayerMove>().SetHp(withoutGun.GetComponent<PlayerMove>().GetHp());
            withGun.GetComponent<PlayerMove>().SetHoldsGun(true);
            withoutGun.GetComponent<PlayerMove>().SetHoldsGun(true);
            withGun.GetComponent<PlayerMove>().SetHoldsGrenade(withoutGun.GetComponent<PlayerMove>().GetHoldsGrenade());
            didSwitch = true;
        }
        else if (holdsGun && !withGun.GetComponent<PlayerMove>().GetIsAlive())
        {
            withoutGun.transform.position = withGun.transform.position;
            withoutGun.transform.rotation = withGun.transform.rotation;
            withoutGun.GetComponent<PlayerMove>().SetHp(withGun.GetComponent<PlayerMove>().GetHp());
            withoutGun.GetComponent<PlayerMove>().TakeDamage(0);
            withoutGun.SetActive(true);
            withGun.SetActive(false);
        }
        else if (!holdsGun && !withoutGun.GetComponent<PlayerMove>().GetIsAlive())
        {
            withGun.GetComponent<PlayerMove>().SetIsAlive(false);
        }
    }
}

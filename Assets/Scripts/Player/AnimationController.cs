using UnityEngine;
using UnityEngine.Animations.Rigging;
public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private Transform HandRight;
    [SerializeField] private Transform HandLeft;

    [SerializeField] private Rig rigHand;
    [SerializeField] private Rig rigTorso;

    [SerializeField] private Transform AimTorso;


    public void isMove(bool move)
    {
        animator.SetBool("Move", move);
    }


    public void RigOnOff(float wightRig)
    {
        rigHand.weight = wightRig;
      
    }

    public void moveHand(Transform right,Transform left)
    {
        HandRight.transform.position = right.position;
        HandLeft.transform.position = left.position;
        HandRight.transform.rotation = right.rotation;
        HandLeft.transform.rotation = left.rotation;
    }


    public void isDead()
    {
        animator.SetTrigger("Dead");
    }



    public void AimingOn(Transform target)
    {
        rigTorso.weight = 0.5f;
        AimTorso.position = target.position;
    }
    
    public void AimingOff()
    {
        rigTorso.weight = 0f;
    }

}
